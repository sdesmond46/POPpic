using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace BuddyServiceClient
{
  

     internal class BuddyServiceClientHttp :BuddyServiceClientBase
     {
         static IDictionary<string, HttpRequestType> RequestTypeOverrides = new Dictionary<string, HttpRequestType>();


         public bool LoggingEnabled { get; set; }

         protected override string ClientName
         {
			get{
        		return "DotNet";
			}
         }

         private string sdkVersion;
         protected override string ClientVersion
         {
             get { return  sdkVersion; }
         }
        private string SdkVersion {
            get {
                return String.Format("{0};{1}", ClientName, ClientVersion); 
            }
        }

        internal enum HttpRequestType {
            HttpPostUrlEncoded,
            HttpPostMultipartForm,
            HttpGet
        }

      
        public override bool IsLocal
        {
            get
            {
                return ServiceRoot.Contains("localhost");
            }
        }

        public string ServiceRoot
        {
            get;
            private set;
        }

        private HttpRequestType _requestType = HttpRequestType.HttpPostUrlEncoded;
        public HttpRequestType RequestType
        {
            get
            {
                return _requestType;
            }
            set
            {
                _requestType = value;
            }
        }

        static BuddyServiceClientHttp()
        {
            RequestTypeOverrides["Sound_Sounds_GetSound"] = HttpRequestType.HttpGet;
            RequestTypeOverrides["Blobs_Blob_GetBlob"] = HttpRequestType.HttpGet;
            RequestTypeOverrides["Videos_Video_GetVideo"] = HttpRequestType.HttpGet;

            RequestTypeOverrides["Videos_Video_AddVideo"] = HttpRequestType.HttpPostMultipartForm;
            RequestTypeOverrides["Blobs_Blob_AddBlob"] = HttpRequestType.HttpPostMultipartForm;
            RequestTypeOverrides["Pictures_Photo_Add"] = HttpRequestType.HttpPostMultipartForm;
            RequestTypeOverrides["Pictures_Photo_AddWithWatermark"] = HttpRequestType.HttpPostMultipartForm;
            RequestTypeOverrides["Pictures_ProfilePhoto_Add"] = HttpRequestType.HttpPostMultipartForm;
        }

        public BuddyServiceClientHttp(string root, string sdkVersion)
        {
            if (String.IsNullOrEmpty(root)) throw new ArgumentNullException("root");
            ServiceRoot = root;
            this.sdkVersion = sdkVersion;
            //LoggingEnabled = true;
        }

        public virtual void LogRequest(string method, string url, string body)
        {
            if (LoggingEnabled)
            {
                Debug.WriteLine("{0}: {1}", method, url);
                if (body != null)
                {
                    Debug.WriteLine(body);
                    
                }
            }
        }

        public virtual void LogResponse(string method, string body, TimeSpan time, HttpWebResponse response = null)
        {
            if (LoggingEnabled){
                Debug.WriteLine("{1}: {0} ({2:0.0}ms)", response == null ? "(null)" : response.StatusCode.ToString(), method, time.TotalMilliseconds);
                if (body != null)
                {
                    Debug.WriteLine(body);
                 
                }
            }
        }



        public override void CallMethodAsync<T>(string methodName, IDictionary<string, object> parameters, Action<BuddyCallResult<T>> callback)
        {
      
            var requestType = RequestType;

            if (RequestTypeOverrides.ContainsKey(methodName))
            {
                requestType = RequestTypeOverrides[methodName];
            }

            CallMethodAsync<T>(methodName, requestType, parameters, callback);
                
        }

        public void CallMethodAsync<T>(string methodName, HttpRequestType requestType, IDictionary<string, object> parameters, Action<BuddyCallResult<T>> callback)
        {
            DateTime start = DateTime.Now;

            Action<Exception, BuddyCallResult<T>> handleException = (ex, bcr) =>
            {
               
                WebException webEx = ex as WebException;
                HttpWebResponse response = null;
                var err = BuddyError.UnknownServiceError;

                bcr.Message = ex.ToString();
                if (webEx != null )
                {
                    err = BuddyError.InternetConnectionError;

                    if (webEx.Response != null) {
                        response = (HttpWebResponse)webEx.Response;
                        bcr.Message = response.StatusDescription;
                    }
                    else {
                        bcr.Message = webEx.Status.ToString();
                    }

                }
               
                bcr.Error = err;
                LogResponse(methodName, bcr.Message,DateTime.Now.Subtract(start), response);
                  
                callback(bcr);
            };

           
            GetResponse(methodName, parameters, requestType, (ex, response) =>
            {
                var bcr = new BuddyCallResult<T>();
				var isResponseRequest = typeof(T).Equals(typeof(HttpWebResponse));

                if (isResponseRequest)
                {
                    bcr.Result = (T)(object)response;
                }

                if (ex != null)
                {
                    handleException(ex, bcr);
                    
                    return;
                }
                else if (response != null)
                {

                    if (!isResponseRequest)
                    {
                    
                        string body = null;
                        try
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                body = new StreamReader(responseStream).ReadToEnd();
                            }

                        }
                        catch (Exception rex)
                        {
                            handleException(rex, bcr);
                            return;
                        }


                        LogResponse(methodName, body, DateTime.Now.Subtract(start), response);

                        var err = GetBuddyError(body);

                        if (err != BuddyError.None)
                        {
                            bcr.Error = err;
                            callback(bcr);
                            return;
                        }


                        if (typeof(T).Equals(typeof(string)))
                        {
                            bcr.Result = (T)Convert.ChangeType(body, typeof(T), CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var serializer = Newtonsoft.Json.JsonSerializer.Create(null);


                            //json parse
                            try
                            {
                                var envelope = serializer.Deserialize<JsonEnvelope<T>>(new JsonTextReader(new StringReader(body)));
                                bcr.Result = envelope.data;

                            }
                            catch
                            {
                                bcr.Error = BuddyError.UnknownServiceError;
                                bcr.Message = body;
                            }
                        }
                    }
                    try
                    {
                        callback(bcr);
                    }
                    catch (Exception ex3)
                    {
                        handleException(ex3, bcr);
                    }

                }
            });


        }

        private const int EncodeChunk = 32000;

        private static string EscapeDataString(string value)
        {
            StringBuilder encoded = new StringBuilder(value.Length);

            var pos = 0;

            // encode the string in 32K chunks.
            while (pos < value.Length)
            {
                var len = Math.Min(EncodeChunk, value.Length - pos);
                var encodedPart = value.Substring(pos, len);
                encodedPart = Uri.EscapeDataString(encodedPart);
                encoded.Append(encodedPart);
                pos += EncodeChunk;
            }
            return encoded.ToString();
        }


        private string GetUrlEncodedParameters(IDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null) continue;

                string val = null;
                if (kvp.Value is BuddyFile)
                {
                    val = Convert.ToBase64String(((BuddyFile)kvp.Value).Bytes);
                    val = EscapeDataString(val);
                }
                else
                {
                    val = EscapeDataString(kvp.Value.ToString());

                }
                sb.AppendFormat("{2}{0}={1}", kvp.Key, val, isFirst ? "" : "&");
                isFirst = false;
            }
            return sb.ToString();
        }

        private const int TimeoutMilliseconds = 30000 * 8;

        private void GetResponse(string methodName, IDictionary<string, object> parameters, HttpRequestType requestType, Action<Exception, HttpWebResponse> callback)
        {

            var url = String.Format("{0}/Service/V1/BuddyService.ashx?{1}", ServiceRoot, methodName);


            switch (requestType)
            {

                case HttpRequestType.HttpGet:
                    // add the parameters to the url.
                    //
                    url += "&" + GetUrlEncodedParameters(parameters);
                    break;
            }

            HttpWebRequest wr = null;

            try
            {
                wr = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                callback(ex, null);
                return;
            }

            wr.Headers["BuddyPlatformSDK"] = SdkVersion;
#if WINDOWS_PHONE
            // no timeout prop on WP8
            var timeoutProp = wr.GetType().GetProperty("Timeout");
            if (timeoutProp != null) {
                timeoutProp.SetValue(wr, 400000, null);
            }
#endif
           
            switch (requestType)
            {

                case HttpRequestType.HttpGet:
                    wr.Method = "GET";
                    break;
                default:
                    wr.Method = "POST";
                    break;

            }

            Action getResponse = () =>
            {
                try
                {
                    int requestStatus = -1;
                    LogRequest(methodName, url, null);
                    var asyncResult = wr.BeginGetResponse((async2) =>
                    {   
                        try
                        {
                            lock (wr)
                            {
                                if (requestStatus == 1)
                                {
                                    throw new WebException("Request timed out.", WebExceptionStatus.RequestCanceled);
                                }
                                else
                                {
                                    HttpWebResponse response = (HttpWebResponse)wr.EndGetResponse(async2);
                                    callback(null, response);
                                }
                            }
                        }
                        catch (WebException ex)
                        {
                            callback(ex, null);
                        }
                    }, null);

                    // spin up a timer to check for timeout.  not all platforms
                    // support proper threadpool wait.
                    //
                    Action timeoutHandler = () =>
                    {
                        lock (wr)
                        {
                            if (requestStatus == -1)
                            {
                                requestStatus = 1;
                                wr.Abort();
                            }
                        }
                    };
#if WINRT
                    TimeSpan delay = TimeSpan.FromMilliseconds(TimeoutMilliseconds);

                    Windows.System.Threading.ThreadPoolTimer.CreateTimer(
                        (source) =>
                        {
                            timeoutHandler();
                        }, delay);
         
#else
                    new System.Threading.Timer((state) =>
                    {
                       timeoutHandler();
                    }, null, TimeoutMilliseconds, System.Threading.Timeout.Infinite);
#endif

                }
                catch (WebException wex)
                {
                    LogResponse(methodName, wex.ToString(), TimeSpan.Zero);
                    callback(wex, null);
                }
            };

            try
            {
                if (HttpRequestType.HttpGet == requestType)
                {
                   getResponse();
                }
                else
                {
                    wr.BeginGetRequestStream((async) =>
                    {
                        try
                        {
                            using (var rs = wr.EndGetRequestStream(async))
                            {
                                switch (requestType)
                                {
                                    case HttpRequestType.HttpPostUrlEncoded:
                                        wr.ContentType = "application/x-www-form-urlencoded; charset=utf-8;";
                                        var body = GetUrlEncodedParameters(parameters);
                                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(body);
                                        rs.Write(formitembytes, 0, formitembytes.Length);
                                        LogRequest(methodName, url, body);
                                        break;
                                    case HttpRequestType.HttpPostMultipartForm:
                                        HttpPostMultipart(wr, rs, parameters);
                                        break;
                                }
                                rs.Flush();
                            }

                            getResponse();

                        }
                        catch (WebException wex)
                        {
                            LogResponse(methodName, wex.ToString(), TimeSpan.Zero);
                               
                            callback(wex, null);
                        }


                    }, null);
                }
            }
            catch (WebException wex)
            {
                LogResponse(methodName, wex.ToString(), TimeSpan.Zero);
                               
                callback(wex, null);
            }

        }





        private static void HttpPostMultipart(HttpWebRequest wr, Stream requestStream, IDictionary<string, object> nvc)
        {
            var files = new List<BuddyFile>();
            
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            wr.ContentType = "multipart/form-data; boundary=" + boundary;

            var partBoundary = "\r\n--" + boundary + "\r\n";
            var boundarybytes = Encoding.UTF8.GetBytes(partBoundary);

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (var kvp in nvc)
            {
                if (kvp.Value == null) continue;

                if (kvp.Value is BuddyFile)
                {
                    files.Add((BuddyFile)kvp.Value);
                    continue;
                }

                requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, kvp.Key, kvp.Value.ToString());
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                requestStream.Write(formitembytes, 0, formitembytes.Length);
            }

            requestStream.Write(boundarybytes, 0, boundarybytes.Length);

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, file.Name, file.Name, file.ContentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerbytes, 0, headerbytes.Length);
                requestStream.Write(file.Bytes, 0, (int)file.Data.Length);
            }

            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
        }
       
     }


  [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#if PUBLIC_SERIALIZATION
        public
#else
     internal
#endif
 class JsonEnvelope<T>
     {
         public T data = default(T);
     }


}

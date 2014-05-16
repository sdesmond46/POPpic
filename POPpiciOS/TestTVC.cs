using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using POPpicLibrary;
using System.Net;
using System.Threading.Tasks;

namespace POPpiciOS
{
	public partial class TestTVC : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("TestTVC", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("TestTVC");

		public TestTVC (IntPtr handle) : base (handle)
		{
			this.SelectionStyle = UITableViewCellSelectionStyle.Blue;
		}

		public static TestTVC Create ()
		{
			return (TestTVC)Nib.Instantiate (null, null) [0];
		}

		GameViewModel viewModel;
		WebClient webClient = new WebClient();
		public void SetData(GameViewModel viewModel) {
			webClient.CancelAsync ();
			this.viewModel = viewModel;
			this.UserNameLabel.Text = this.viewModel.OpponentName;
			this.LastMoveLabel.Text = this.viewModel.PreviousActionTimeStamp;
			this.TimeElapsedLabel.Text = this.viewModel.PreviousActionDescription;

			IOSUtilities.SetImageFromStream (PopPicImageCache.GetUserProfileImage (
				this.viewModel.Opponent.ID, 
				this.viewModel.Opponent.ProfilePicture.ToString (), 
				TimeSpan.FromMinutes (5)),
				this.UserProfileImage,
				this);

		}
	}
}


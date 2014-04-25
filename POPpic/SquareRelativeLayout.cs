using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace POPpic
{
	public class SquareRelativeLayout : RelativeLayout
	{
//		@Override
//		public void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
//			super.onMeasure(widthMeasureSpec, widthMeasureSpec);
//		}

		public SquareRelativeLayout(Context c) : base(c) {}
		public SquareRelativeLayout(Context c, Android.Util.IAttributeSet a) : base(c, a) {}
		public SquareRelativeLayout(Context c, Android.Util.IAttributeSet a, int d) : base(c, a, d) {}
		public SquareRelativeLayout(IntPtr p, JniHandleOwnership h) : base(p, h) {}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, widthMeasureSpec);
		}
	}
}


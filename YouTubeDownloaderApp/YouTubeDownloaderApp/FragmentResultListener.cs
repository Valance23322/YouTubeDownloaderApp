using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouTubeDownloaderApp
{
    internal class FragmentResultListener : Java.Lang.Object, IFragmentResultListener
    {
        Action<Bundle> resultAction { get; set; }

        public FragmentResultListener(Action<Bundle> resultAction)
        {
            this.resultAction = resultAction;
        }
        public void OnFragmentResult(string requestKey, Bundle bundle)
        {
            resultAction(bundle);
        }
    }
}
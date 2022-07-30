using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouTubeDownloaderApp
{
    public class MainFragment : AndroidX.Fragment.App.Fragment
    {

        public Button DownloadBtn { get; set; }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.MainFragment, container, false);

            DownloadBtn = view.FindViewById<Button>(Resource.Id.DownloadBtn);
            DownloadBtn.Click += ShowDownloadOptions;

            ParentFragmentManager.SetFragmentResultListener("DownloadOptions", ViewLifecycleOwner, new FragmentResultListener(OnFinishDownloadOptionsDialog));

            return view;
        }

        protected virtual void ShowDownloadOptions(object sender, EventArgs e)
        {
            var DownloadOptionsFragment = new DownloadOptions();
            DownloadOptionsFragment.Show(ParentFragmentManager, "dialog");
        }

        public void OnFinishDownloadOptionsDialog(Bundle bundle)
        {
            List<string> resultList = new List<string>();
            resultList.Add($"Download Result: {bundle.GetString("DownloadResult")}");
            resultList.Add($"File Name: {bundle.GetString("FileName")}");
            resultList.Add($"Save Folder: {bundle.GetString("SaveFolder")}");
            Console.WriteLine($"\n\n\n\n{string.Join("\n", resultList)}\n\n\n\n");
        }
    }
}
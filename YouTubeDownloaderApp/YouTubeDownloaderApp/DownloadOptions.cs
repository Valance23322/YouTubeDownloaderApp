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
    public class DownloadOptions : AndroidX.Fragment.App.DialogFragment
    {
        public Button ContinueDownloadBtn { get; set; }
        public Button CancelDownloadBtn { get; set; }
        public EditText FileNameTxt { get; set; }
        public EditText SaveFolderTxt { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.DownloadOptions, container, false);

            ContinueDownloadBtn = view.FindViewById<Button>(Resource.Id.ContinueDownloadBtn);
            CancelDownloadBtn = view.FindViewById<Button>(Resource.Id.CancelDownloadBtn);
            FileNameTxt = view.FindViewById<EditText>(Resource.Id.FileNameTxt);
            SaveFolderTxt = view.FindViewById<EditText>(Resource.Id.SaveFolderTxt);


            ContinueDownloadBtn.Click += ContinueVideoDownload;
            CancelDownloadBtn.Click += CancelVideoDownload;

            return view;
        }

        protected virtual void ContinueVideoDownload(object sender, EventArgs e)
        {
            Bundle result = new Bundle();
            result.PutString("DownloadResult", "DOWNLOAD TIME!!!!");
            result.PutString("FileName", FileNameTxt.Text);
            result.PutString("SaveFolder", SaveFolderTxt.Text);
            ParentFragmentManager.SetFragmentResult("DownloadOptions", result);
            Dismiss();
        }

        protected virtual void CancelVideoDownload(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}
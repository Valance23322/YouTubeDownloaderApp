using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouTubeDownloaderApp
{
    public class DownloadOptionsFragment : AndroidX.Fragment.App.DialogFragment, IActivityResultCallback
    {
        public Button SaveDownloadBtn { get; set; }
        public Button CancelDownloadBtn { get; set; }
        public EditText FileNameTxt { get; set; }

        public Action<string, Android.Net.Uri> DownloadAction { get; set; }
        ActivityResultLauncher ArlStartForResult { get; set; }
        private Android.Net.Uri saveFileUri { get; set; }

        public DownloadOptionsFragment(Action<string, Android.Net.Uri> DownloadAction) : base()
        {
            this.DownloadAction = DownloadAction;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ArlStartForResult = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.DownloadOptions, container, false);

            SaveDownloadBtn = view.FindViewById<Button>(Resource.Id.SaveDownloadBtn);
            CancelDownloadBtn = view.FindViewById<Button>(Resource.Id.CancelDownloadBtn);
            FileNameTxt = view.FindViewById<EditText>(Resource.Id.FileNameTxt);

            SaveDownloadBtn.Click += SaveVideoDownload;
            CancelDownloadBtn.Click += CancelVideoDownload;

            return view;
        }

        protected virtual void SaveVideoDownload(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FileNameTxt.Text))
            {
                new AlertDialog.Builder(Context)
                    .SetTitle("Missing Fields")
                    .SetMessage($"The Save File Name must be filled in.")
                    .SetNegativeButton(Android.Resource.String.Ok, (EventHandler<DialogClickEventArgs>)null)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .Show();
                return;
            }

            FileNameTxt.Text = FileNameTxt.Text.EndsWith(".mp4") ? FileNameTxt.Text : $"{FileNameTxt.Text}.mp4";

            Intent i = new Intent(Intent.ActionCreateDocument);
            i.AddCategory(Intent.CategoryOpenable);
            i.SetType("video/mp4");
            i.PutExtra(Intent.ExtraTitle, FileNameTxt.Text);
            ArlStartForResult.Launch(i);
        }

        protected virtual void CancelVideoDownload(object sender, EventArgs e)
        {
            Dismiss();
        }

        public void OnActivityResult(Java.Lang.Object p0)
        {
            if (p0 is ActivityResult)
            {
                ActivityResult result = p0 as ActivityResult;

                if (result.ResultCode == (int)Result.Ok)
                {
                    saveFileUri = result.Data.Data;

                    DownloadAction(FileNameTxt.Text, saveFileUri);
                    Dismiss();
                }
            }
        }
    }
}
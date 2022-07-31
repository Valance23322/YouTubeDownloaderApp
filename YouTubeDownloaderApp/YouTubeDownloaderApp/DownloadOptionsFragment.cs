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
        public Button ChangeFolderBtn { get; set; }
        public Button ContinueDownloadBtn { get; set; }
        public Button CancelDownloadBtn { get; set; }
        public EditText FileNameTxt { get; set; }
        public TextView SaveFolderPathTxt { get; set; }

        public Action<string, string> DownloadAction { get; set; }
        ActivityResultLauncher ArlStartForResult { get; set; }

        private string saveFolderPath { get; set; }

        public DownloadOptionsFragment(Action<string, string> DownloadAction) : base()
        {
            this.DownloadAction = DownloadAction;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            saveFolderPath = string.Empty;
            ArlStartForResult = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.DownloadOptions, container, false);

            ChangeFolderBtn = view.FindViewById<Button>(Resource.Id.ChangeFolderBtn);
            ContinueDownloadBtn = view.FindViewById<Button>(Resource.Id.ContinueDownloadBtn);
            CancelDownloadBtn = view.FindViewById<Button>(Resource.Id.CancelDownloadBtn);
            FileNameTxt = view.FindViewById<EditText>(Resource.Id.FileNameTxt);
            SaveFolderPathTxt = view.FindViewById<TextView>(Resource.Id.SaveFolderPathTxt);

            ChangeFolderBtn.Click += ChangeSaveFolder;
            ContinueDownloadBtn.Click += ContinueVideoDownload;
            CancelDownloadBtn.Click += CancelVideoDownload;

            return view;
        }

        protected virtual void ChangeSaveFolder(object sender, EventArgs e)
        {
            Intent i = new Intent(Intent.ActionOpenDocumentTree);
            i.AddCategory(Intent.CategoryDefault);
            ArlStartForResult.Launch(i);
        }

        protected virtual void ContinueVideoDownload(object sender, EventArgs e)
        {
            StringBuilder missingFields = new StringBuilder();
            if (string.IsNullOrWhiteSpace(FileNameTxt.Text))
            {
                missingFields.Append("Save File Name");
            }
            if (string.IsNullOrWhiteSpace(saveFolderPath))
            {
                missingFields.Append($"{(missingFields.Length > 0 ? " and " : "")}Save Folder");
            }
            if (missingFields.Length > 0)
            {
                new AlertDialog.Builder(Context)
                    .SetTitle("Missing Fields")
                    .SetMessage($"The {missingFields} field(s) must be filled in.")
                    .SetNegativeButton(Android.Resource.String.Ok, (EventHandler<DialogClickEventArgs>)null)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .Show();
                return;
            }

            DownloadAction(FileNameTxt.Text, saveFolderPath);
            Dismiss();
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
                    string filePath = result.Data.Data.Path;
                    SaveFolderPathTxt.Text = filePath.Substring(filePath.IndexOf(":") + 1);
                    saveFolderPath = filePath;
                }
            }
        }
    }
}
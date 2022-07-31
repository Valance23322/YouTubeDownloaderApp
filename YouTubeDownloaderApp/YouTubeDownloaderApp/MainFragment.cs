using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeDownloaderApp
{
    public class MainFragment : AndroidX.Fragment.App.Fragment, IActivityResultCallback
    {
        public Button DownloadBtn { get; set; }
        public Spinner websiteSelectionSpinner { get; set; }

        public EditText UrlInputEditText { get; set; }

        public EditText ChannelIDEditText { get; set; }

        public Action<string, Android.Net.Uri> DownloadAction { get; set; }
        ActivityResultLauncher ArlStartForResult { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            ArlStartForResult = RegisterForActivityResult(new ActivityResultContracts.RequestMultiplePermissions(), this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            SetupDownloadDialogAction();

            var view = inflater.Inflate(Resource.Layout.MainFragment, container, false);
            
            DownloadBtn = view.FindViewById<Button>(Resource.Id.DownloadBtn);
            DownloadBtn.Click += ShowDownloadOptions;

            websiteSelectionSpinner = view.FindViewById<Spinner>(Resource.Id.websiteSelection);
            websiteSelectionSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(websiteSelected);

            var adapter = ArrayAdapter.CreateFromResource(this.Context, Resource.Array.websiteSelectionOptions, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            websiteSelectionSpinner.Adapter = adapter;



            UrlInputEditText = view.FindViewById<EditText>(Resource.Id.URLBox);
            UrlInputEditText.SetHint(Resource.String.url_hint_text);

            ChannelIDEditText = view.FindViewById<EditText>(Resource.Id.ChannelID);
            ChannelIDEditText.SetHint(Resource.String.channel_id_hint_text);

            return view;
        }

        public virtual void SetupDownloadDialogAction()
        {
            DownloadAction = ReceiveDownloadParams;
        }

        protected virtual void ShowDownloadOptions(object sender, EventArgs e)
        {
            StringBuilder missingFields = new StringBuilder();
            if (websiteSelectionSpinner.SelectedItemId == 0)
            {
                missingFields.Append("The Website must be selected");
            }
            if (string.IsNullOrWhiteSpace(UrlInputEditText.Text))
            {
                missingFields.Append($"{(missingFields.Length > 0 ? " and the " : "The ")}URL must be filled in");
            }
            if (missingFields.Length > 0)
            {
                new AlertDialog.Builder(Context)
                    .SetTitle("Missing Fields")
                    .SetMessage($"{missingFields}.")
                    .SetNegativeButton(Android.Resource.String.Ok, (EventHandler<DialogClickEventArgs>)null)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .Show();
                return;
            }

            bool readExternal = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted;
            bool writeExternal = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted;
            if (!readExternal || !writeExternal)
            {
                List<string> requestPermissions = new List<string>();
                if (!readExternal)
                {
                    requestPermissions.Add(Manifest.Permission.ReadExternalStorage);
                }
                if (!writeExternal)
                {
                    requestPermissions.Add(Manifest.Permission.WriteExternalStorage);
                }

                ArlStartForResult.Launch(requestPermissions.ToArray());
            }
            else
            {
                new DownloadOptionsFragment(DownloadAction).Show(ParentFragmentManager, "dialog");
            }
        }

        //this is where we would change the call to the different methods for different websites
        public void websiteSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner websiteSelected = (Spinner)sender;
            if (websiteSelectionSpinner.SelectedItem.ToString() == "YouTube")
            {
                ChannelIDEditText.Visibility = ViewStates.Visible;
            }
            else
            {
                ChannelIDEditText.Visibility = ViewStates.Invisible;
            }

        }

        public virtual async void ReceiveDownloadParams(string fileName, Android.Net.Uri fileUri)
        {
            string videoURL = this.UrlInputEditText.Text;            
            string errorMessage = await YouTubeDownloaderService.DownloadVideoAsync(Context.ContentResolver, fileName, fileUri, videoURL);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                new AlertDialog.Builder(Context)
                    .SetTitle("Error")
                    .SetMessage(errorMessage)
                    .SetNegativeButton(Android.Resource.String.Ok, (EventHandler<DialogClickEventArgs>)null)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .Show();
            }
            else
            {
                Toast.MakeText(Context, "Download Completed!", ToastLength.Short).Show();
            }
        }

        public void OnActivityResult(Java.Lang.Object p0)
        {
            if (p0 is Java.Util.HashMap)
            {
                Java.Util.HashMap map = p0 as Java.Util.HashMap;
                if(map.Values().Cast<bool>().All(x => x))
                {
                    ShowDownloadOptions(null, null);
                }
                else
                {
                    new AlertDialog.Builder(Context)
                    .SetTitle("Permissions Denied")
                    .SetMessage("Can't download videos without read and write permissions")
                    .SetNegativeButton(Android.Resource.String.Ok, (EventHandler<DialogClickEventArgs>)null)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .Show();
                }
            }
        }
    }
}
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
        public Spinner websiteSelectionSpinner { get; set; }

        public Action<string, string> DownloadAction { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
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


            EditText urlInput = view.FindViewById<EditText>(Resource.Id.URLBox);
            urlInput.KeyPress += (object sender, View.KeyEventArgs keyPress) =>
            {
                keyPress.Handled = false;
                if (keyPress.Event.Action == KeyEventActions.Down && keyPress.KeyCode == Keycode.Enter)
                {
                    Toast.MakeText(this.Context, "That's a URL!", ToastLength.Short).Show();
                }
            };

            return view;
        }

        public virtual void SetupDownloadDialogAction()
        {
            DownloadAction = ReceiveDownloadParams;
        }

        protected virtual void ShowDownloadOptions(object sender, EventArgs e)
        {
            new DownloadOptionsFragment(DownloadAction).Show(ParentFragmentManager, "dialog");
        }

        public void websiteSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner websiteSelected = (Spinner)sender;
            string toast = "You selected a website!";
            Toast.MakeText(this.Context, toast, ToastLength.Short).Show();
        }

        public virtual void ReceiveDownloadParams(string fileName, string saveFolder)
        {
            List<string> resultList = new List<string>();
            resultList.Add($"File Name: {fileName}");
            resultList.Add($"Save Folder: {saveFolder}");
            Console.WriteLine($"\n\n\n\n{string.Join("\n", resultList)}\n\n\n\n");
        }
    }
}
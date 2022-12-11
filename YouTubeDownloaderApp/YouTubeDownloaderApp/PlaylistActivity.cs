using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YouTubeDownloaderApp
{
    [Activity]
    public class PlaylistActivity : AppCompatActivity
    {

        public FrameLayout FragmentContainer { get; set; }

        public PlaylistViewFragment PlaylistViewFragment { get; set; }

        public const string URLParameter = "url";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);

            FragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragmentContainer);

            PlaylistViewFragment = new PlaylistViewFragment(this.Intent.GetStringExtra(URLParameter));

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.fragmentContainer, PlaylistViewFragment)
                .CommitAllowingStateLoss();
        }

        
    }
}

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
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using FFImageLoading;



namespace YouTubeDownloaderApp
{
    public class PlaylistViewFragment : AndroidX.Fragment.App.Fragment
    {
        public RecyclerView PlaylistRecycler { get; set; }
        public IList<Playlist> PlaylistsList { get; set; }

        public string PlaylistURL { get; set; }
        
        public PlaylistViewFragment(string url)
        {
            PlaylistURL = url;
        }
        public void RefreshUI (IList<Playlist> playlist)
        {
            this.Activity.RunOnUiThread(() =>
            {
                PlaylistsList = playlist;
                UpdateAdapter(PlaylistRecycler);
            });
            
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PlaylistViewFragment, container, false);
            this.PlaylistRecycler = view.FindViewById<RecyclerView>(Resource.Id.PlaylistRecycler);

            PlaylistRecycler.SetLayoutManager(new LinearLayoutManager(Context));

            GetPlaylistInfoAsync(PlaylistURL);
            return view;
        }

        private void UpdateAdapter(RecyclerView PlaylistRecycler)
        {
            PlaylistRecycler.SetAdapter(new PlaylistAdapter(PlaylistsList));
        }

        public virtual async Task GetPlaylistInfoAsync(string url)
        {
            var data = await YouTubeDownloaderService.DownloadPlaylistAsync(url, this.Context);
            Console.WriteLine(data.FirstOrDefault()?.Snippet?.Title);
            this.RefreshUI(data);

        }
    }

    public class PlaylistHolder : RecyclerView.ViewHolder
    {
        public ImageView PlaylistThumbnail { get; set; }
        public TextView PlaylistTitle { get; set; }
        public TextView NumberOfPlaylist { get; set; }

        //set data
        public void SetUI (Playlist playlist)
        {
            string url = playlist.Snippet.Thumbnails.Maxres?.Url ?? playlist.Snippet.Thumbnails.Default__?.Url ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(url))
            {
                ImageService.Instance.LoadUrl(url).Into(PlaylistThumbnail);
            }
            
            PlaylistTitle.Text = playlist.Snippet.Title;
            NumberOfPlaylist.Text = $"{playlist.ContentDetails.ItemCount} videos";
        }
        public PlaylistHolder(View itemView) : base(itemView)
        {
            PlaylistThumbnail = itemView.FindViewById<ImageView>(Resource.Id.PlaylistThumbnail);
            PlaylistTitle = itemView.FindViewById<TextView>(Resource.Id.PlaylistTitle);
            NumberOfPlaylist = itemView.FindViewById<TextView>(Resource.Id.NumberOfPlaylists);

        }
    }

    public class PlaylistAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => channelPlaylists.Count;

        IList<Playlist> channelPlaylists;
        public PlaylistAdapter (IList<Playlist> playlists)
        {
            channelPlaylists = playlists;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PlaylistHolder playlistHolder = holder as PlaylistHolder;
            playlistHolder.SetUI(channelPlaylists[position]);
        }
        public override int GetItemViewType(int position)
        {
            return Resource.Layout.PlaylistList;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(viewType, parent, false);
            return new PlaylistHolder(view);
        }
    }
}
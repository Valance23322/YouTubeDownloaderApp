using Android;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

public class YouTubeDownloaderService
{
    public static async Task<string> DownloadVideoAsync(string fileName, string path, string videoURL)
    {
        string errorMessage = null;
        var youTube = YouTube.Default; // starting point for YouTube actions
        try
        {
            var video = youTube.GetVideo(videoURL); // gets a Video object with info about the video
            File.WriteAllBytes(Path.Combine(path, fileName), video.GetBytes());
        }
        catch(ArgumentException ex)
        {
            errorMessage = ex.Message;
        }
        catch(Exception ex)
        {
            errorMessage = ex.Message;
        }
        return errorMessage;
    }

    public static async void DownloadPlaylistAsync()
    {
        //TODO Use YouTube API -> Channels.List, part = id forUsername = channel username, then Playlists.List, channelID = ID from Channels.List return, part = snippet, id = id from playlist URL
    }

    public virtual async Task PlayListAPIAsync()
    {
        UserCredential credential;
        using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                // This OAuth 2.0 access scope allows for full read/write access to the
                // authenticated user's account.
                new[] { YouTubeService.Scope.Youtube },
                "user",
                CancellationToken.None,
                new FileDataStore(this.GetType().ToString())
            );
        }

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = this.GetType().ToString()
        });

        // Create a new, private playlist in the authorized user's channel.
        var newPlaylist = new Playlist();
        newPlaylist.Snippet = new PlaylistSnippet();
        newPlaylist.Snippet.Title = "Test Playlist";
        newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
        newPlaylist.Status = new PlaylistStatus();
        newPlaylist.Status.PrivacyStatus = "public";
        newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

        // Add a video to the newly created playlist.
        var newPlaylistItem = new PlaylistItem();
        newPlaylistItem.Snippet = new PlaylistItemSnippet();
        newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
        newPlaylistItem.Snippet.ResourceId = new ResourceId();
        newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
        newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
        newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

        Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
    }

    public virtual async Task UploadVideo()
    {
        UserCredential credential;
        using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                // This OAuth 2.0 access scope allows an application to upload files to the
                // authenticated user's YouTube channel, but doesn't allow other types of access.
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None
            );
        }

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        });

        var video = new Google.Apis.YouTube.v3.Data.Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new string[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
        var filePath = @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            await videosInsertRequest.UploadAsync();
        }
    }


    void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
    {
        switch (progress.Status)
        {
            case UploadStatus.Uploading:
                Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                break;

            case UploadStatus.Failed:
                Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                break;
        }
    }

    void videosInsertRequest_ResponseReceived(Google.Apis.YouTube.v3.Data.Video video)
    {
        Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
    }

}
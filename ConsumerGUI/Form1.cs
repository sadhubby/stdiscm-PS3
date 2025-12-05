using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Grpc.Core;
using StreamingProtos;

namespace ConsumerGUI
{
    public partial class Form1 : Form
    {
        private List<VideoItem> videos = new List<VideoItem>();
        private PictureBox currentPreviewBox;
        private AxWMPLib.AxWindowsMediaPlayer hoverPlayer;
        private Timer previewTimer;

        private Channel _channl;
        private VideoLibraryService.VideoLibraryServiceClient _libraryClient;
        private readonly string _serverAddress = "localhost:5000";
        private Timer _refreshTimer;
        public Form1()
        {
            InitializeComponent();
            InitHoverPlayer();
            InitPreviewTimer();
            InitGrpcClient();
            InitRefreshTimer();
            RefreshVideoList();
            // LoadMockVideos();
            RenderThumbnails();
        }

       private void InitGrpcClient()
       {
            _channel = new Channel(_serverAddress, ChannelCredentials.Insecure); 
            _libraryClient = new VideoLibraryService.VideoLibraryServiceClient(_channel);
        }
        private void LoadMockVideos()
        {

            /*@TODO 
            * Load videos in the video player. 
            * 
            * Currently hardcoded to have the videos prepared beforehand in the folder stdiscm-ps3 -> Consumer -> Uploads
            * Make this not be hardcoded but instead get from producer that will save it in the consumer uploads folder. 
            * 
            */
            string uploadsPath = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Consumer\Uploads")
            );

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            videos.Add(new VideoItem("RivalsAhh.mp4", Path.Combine(uploadsPath, "RivalsAhh.mp4")));
            videos.Add(new VideoItem("Taboo Gameplay 1.mp4", Path.Combine(uploadsPath, "Taboo Gameplay 1.mp4")));
            videos.Add(new VideoItem("Taboo Gameplay 3.mp4", Path.Combine(uploadsPath, "Taboo Gameplay 3.mp4")));
            videos.Add(new VideoItem("Taboo Gameplay 4.mp4", Path.Combine(uploadsPath, "Taboo Gameplay 4.mp4")));
            videos.Add(new VideoItem("Taboo Gameplay Furnace.mp4", Path.Combine(uploadsPath, "Taboo Gameplay Furnace.mp4")));
        }

        private void RenderThumbnails()
        {
            flowPanelVideos.Controls.Clear();

            foreach (var vid in videos)
            {
                var thumbnail = VideoThumbnailer.GetThumbnail(vid.FilePath);

                PictureBox pb = new PictureBox
                {
                    Width = 200,
                    Height = 120,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Image = thumbnail
                };

                pb.Tag = new ThumbnailInfo(vid.FilePath, thumbnail);

                pb.MouseHover += ThumbnailMouseHover;
                pb.MouseLeave += ThumbnailMouseLeave;
                pb.Click += ThumbnailClick;

                flowPanelVideos.Controls.Add(pb);
            }
        }

        private void ThumbnailMouseHover(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            var info = (ThumbnailInfo)pb.Tag;
            currentPreviewBox = pb;

            ShowPreview(info, pb);
            previewTimer.Start();
        }

        private void PreviewTimerTick(object sender, EventArgs e)
        {
            if (currentPreviewBox == null)
                return;

            // prevent flicker by checking real mouse position
            if (!currentPreviewBox.Bounds.Contains(PointToClient(Cursor.Position)))
            {
                StopPreview();
            }
        }

        private void ShowPreview(ThumbnailInfo info, PictureBox pb)
        {
            if (!File.Exists(info.Path))
                return;

            hoverPlayer.URL = info.Path;
            hoverPlayer.settings.autoStart = true;
            hoverPlayer.Ctlcontrols.currentPosition = 0;

            hoverPlayer.Bounds = pb.Bounds;
            hoverPlayer.Parent = pb.Parent;
            hoverPlayer.BringToFront();
            hoverPlayer.Visible = true;
        }

        private void StopPreview()
        {
            hoverPlayer.Ctlcontrols.stop();
            hoverPlayer.Visible = false;

            if (currentPreviewBox != null)
            {
                var info = (ThumbnailInfo)currentPreviewBox.Tag;
                currentPreviewBox.Image = info.Thumbnail;
            }

            previewTimer.Stop();
            currentPreviewBox = null;
        }

        private void ThumbnailMouseLeave(object sender, EventArgs e)
        {
            // do nothing muna haha
        }

        private void ThumbnailClick(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            var info = (ThumbnailInfo)pb.Tag;

            VideoPlayer.URL = info.Path;
        }

        private void InitHoverPlayer()
        {
            hoverPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            hoverPlayer.CreateControl();
            hoverPlayer.uiMode = "none";
            hoverPlayer.Visible = false;
            hoverPlayer.settings.mute = true;
            this.Controls.Add(hoverPlayer);
        }

        private void InitPreviewTimer()
        {
            previewTimer = new Timer
            {
                Interval = 100
            };
            previewTimer.Tick += PreviewTimerTick;
        }

        private void InitRefreshTimer()
        {
            _refreshTimer = new Timer
            {
                Interval = 5000 // Refresh every 5 seconds (5000 ms)
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            await RefreshVideoList();
        }

        private async Task RefreshVideoList()
        {
            try
            {
                // 1. Call the gRPC service
                var request = new ListVideosRequest();
                var response = await _libraryClient.ListVideosAsync(request);
                
                // 2. Check if the video list has changed (simple comparison by count)
                if (response.Videos.Count > videos.Count)
                {
                    // 3. Update the local list and trigger a UI refresh
                    
                    // Clear the existing list
                    videos.Clear(); 

                    // Convert gRPC VideoInfo objects to local VideoItem objects
                    foreach (var videoInfo in response.Videos)
                    {
                        // Note: FilePath should point to the actual saved file on the Consumer VM
                        // You might need to adjust how the consumer service provides the actual physical path
                        // or rely on the PlaybackUrl if it's meant to stream back from the same host.
                        videos.Add(new VideoItem(
                            videoInfo.FileName, 
                            videoInfo.PlaybackUrl // Using PlaybackUrl as the source for the WMP control
                        ));
                    }
                    
                    RenderThumbnails();
                }
            }
            catch (Exception ex)
            {
                // Handle connection errors (e.g., server is offline)
                Console.WriteLine($"Error fetching video list: {ex.Message}");
            }
        }
        private class ThumbnailInfo
        {
            public string Path { get; }
            public Image Thumbnail { get; }

            public ThumbnailInfo(string path, Image thumb)
            {
                Path = path;
                Thumbnail = thumb;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ConsumerGUI
{
    public partial class Form1 : Form
    {
        private List<VideoItem> videos = new List<VideoItem>();
        private PictureBox currentPreviewBox;
        private AxWMPLib.AxWindowsMediaPlayer hoverPlayer;

        public Form1()
        {
            InitializeComponent();
            InitHoverPlayer();
            LoadMockVideos();
            RenderThumbnails();
        }

        private void LoadMockVideos()
        {
            videos.Add(new VideoItem("RivalsAhh.mp4",
                @"C:\Users\Nitro 5\Videos\MarvelRivals\Highlights\RivalsAhh.mp4"));

            videos.Add(new VideoItem("Giganto Knull.mkv",
                @"C:\Users\Nitro 5\Videos\Giganto Knull.mkv"));
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
                    Image = thumbnail,
                    Tag = vid
                };

                // store original image!
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

            hoverPlayer.URL = info.Path;
            hoverPlayer.Ctlcontrols.currentPosition = 0;
            hoverPlayer.Ctlcontrols.play();

            hoverPlayer.Bounds = pb.Bounds;
            hoverPlayer.Parent = pb.Parent;
            hoverPlayer.BringToFront();
            hoverPlayer.Visible = true;
        }

        private void ThumbnailMouseLeave(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            var info = (ThumbnailInfo)pb.Tag;

            hoverPlayer.Ctlcontrols.stop();
            hoverPlayer.Visible = false;

            // restore original thumbnail
            pb.Image = info.Thumbnail;
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
            hoverPlayer.CreateControl();     // IMPORTANT LINE!
            hoverPlayer.uiMode = "none";
            hoverPlayer.Visible = false;
            hoverPlayer.settings.mute = true;

            this.Controls.Add(hoverPlayer);
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ConsumerGUI
{
    public partial class Form1 : Form
    {
        private List<VideoItem> videos = new List<VideoItem>();
        private Timer previewTimer;
        private PictureBox currentPreviewBox;


        public Form1()
        {
            InitializeComponent();
            LoadMockVideos();
            RenderThumbnails();
            SetupPreviewTimer();
        }

        private void SetupPreviewTimer()
        {
            previewTimer = new Timer();
            previewTimer.Interval = 500; // fake preview animation
            previewTimer.Tick += PreviewTimerTick;
        }

        private void LoadMockVideos()
        {
            videos.Add(new VideoItem("RivalsAhh.mp4", "C:\\Users\\Nitro 5\\Videos\\MarvelRivals\\Highlights\\RivalsAhh.mp4"));
            videos.Add(new VideoItem("Giganto Knull.mkv", "C:\\Users\\Nitro 5\\Videos\\Giganto Knull.mkv"));
        }

        private void RenderThumbnails()
        {
            flowPanelVideos.Controls.Clear();

            foreach (var vid in videos)
            {
                PictureBox pb = new PictureBox
                {
                    Width = 200,
                    Height = 120,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Image = Properties.Resources.default_thumb,
                    Tag = vid
                };

                pb.MouseHover += ThumbnailMouseHover;
                pb.MouseLeave += ThumbnailMouseLeave;
                pb.Click += ThumbnailClick;

                flowPanelVideos.Controls.Add(pb);
            }
        }

        private void ThumbnailMouseHover(object sender, EventArgs e)
        {
            currentPreviewBox = sender as PictureBox;
            currentPreviewBox.BorderStyle = BorderStyle.Fixed3D;
            previewTimer.Start();
        }

        private void ThumbnailMouseLeave(object sender, EventArgs e)
        {
            previewTimer.Stop();
            var pb = sender as PictureBox;
            pb.BorderStyle = BorderStyle.FixedSingle;
            pb.Image = Properties.Resources.default_thumb;
        }

        private int previewFrame = 0;

        private void PreviewTimerTick(object sender, EventArgs e)
        {
            if (currentPreviewBox == null) return;

            previewFrame++;

            // Fake animation
            currentPreviewBox.BackColor =
                (previewFrame % 2 == 0) ? Color.LightGray : Color.White;
        }

        private void ThumbnailClick(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            var video = (VideoItem)pb.Tag;

            
            VideoPlayer.URL = video.FilePath;
        }
    }
}

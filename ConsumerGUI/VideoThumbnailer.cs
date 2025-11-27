using AxWMPLib;
using System.Drawing;

public static class VideoThumbnailer
{
    public static Image GetThumbnail(string path, AxWindowsMediaPlayer player)
    {
        try
        {
            player.URL = path;
            player.Ctlcontrols.currentPosition = 1; // 1 second mark

            Bitmap bmp = new Bitmap(player.Width, player.Height);
            player.DrawToBitmap(bmp, player.ClientRectangle);

            return bmp;
        }
        catch
        {
            return ConsumerGUI.Properties.Resources.default_thumb;
        }
    }
}

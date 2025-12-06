using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerGUI
{
    public static class HttpVideoFetcher
    {
        private static readonly HttpClient _http = new HttpClient();

        public static async Task<string> DownloadToTemp(string url)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "vid_" + Guid.NewGuid() + Path.GetExtension(url));

            using (var resp = await _http.GetAsync(url))
            {
                resp.EnsureSuccessStatusCode();

                using (var fs = new FileStream(tempFile, FileMode.Create))
                {
                    await resp.Content.CopyToAsync(fs);
                }
            }

            return tempFile;
        }
    }
}

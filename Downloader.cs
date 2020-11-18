using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace pr0loader
{
    internal class Downloader
    {
        public static void GetData(List<string> urls)
        {
            Console.WriteLine("[Image Downloader]");
            Directory.CreateDirectory("Downloads"); //Crete module specific output folder
            foreach (var line in urls)
            {
                var username = Misc.GetStringBetween(line, "user/", "/");
                var fname = line.Split(new[] {$"{username}/"}, StringSplitOptions.None)[1];
                Console.SetCursorPosition(0, 1);
                Console.WriteLine($"Working on {fname}...");
                Download(line, fname, username);
            }
        }

        private static void Download(string url, string fname, string username)
        {
            Directory.CreateDirectory($"Downloads\\{fname}");
            var imageUrls = new List<string>();
            var atEnd = false;
            var lastImage = "";
            int counter = 0;            //Counts downloaded images

            #region Extracting

            var req = (HttpWebRequest) WebRequest.Create(
                "https://www.pr0gramm.com/api/items/get?flags=1&" + $"user={username}&collection={fname}");
            var res = (HttpWebResponse) req.GetResponse();
            var resp = new StreamReader(res.GetResponseStream()).ReadToEnd();
            while (resp.Contains("\"image\":"))
            {
                var tmp = Misc.GetStringBetween(resp, "\"image\":\"", "\"");
                lastImage = Misc.GetStringBetween(resp, "{\"id\":", ",");
                imageUrls.Add("https://img.pr0gramm.com/" + tmp.Replace("\\", string.Empty));
                try
                {
                    resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[3];
                }
                catch
                {
                    try
                    {
                        resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[2];
                    }
                    catch
                    {
                        resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[1];
                    }
                }
            }

            while (!atEnd)
            {
                req = (HttpWebRequest) WebRequest.Create(
                    $"https://www.pr0gramm.com/api/items/get?older={lastImage}&" +
                    $"flags=1&user={username}&collection={fname}");
                res = (HttpWebResponse) req.GetResponse();
                resp = new StreamReader(res.GetResponseStream()).ReadToEnd();
                if (resp.Contains("atEnd\":true"))
                    atEnd = true;
                while (resp.Contains("\"image\":"))
                {
                    var tmp = Misc.GetStringBetween(resp, "\"image\":\"", "\"");
                    lastImage = Misc.GetStringBetween(resp, "{\"id\":", ",");
                    Console.Title = lastImage;
                    imageUrls.Add("https://img.pr0gramm.com/" + tmp.Replace("\\", string.Empty));
                    try
                    {
                        resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[3];
                    }
                    catch
                    {
                        try
                        {
                            resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[2];
                        }
                        catch
                        {
                            resp = resp.Split(new[] {tmp}, StringSplitOptions.None)[1];
                        }
                    }
                }
            }

            #endregion

            ThreadPool.SetMaxThreads(4, 4);
            Parallel.ForEach(imageUrls, delegate(string line)
            {
                using (var client = new WebClient())
                {
                    var _ = line.Count(f => f == '/');
                    var filename = line.Split('/')[_];
                    client.DownloadFile(line, $"Downloads\\{fname}\\{filename}");
                    Console.Title = $"Downloaded: {++counter}/{imageUrls.Count}";
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace pr0loader
{
    //TODO FULLSIZE CHECK

    internal class URLSaver
    {
        public static void InsertUrl()
        {
            Console.WriteLine("Gib deine URL ein (oderer mehrere getrennt durch ein Komma)");
            var input = Console.ReadLine();
            var urls = new List<string>();
            urls.AddRange(input.Split(','));
            Console.Clear();
            GetData(urls);
        }

        public static void GetData(List<string> urls)
        {
            Console.WriteLine("[URL Saver]");
            Directory.CreateDirectory("URL Saver"); //Create Module specific output folder
            foreach (var line in urls)
            {
                var username = Misc.GetStringBetween(line, "user/", "/");
                var fname = line.Split(new[] {$"{username}/"}, StringSplitOptions.None)[1];

                Console.SetCursorPosition(0, 1);
                Console.WriteLine($"Working on: {fname}...");
                //Write file
                File.WriteAllLines($"URL Saver\\{fname}.txt", ExtractUrls(username, fname));
            }
        }


        private static List<string> ExtractUrls(string username, string fname)
        {
            var imageUrls = new List<string>();
            var atEnd = false;
            var lastImage = "";

            #region Extracting

            var req = (HttpWebRequest) WebRequest.Create(
                $"https://www.pr0gramm.com/api/items/get?flags=1&user={username}&collection={fname}");
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

            return imageUrls;
        }
    }
}
using System;
using System.Collections.Generic;

namespace pr0loader
{
    internal class Misc
    {
        public static string mode = "";

        public static void InsertUrl()
        {
            Console.WriteLine("Gib deine URL ein (oder mehrere getrennt durch ein Komma)");
            var input = Console.ReadLine();
            var urls = new List<string>();
            urls.AddRange(input.Split(','));
            Console.Clear();
            switch (mode)
            {
                case "0":
                    URLSaver.GetData(urls);
                    break;
                case "1":
                    Downloader.GetData(urls);
                    break;
                default:
                    Console.WriteLine("Invalid mode");
                    return;
            }
        }


        public static string GetStringBetween(string input, string begin, string end)
        {
            return input.Split(new[] {begin}, StringSplitOptions.None)[1]
                .Split(new[] {end}, StringSplitOptions.None)[0];
        }
    }
}
using System;
namespace pr0loader
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "pr0gramm.com Saver by nop";
            ret:
            Console.Write("0) Extract URLs\n1) Download your favorites\n2) Exit\n> ");
            Misc.mode = Console.ReadLine();

            if (Misc.mode == "2")
            {
                Environment.Exit(1);
            }

            Misc.InsertUrl();

            Console.WriteLine("DONE\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
            Main();
        }
    }
}
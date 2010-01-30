using System;

namespace Honeymoon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HoneymoonGame game = new HoneymoonGame())
            {
                game.Run();
            }
        }
    }
}


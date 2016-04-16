using System;

namespace SpacePlatformer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SpacePlatformer game = new SpacePlatformer())
            {
                game.Run();
            }
        }
    }
#endif
}


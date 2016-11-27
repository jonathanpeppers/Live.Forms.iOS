using System;
using UIKit;

namespace Live.Forms.iOS
{
    public static class Extensions
    {
        public static void Configure(this UIApplicationDelegate del, string directory)
        {
            Configuration.Instance = new LiveXaml(directory);
        }
    }
}

using System.Diagnostics;
using Xamarin.Forms;

namespace Live.Forms
{
    public static class Extensions
    {
        [Conditional("DEBUG")]
        public static void Watch(this Element element, string xamlPath)
        {
            //NOTE: library only supported on iOS
        }
    }
}

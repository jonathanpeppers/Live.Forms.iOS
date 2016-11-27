using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace Live.Forms.iOS
{
    public static class Extensions
    {
        private const string NotSupported = "Live.Forms.iOS is only supported on iOS!";

        [Conditional("DEBUG")]
        public static void Watch(this Element element)
        {
            //NOTE: library only supported on iOS
            Device.OnPlatform(null, 
                () => { throw new NotSupportedException(NotSupported); }, 
                () => { throw new NotSupportedException(NotSupported); });

            //TODO: implementation
        }
    }
}

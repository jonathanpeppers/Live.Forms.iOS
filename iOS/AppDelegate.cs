using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Live.Forms.iOS;

namespace Live.Forms.Sample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.Configure("/Users/jonathanpeppers/Desktop/Git/Live.Forms.iOS/Live.Forms.Sample/");

            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new Live.Forms.Sample.App());

            return base.FinishedLaunching(app, options);
        }
    }
}

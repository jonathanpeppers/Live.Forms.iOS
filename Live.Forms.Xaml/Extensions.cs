using System;
using Xamarin.Forms.Xaml;

namespace Live.Forms
{
    public static class Extensions
    {
        public static void LoadFromXaml(this object view, string xaml)
        {
            XamlLoader.Load(view, xaml);
        }
    }
}

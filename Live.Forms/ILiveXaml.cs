using Xamarin.Forms;

namespace Live.Forms
{
    public interface ILiveXaml
    {
        void Watch(string xamlPath, Element element);
    }
}


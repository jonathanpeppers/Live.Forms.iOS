using Xamarin.Forms;

namespace Live.Forms.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Live.Forms.iOS.Configuration.Directory = "/Users/jonathanpeppers/Desktop/Git/Live.Forms.iOS/Live.Forms.Sample/";

            MainPage = new MainPage();
        }
    }
}

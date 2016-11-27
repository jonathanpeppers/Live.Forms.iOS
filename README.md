# Live.Forms.iOS
Xamarin.Forms live XAML reloading (iOS only)

Currently this is a work in progress. The idea is simple: iOS apps can break out of the simulator and watch files on your machine. We can watch XAML files and reload when you save (live at runtime).

So for example, you have a page `MainPage.xaml.cs`:
```csharp
using Live.Forms;

class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        //Here is where the magic happens
        this.Watch();
    }
}
```

Only other setup in is your `AppDelegate.cs`
```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    this.Configure("/path/to/your/folder/");

    global::Xamarin.Forms.Forms.Init();

    LoadApplication(new Live.Forms.Sample.App());

    return base.FinishedLaunching(app, options);
}
```

From here you can modify your XAML in `MainPage.xaml` and see changes reload live.

# //TODO: 
- Next step is NuGet package setup and builds
- Need to eliminate need for `this.Configure("/path/to/your/folder/")` in iOS project, I think this will require MSBuild trickery to figure out the path to your csproj at runtime in the iOS simulator.
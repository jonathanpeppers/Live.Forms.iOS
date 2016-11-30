//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//#addin "Cake.ExtendedNuGet"

// Input args
string target = Argument("target", "Default");
string configuration = Argument("configuration", "Release");

// Define vars
var dirs = new[] 
{
    Directory("./nuget"),
    Directory("./Live.Forms/bin") + Directory(configuration),
    Directory("./Live.Forms.Fody/bin") + Directory(configuration),
    Directory("./Live.Forms.iOS/bin") + Directory(configuration),
};
string sln = "./Live.Forms.iOS.sln";
string version = "0.1.1.2";

Task("Clean")
    .Does(() =>
    {
        foreach (var dir in dirs)
            CleanDirectory(dir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(sln);
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        if(IsRunningOnWindows())
        {
            MSBuild(sln, settings =>
                settings
                    .WithProperty("Platform", new[] { "iPhoneSimulator" })
                    .SetConfiguration(configuration));
        }
        else
        {
            XBuild(sln, settings =>
                settings
                    .WithProperty("Platform", new[] { "iPhoneSimulator" })
                    .SetConfiguration(configuration));
        }
    });

Task("NuGet-Package-Live")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings   = new NuGetPackSettings
        {
            Verbosity = NuGetVerbosity.Detailed,
            Version = version,
            Files = new [] 
            {
                new NuSpecContent { Source =  dirs.Last() + File("Live.Forms.dll"), Target = "lib/portable-net45+win8+wpa81+wp8" },
                new NuSpecContent { Source =  dirs.Last() + File("Live.Forms.iOS.dll"), Target = "lib/Xamarin.iOS10" },
            },
            OutputDirectory = "./nuget"
        };
            
        NuGetPack("./Live.Forms.iOS.nuspec", settings);
    });

Task("NuGet-Package-Fody")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings   = new NuGetPackSettings
        {
            Verbosity = NuGetVerbosity.Detailed,
            Version = version,
            Files = new [] 
            {
                new NuSpecContent { Source =  dirs.Reverse().Skip(1).First() + File("Live.Forms.Fody.dll"), Target = "" },
            },
            OutputDirectory = "./nuget"
        };
            
        NuGetPack("./Live.Forms.Fody.nuspec", settings);
    });

Task("NuGet-Push")
    //.IsDependentOn("NuGet-Package-Live")
    .IsDependentOn("NuGet-Package-Fody")
    .Does(() =>
    {
        var apiKey = TransformTextFile ("./.nugetapikey").ToString();

        //NuGetPush("./nuget/Live.Forms.iOS." + version + ".nupkg", new NuGetPushSettings 
        //{
        //    Verbosity = NuGetVerbosity.Detailed,
        //    Source = "nuget.org",
        //    ApiKey = apiKey
        //});

        NuGetPush("./nuget/Live.Forms.Fody." + version + ".nupkg", new NuGetPushSettings 
        {
            Verbosity = NuGetVerbosity.Detailed,
            Source = "nuget.org",
            ApiKey = apiKey
        });
    });

Task("Default")
    .IsDependentOn("NuGet-Push");

RunTarget(target);
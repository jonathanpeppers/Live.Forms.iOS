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
    Directory("./Live.Forms.iOS/bin") + Directory(configuration),
};
string sln = "./Live.Forms.iOS.sln";
string version = "0.1.0.0";

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

Task("NuGet-Package")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings   = new NuGetPackSettings
        {
            Version = version,
            Files = new [] 
            {
                new NuSpecContent { Source =  dirs.Last() + File("Live.Forms.dll"), Target = "lib/netstandard1.1" },
                new NuSpecContent { Source =  dirs.Last() + File("Live.Forms.iOS.dll"), Target = "lib/xamarinios" },
            },
            OutputDirectory = "./nuget"
        };
            
        NuGetPack("./Live.Forms.iOS.nuspec", settings);
    });

Task("Default")
    .IsDependentOn("NuGet-Package");

RunTarget(target);
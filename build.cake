//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

string target = Argument("target", "Default");
string configuration = Argument("configuration", "Release");

// Define directories.
var dirs = new[] 
{
    Directory("./Live.Forms/bin") + Directory(configuration),
    Directory("./Live.Forms.iOS/bin") + Directory(configuration),
};
string sln = "./Live.Forms.iOS.sln";

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
      // Use MSBuild
      MSBuild(sln, settings =>
        settings
            .WithProperty("Platform", new[] { "iPhoneSimulator" })
            .SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild(sln, settings =>
        settings
            .WithProperty("Platform", new[] { "iPhoneSimulator" })
            .SetConfiguration(configuration));
    }
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
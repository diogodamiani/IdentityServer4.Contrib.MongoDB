var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var isWindows           = IsRunningOnWindows();
var netcore             = "netcoreapp1.1";
var netstandard         = "netstandard1.5";

var libIS4MongoPath     = Directory("./src/IdentityServer4.MongoDB");
var demoHostPath		= Directory("./src/Host");
var buildArtifacts      = Directory("./artifacts/packages");

///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration
    };
	
	var projects = GetFiles("./**/*.csproj");
	
	// main build (Windows local and Appveyor)
    // build for all targets
    if (isWindows)
    {
		foreach(var project in projects)
		{
			DotNetCoreBuild(project.GetDirectory().FullPath, settings);
		}
    }
    // local mac / travis
    // don't build for .net framework
    else
    {
        settings.Framework = netstandard;
        DotNetCoreBuild(libIS4MongoPath, settings);
        
        settings.Framework = netcore;
        DotNetCoreBuild(demoHostPath, settings);
	}
});

///////////////////////////////////////////////////////////////////////////////
//Task("Test")
//    .IsDependentOn("Restore")
//    .IsDependentOn("Clean")
//    .Does(() =>
//{
//    var settings = new DotNetCoreTestSettings
//    {
//        Configuration = configuration
//    };
//
//    if (!isWindows)
//    {
//        Information("Not running on Windows - skipping tests for full .NET Framework");
//        settings.Framework = netcore;
//    }
//
//    var projects = GetFiles("./test/**/*.csproj");
//    foreach(var project in projects)
//    {
//        DotNetCoreTest(project.FullPath, settings);
//    }
//});

///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
	if (!isWindows)
    {
        Information("Not running on Windows - skipping pack");
        return;
    }
	
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
    };

    // add build suffix for CI builds
    if(isAppVeyor)
    {
        settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
    }

    DotNetCorePack(libIS4MongoPath, settings);
});

///////////////////////////////////////////////////////////////////////////////
Task("Default")
  .IsDependentOn("Build")
  //.IsDependentOn("Test")
  .IsDependentOn("Pack");

RunTarget(target);
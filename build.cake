#r tools/JAVS.Cake/lib/netstandard2.0/JAVS.Cake.dll

var target = Argument("target", "Build-Solution");
var config = Argument("config", "Release");
var fullVersion = Argument("fullVersion", "1.0.0.0");

void CheckTeamCity()
{
    if (TeamCity.IsRunningOnTeamCity)
    {
        TaskSetup(x =>
        {
            TeamCity.WriteStartBuildBlock(x.Task.Name);
            TeamCity.WriteStartProgress(x.Task.Name);
        });

        TaskTeardown(x =>
        {
            TeamCity.WriteEndProgress(x.Task.Name);
            TeamCity.WriteEndBuildBlock(x.Task.Name);
        });
    }
}

CheckTeamCity();

Task("Update-Version")
    .Does(() =>
    {
        foreach (var csproj in GetFiles("**/*.csproj"))
        {
            Information($"Updating {csproj} to {fullVersion}...");
            RegexReplaceInFile(csproj.ToString(), "(<Version>)([0-9\\.]+)(</Version>)", $"${{1}}{fullVersion}$3");
        }
    });

Task("Build-Solution")
    .Does(() => DotNetCoreBuild("DirectShowLib", new DotNetCoreBuildSettings()
        {
            Configuration = config
        }));

RunTarget(target);

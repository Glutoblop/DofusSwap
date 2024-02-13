using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;

namespace Releaser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TODO
            // - Find out the current version
            // - Increment it by build +1
            // - Change the latest_version.txt file to new version
            // - Change the AssemblyVersion version to the same new version
            // - Change ReadMe download link to point to the new version
            // - Build the project in Release.
            // - Zip up the DofusSwap.exe and name it {new version}.zip
            // - Move this zip into Downloadables folder
            // - git commit and push latest_version, readme and .zip file with commit message Version {new version} and tag this commit with {new version}

            // --- Find current version
            var latestVersionStr = File.ReadAllText("../latest_version.txt");
            if (!Version.TryParse(latestVersionStr, out Version latestVersion))
            {
                Console.WriteLine($"latest_version.txt not found");
                return;
            }

            // --- Make new version
            Version newVersion = new Version(latestVersion.Major, latestVersion.Minor, latestVersion.Build + 1);
            

            // --- Replace the new version in these files
            string[] paths = 
            {
                "../latest_version.txt",
                "../Properties/AssemblyInfo.cs",
                "../readme.md"
            };

            foreach (var path in paths)
            {
                var assemblyText = File.ReadAllText(path);
                assemblyText = assemblyText.Replace(latestVersionStr, newVersion.ToString());
                File.WriteAllText(path, assemblyText);   
            }

            // --- Build DofusSwap in Release mode
            //Because its an old winforms format, i can't use dotnet command and had to configure MSBuild.exe to the environment Path variables instead.

            var buildProcess = new Process();
            buildProcess.StartInfo =
                new ProcessStartInfo("CMD.exe")
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Arguments = "/C msbuild ..\\.\\DofusSwap.csproj -property:Configuration=Release"
                };
            buildProcess.Start();
            buildProcess.WaitForExit();

            Console.WriteLine("DofusSwap Built");



            // --- Zip up DofusSwap.exe into dofusswap_{version}.zip and put into Downloadables folder

            //Delete everything in the bin/Release folder apart from exe
            var exeDirectory = $"{Directory.GetCurrentDirectory()}\\..\\bin\\Release";

            var releaseFolder = new DirectoryInfo(exeDirectory);
            var releaseFiles = releaseFolder.GetFiles().ToList();

            for (var i = releaseFiles.Count - 1; i >= 0; i--)
            {
                var releaseFile = releaseFiles[i];
                if (releaseFile.Name == "DofusSwap.exe") continue;
                File.Delete(releaseFile.FullName);
                releaseFiles.RemoveAt(i);
            }

            Console.WriteLine("bin/Release prepared for zip");

            var downloadablesDirectory = $"{Directory.GetCurrentDirectory()}\\..\\Downloadables\\dofusswap_{newVersion}.zip";
            ZipFile.CreateFromDirectory(exeDirectory, downloadablesDirectory);

            Console.WriteLine("zip file created");


            // --- Commit and push this new version to master


        }
    }
}

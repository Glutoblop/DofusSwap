using DofusSwap.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwap.Updater
{
    public class CmdUpdater
    {
        // --- TODO:
        // * Check if there is a newer version than this one
        // * Download it into Downloads folder
        // * Unzip it
        // * Open a command line that will:
        //     * Delete the old exe
        //     * Move the new exe into the old exe's folder
        //     * Start the new exe
        // * Update complete

        public async Task CheckForUpdate()
        {
            try
            {
                var currentVersionStr = GetVersionString();
                string latestVersionStr = "";

                var versionUrl = String.Format(Resources.update_url, currentVersionStr);

                //Find the latest version available
                var client = new WebClient();
                latestVersionStr = await client.DownloadStringTaskAsync(versionUrl);

                if (!Version.TryParse(currentVersionStr, out Version currentVersion)) return;
                if (!Version.TryParse(latestVersionStr, out Version latestVersion)) return;

                //if (currentVersion >= latestVersion) return;

                //Request user if they want new version
                var dialogResult = MessageBox.Show($"Update {latestVersion} available, do you want to install it?", "DofusSwap",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                
                //If no, do nothing 
                if (dialogResult != DialogResult.Yes) return;

                //Download the new version
                var downloadUrl = String.Format(Resources.download_url, latestVersionStr);
                var zipData = await client.DownloadDataTaskAsync(downloadUrl);

                //Unzip this into the Download folder.
                var zip_path = String.Format(Resources.zip_download_path, GetDownloadFolderPath(), latestVersionStr);

                var zip_file_path = $"{zip_path}.zip";
                if (File.Exists(zip_file_path)) File.Delete(zip_file_path);
                File.WriteAllBytes(zip_file_path, zipData);

                if (Directory.Exists(zip_path))
                {
                    RecursiveDelete(new DirectoryInfo(zip_path));
                }

                ZipFile.ExtractToDirectory(zip_file_path, zip_path);

                //var downloadFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Temp\\";
                var exeName = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.exe";

                //THIS DOESNT WORK BECAUSE IT ALWAYS CREATES THE CMD AS A CHILD PROCESS
                //WHEN THE EXE IS KILLED, SO IS THE CMD

                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var exeFolderPath = Path.GetDirectoryName(exePath);
                var command = "call start /k cmd /c echo \"Starting update..\" && "
                              + "ping 127.0.0.1 -n 2 > nul && "
                              + $"taskkill /f /t /im {exeName} && "
                              + "ping 127.0.0.1 -n 2 > nul && echo \"Done!\" && "
                              + $"del {exeFolderPath}\\{exeName} && "
                              + "ping 127.0.0.1 -n 2 > nul && echo \"Done!\" && "
                              + $"xcopy /F /h /i /c /k /e /r /y \"{zip_path}\\{exeName}\" \"{exeFolderPath}\\{exeName}*\" && " 
                              + "ping 127.0.0.1 -n 2 > nul && echo \"Done!\" && "
                              + $"start {exeFolderPath}\\{exeName}\"";

                Process.Start(new ProcessStartInfo()
                {
                    Arguments = $"{command}",
                    FileName = "CMD.exe",
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Download failed", "DofusSwap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private string GetVersionString()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        string GetDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }

        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            baseDir.Delete(true);
        }
    }
}

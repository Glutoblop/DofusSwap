using DofusSwap.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace DofusSwap.Updater
{
    public class Updater
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

        private async void CheckForUpdate()
        {
            try
            {
                var currentVersionStr = GetVersionString();
                string latestVersionStr = "";

                var versionUrl = String.Format(Resources.update_url, currentVersionStr);

                var client = new WebClient();
                latestVersionStr = await client.DownloadStringTaskAsync(versionUrl);

                if (!Version.TryParse(currentVersionStr, out Version currentVersion)) return;
                if (!Version.TryParse(latestVersionStr, out Version latestVersion)) return;

                //if (currentVersion >= latestVersion) return;

                var dialogResult = MessageBox.Show($"Update {latestVersion} available, do you want to install it?", "DofusSwap",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (dialogResult == DialogResult.Yes)
                {
                    var downloadUrl = String.Format(Resources.download_url, latestVersionStr);
                    var zipData = await client.DownloadDataTaskAsync(downloadUrl);

                    var zip_path = String.Format(Resources.zip_download_path, GetDownloadFolderPath(), latestVersionStr);

                    var zip_file_path = $"{zip_path}.zip";
                    if (File.Exists(zip_file_path)) File.Delete(zip_file_path);
                    File.WriteAllBytes(zip_file_path, zipData);

                    if (Directory.Exists(zip_path))
                    {
                        RecursiveDelete(new DirectoryInfo(zip_path));
                    }

                    ZipFile.ExtractToDirectory(zip_file_path, zip_path);

                    //Auto run the setup
                    Process.Start($"{zip_path}\\SetupDofusSwap.exe");

                    //Open explorer to the file
                    //Process.Start("explorer.exe",$"{zip_path}");

                }
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

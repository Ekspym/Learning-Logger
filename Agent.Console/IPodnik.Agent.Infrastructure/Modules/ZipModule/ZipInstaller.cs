using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;
using Microsoft.Win32;
using Serilog;
using System.Diagnostics;
using System.Net;

namespace IPodnik.Agent.Infrastructure.Modules.ZipModule
{
    public class ZipInstaller : BaseModule
    {
        public ZipInstaller(IAgentTaskProgressQueue progressQueue)
                 : base(progressQueue)
        {
        }

        private bool IsZipInstalled()
        {
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string sevenZipPath = Path.Combine(programFilesPath, "7-Zip", "7z.exe");

            if (File.Exists(sevenZipPath))
            {
                return true;
            }

            string registryKey = Constants.ZipInstaller.INSTALLER_REGISTRY;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        string displayName = subkey.GetValue("DisplayName") as string;
                        if (displayName != null && displayName.Contains("7-Zip"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void InstallZip(AgentJobDto job)
        {
            string installerUrl = Constants.ZipInstaller.INSTALLER_PATH;
            string installerPath = Path.Combine(Path.GetTempPath(), "7z_installer.exe");

            LogProgress(job, "Downloading 7-Zip installer...", 20);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(installerUrl, installerPath);
            }

            LogProgress(job, "Running installer...", 50);

            Process installer = new Process
            {
                StartInfo =
                {
                    FileName = installerPath,
                    Arguments = "/S",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                installer.Start();
                installer.WaitForExit();

                LogProgress(job, "Installer finished.", 90);
                Log.Information("Installation of 7zip was successful");
            }
            catch (Exception ex)
            {
                LogProgress(job, "Installation failed.");
                Log.Error("Installation of 7zip failed", ex);
            }
            finally
            {
                if (File.Exists(installerPath))
                    File.Delete(installerPath);
            }

            LogProgress(job, "Process complete.", 100);
        }

        public ModuleReportDto TryInstallZip(AgentJobDto job)
        {
            LogProgress(job, "Checking if 7-Zip is already installed...", 0);

            if (!IsZipInstalled())
            {
                LogProgress(job, "7-Zip not found. Beginning installation...", 10);
                InstallZip(job);
                return new ModuleReportDto { Success = true };
            }
            else
            {
                LogProgress(job, "7-Zip already installed.", 100);
                return new ModuleReportDto { Success = false, Error = "Zip already installed" };
            }
        }
    }
}

using MetroFramework.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Reflection;

namespace FastGPU_P
{
    public partial class Form1 : MetroForm
    {
        private readonly bool _isWin10;

        public Form1()
        {
            var os = Environment.OSVersion;
            _isWin10 = os.Version.Major == 10 && os.Version.Build < 22000;
            
            InitializeComponent();
            Resizable = false;
            MaximizeBox = false;
            Shown += Form1_Shown;
        }

        // OS & Hardware
        
        private static string GetWindowsEdition()
        {
            using ManagementObjectSearcher searcher = new("SELECT Caption FROM Win32_OperatingSystem");
            using var results = searcher.Get();

            foreach (ManagementObject obj in results.Cast<ManagementObject>())
            {
                using (obj)
                {
                    return obj["Caption"]?.ToString() ?? "Unknown";
                }
            }
            return "Unknown";
        }

        private static bool IsWindowsCompatible()
        {
            int buildNumber = Environment.OSVersion.Version.Build;
            string edition = GetWindowsEdition();
            Debug.WriteLine($"Running {edition}");
            
            if (buildNumber >= 19041 && !edition.Contains("Server"))
                return true;
                
            if (buildNumber >= 20348 && edition.Contains("Server"))
                return true;
                
            return false;
        }

        private static string? GetGpuVram(string gpuName)
        {
            string baseRegistryKeyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
            using RegistryKey? baseKey = Registry.LocalMachine.OpenSubKey(baseRegistryKeyPath);

            if (baseKey == null) return null;

            foreach (string subKeyName in baseKey.GetSubKeyNames())
            {
                string subKeyPath = $@"{baseRegistryKeyPath}\{subKeyName}";
                string? foundName = GetValueFromRegistry(subKeyPath, "DriverDesc");
                
                if (foundName == gpuName)
                {
                    return GetValueFromRegistry(subKeyPath, "HardwareInformation.qwMemorySize");
                }
            }
            
            return null;
        }

        private static string? GetValueFromRegistry(string registryKeyPath, string valueName)
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(registryKeyPath);
            if (key != null)
            {
                object? value = key.GetValue(valueName);
                if (value != null)
                {
                    return value.ToString();
                }
            }
            return null;
        }

        private static bool IsFeatureEnabled(string featureName)
        {
            string query = $"SELECT * FROM Win32_OptionalFeature WHERE Name = '{featureName}'";
            using ManagementObjectSearcher searcher = new(query);
            using var results = searcher.Get();

            foreach (ManagementObject obj in results.Cast<ManagementObject>())
            {
                using (obj)
                {
                    int installState = Convert.ToInt32(obj["InstallState"]);
                    return installState == 1;
                }
            }
            return false; 
        }

        private static void ApplyPreventiveFixes()
        {
            try
            {
                using RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\HyperV", writable: true);
                if (key != null)
                {
                    key.SetValue("RequireSecureDeviceAssignment", 0, RegistryValueKind.DWord);
                    key.SetValue("RequireSupportedDeviceAssignment", 0, RegistryValueKind.DWord);
                }
                Debug.WriteLine("Preventive Fixes applied");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to apply preventive fixes: {ex.Message}");
            }
        }

        // PowerShell
        
        private static List<string> ExecutePowerShell(string scriptContent, Dictionary<string, object>? parameters = null)
        {
            string tempScript = Path.GetTempFileName() + ".ps1";
            
            File.WriteAllText(tempScript, scriptContent);

            string args = $"-ExecutionPolicy Bypass -NoProfile -File \"{tempScript}\"";
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    args += $" -{param.Key} \"{param.Value}\"";
                }
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

			using var process = Process.Start(processInfo) ?? throw new Exception("Failed to start PowerShell");
			string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            try { File.Delete(tempScript); } catch { /* Ignore cleanup errors */ }

            if (process.ExitCode != 0)
            {
                string errorMessage = string.IsNullOrWhiteSpace(errors) ? "An unknown PowerShell error occurred." : errors;
                Debug.WriteLine($"PowerShell Error:\n{errorMessage}");
                throw new InvalidOperationException(errorMessage);
            }

            return [.. output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim())];
        }

        private static List<string> RunEmbeddedScript(string scriptName, Dictionary<string, object>? parameters = null)
        {
            string resourcePath = $"FastGPU_P.Scripts.{scriptName}";

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Could not find embedded resource: {resourcePath}");

            using StreamReader reader = new(stream);
            string scriptContent = reader.ReadToEnd();

            return ExecutePowerShell(scriptContent, parameters);
        }

        // UI

        private async void Form1_Shown(object? sender, EventArgs e)
        {
            ToggleActionButtons(false);

            var loadingTask = Task.Run(() =>
            {
                List<string> detectedGpus = [];
                int maxVramIndex = 0;
                double maxVram = 0.0;

                if (!_isWin10)
                {
                    using var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                    using var results = searcher.Get();
                    int index = 0;
                    const double gbDivisor = 1024.0 * 1024.0 * 1024.0;

                    foreach (ManagementObject obj in results.Cast<ManagementObject>())
                    {
                        using (obj)
                        {
                            string gpuName = (string)obj["Name"];
                            detectedGpus.Add(gpuName);

                            double vram = Convert.ToDouble(GetGpuVram(gpuName));
                            if (vram > maxVram)
                            {
                                maxVram = vram;
                                maxVramIndex = index;
                            }
                            index++;

                            string vramGb = (vram / gbDivisor).ToString("0.##");
                            Debug.WriteLine($"{gpuName}: {vramGb}GB");
                        }
                    }
                }

                List<string> detectedVms = ExecutePowerShell("Get-VM | Where-Object Generation -GT 1 | Select-Object -ExpandProperty Name");

                bool isCompatible = IsWindowsCompatible();
                bool isHyperVEnabled = IsFeatureEnabled("Microsoft-Hyper-V-All");
                bool isWslEnabled = IsFeatureEnabled("Microsoft-Windows-Subsystem-Linux");

                return (detectedGpus, maxVramIndex, detectedVms, isCompatible, isHyperVEnabled, isWslEnabled);
            });

            try
            {
                var (gpus, bestGpuIndex, vms, isWinCompatible, isHyperVActive, isWslActive) = await loadingTask;

                if (_isWin10)
                {
                    gpuBox.Items.Clear();
                    gpuBox.Items.Add("Auto");
                    gpuBox.SelectedIndex = 0;
                    gpuBox.Enabled = false;
                }
                else
                {
                    foreach (string gpu in gpus)
                    {
                        gpuBox.Items.Add(gpu);
                    }
                    if (gpuBox.Items.Count > 0)
                    {
                        gpuBox.SelectedIndex = bestGpuIndex;
                    }
                }

                if (gpuBox.Items.Count == 1)
                {
                    gpuBox.Enabled = false;
                }

                foreach (string vm in vms)
                {
                    vmBox.Items.Add(vm);
                }

                if (vmBox.Items.Count == 0)
                {
                    MessageBox.Show("No VMs available!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
                else if (vmBox.Items.Count == 1)
                {
                    vmBox.SelectedIndex = 0;
                    vmBox.Enabled = false;
                }

                if (!isWinCompatible)
                {
                    DialogResult result = MessageBox.Show(
                        "This application is optimized for Windows 10 20H1 and later..\n\n" +
                        "Running on an unsupported Windows version may cause issues.\n\n" +
                        "Do you want to continue anyway?",
                        "Compatibility Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        Application.Exit();
                        return;
                    }
                }

                if (!isHyperVActive)
                {
                    DialogResult result = MessageBox.Show(
                        "Hyper-V feature is disabled. GPU-P requires Hyper-V to be enabled.\n\n" +
                        "Do you want to continue anyway?",
                        "Hyper-V Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        Application.Exit();
                        return;
                    }
                }

                if (isWslActive)
                {
                    Debug.WriteLine("WSL is enabled, this could cause compatibility issues!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleActionButtons(true);
            }
        }

        private void ToggleActionButtons(bool isEnabled)
        {
            addButton.Enabled = isEnabled;
            installDriverBtn.Enabled = isEnabled;
            removeButton.Enabled = isEnabled;
            UseWaitCursor = !isEnabled;
        }

        private void AllocationBar_ValueChanged(object sender, EventArgs e)
        {
            int snappedValue = (int)(Math.Round(allocationBar.Value / 5.0) * 5);

            if (allocationBar.Value != snappedValue)
            {
                allocationBar.Value = snappedValue;
            }

            allocPercent.Text = $"{snappedValue}%";
        }

        // Helpers

        private static string GetGpuInstance(string targetGpu)
        {
            var scriptParams = new Dictionary<string, object>
            {
                { "GPUName", targetGpu }
            };

            var results = RunEmbeddedScript("GetGPUInstance.ps1", scriptParams);
            return results.FirstOrDefault()?.ToString() ?? throw new Exception("Instance ID not found");
        }

        private static void ShutdownVm(string targetVm)
        {
            var scriptParams = new Dictionary<string, object>
            {
                { "VMName", targetVm }
            };

            RunEmbeddedScript("ShutdownVM.ps1", scriptParams);
        }

        private static string InstallDriverCore(string targetVm, string targetGpu, string hostName)
        {
            ShutdownVm(targetVm);

            var scriptParams = new Dictionary<string, object>
            {
                { "VMName", targetVm },
                { "GPUName", targetGpu },
                { "Hostname", hostName }
            };

            var output = RunEmbeddedScript("InstallDriver.ps1", scriptParams);

            if (output.Any(line => line.Contains("SKIP_DRIVER_UPDATE")))
            {
                return "The VM has the exact same GPU driver version as the host. Update Skipped!";
            }

            return "GPU drivers updated successfully!";
        }

        // Button Click Events

        private async void AddButton_Click(object sender, EventArgs e)
        {
            ToggleActionButtons(false);
            
            string targetVm = vmBox.Text;
            string targetGpu = gpuBox.Text;
            int gpuCount = gpuBox.Items.Count;
            int allocationValue = allocationBar.Value;
            string hostName = Environment.MachineName;

            try
            {
                string driverStatus = "";

                await Task.Run(() =>
                {
                    var stateCheck = ExecutePowerShell($"(Get-VM -Name \"{targetVm}\").State");
                    bool wasRunning = stateCheck.FirstOrDefault()?.ToString() == "Running";

                    ShutdownVm(targetVm);
                    ApplyPreventiveFixes();

                    string instancePath = string.Empty;
                    if (gpuCount > 1 && !_isWin10)
                    {
                        instancePath = GetGpuInstance(targetGpu);
                    }

                    var scriptParams = new Dictionary<string, object>
                    {
                        { "VMName", targetVm },
                        { "InstancePath", instancePath },
                        { "GPUResourceAllocationPercentage", allocationValue }
                    };
                    
                    RunEmbeddedScript("AllocateGPU.ps1", scriptParams);
                    
                    driverStatus = InstallDriverCore(targetVm, targetGpu, hostName);

                    if (wasRunning)
                    {
                        ExecutePowerShell($"Start-VM -Name \"{targetVm}\"");
                    }
                });

                MessageBox.Show($"GPU partition assigned!\n\n{driverStatus}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Allocation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleActionButtons(true);
            }
        }

        private async void InstallDriverBtn_Click(object sender, EventArgs e)
        {
            ToggleActionButtons(false);
            
            string targetVm = vmBox.Text;
            string targetGpu = gpuBox.Text;
            string hostName = Environment.MachineName;

            try
            {
                string driverStatus = "";
                
                await Task.Run(() =>
                {
                    driverStatus = InstallDriverCore(targetVm, targetGpu, hostName);
                });

                MessageBox.Show(driverStatus, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Driver Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleActionButtons(true);
            }
        }

        private async void RemoveButton_Click(object sender, EventArgs e)
        {
            ToggleActionButtons(false);
            string targetVm = vmBox.Text;

            try
            {
                await Task.Run(() =>
                {
                    var stateCheck = ExecutePowerShell($"(Get-VM -Name \"{targetVm}\").State");
                    bool wasRunning = stateCheck.FirstOrDefault()?.ToString() == "Running";

                    ShutdownVm(targetVm);
                    
                    var scriptParams = new Dictionary<string, object>
                    {
                        { "VMName", targetVm }
                    };
                    RunEmbeddedScript("RemoveGPU.ps1", scriptParams);

                    if (wasRunning)
                    {
                        ExecutePowerShell($"Start-VM -Name \"{targetVm}\"");
                    }
                });

                MessageBox.Show("Successfully cleared all GPU Partitions from VM", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleActionButtons(true);
            }
        }
    }
}
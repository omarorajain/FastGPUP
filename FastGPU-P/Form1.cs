using MetroFramework.Forms;
using System.Diagnostics;
using System.Management;

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
                List<string> detectedAdapters = ExecutePowerShell("Get-NetAdapter -Physical | Where-Object Status -eq 'Up' | Select-Object -ExpandProperty Name");

                bool isCompatible = IsWindowsCompatible();
                bool isHyperVEnabled = IsFeatureEnabled("Microsoft-Hyper-V-All");
                bool isWslEnabled = IsFeatureEnabled("Microsoft-Windows-Subsystem-Linux");

                return (detectedGpus, maxVramIndex, detectedVms, detectedAdapters, isCompatible, isHyperVEnabled, isWslEnabled);
            });

            try
            {
                var (gpus, bestGpuIndex, vms, adapters, isWinCompatible, isHyperVActive, isWslActive) = await loadingTask;

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

                foreach (string adapter in adapters)
                {
                    networkAdapterBox.Items.Add(adapter);
                }
                if (networkAdapterBox.Items.Count > 0) networkAdapterBox.SelectedIndex = 0;

                LoadSavedIPs();
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
            allocateGpuButton.Enabled = isEnabled;
            removeGpuButton.Enabled = isEnabled;
            installDriverButton.Enabled = isEnabled;
            addIpButton.Enabled = isEnabled;
            removeIpButton.Enabled = isEnabled;
            applyNetworkButton.Enabled = isEnabled;

            Cursor = isEnabled ? Cursors.Default : Cursors.AppStarting;
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
    }
}

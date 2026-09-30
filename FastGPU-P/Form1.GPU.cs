using Microsoft.Win32;

namespace FastGPU_P
{
	public partial class Form1
	{

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

		private static string GetGpuInstance(string targetGpu)
		{
			var scriptParams = new Dictionary<string, object>
			{
				{ "GPUName", targetGpu }
			};

			var results = RunEmbeddedScript("GetGPUInstance.ps1", scriptParams);
			return results.FirstOrDefault()?.ToString() ?? throw new Exception("Instance ID not found");
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
		private async void AllocateGpuButton_Click(object sender, EventArgs e)
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

		private async void InstallDriverButton_Click(object sender, EventArgs e)
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

		private async void RemoveGpuButton_Click(object sender, EventArgs e)
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

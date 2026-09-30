using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace FastGPU_P
{
	public partial class Form1
	{
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

		private static void ShutdownVm(string targetVm)
		{
			var scriptParams = new Dictionary<string, object>
			{
				{ "VMName", targetVm }
			};

			RunEmbeddedScript("ShutdownVM.ps1", scriptParams);
		}
	}
}

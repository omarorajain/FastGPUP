using System.Diagnostics;
using System.Reflection;

namespace FastGPU_P
{
	public partial class Form1
	{
		private static List<string> ExecutePowerShell(string scriptContent, Dictionary<string, object>? parameters = null)
		{
			string tempScript = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".ps1");

			File.WriteAllText(tempScript, scriptContent);

			string args = $"-ExecutionPolicy Bypass -NoProfile -NonInteractive -File \"{tempScript}\"";
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
	}
}

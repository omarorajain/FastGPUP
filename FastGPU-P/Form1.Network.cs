using System.Text.Json;

namespace FastGPU_P
{
	public class ClientIP
	{
		public string IP { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public bool IsEnabled { get; set; } = true;

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(Description) ? IP : $"{IP} ({Description})";
		}
	}

	public partial class Form1
	{
		private readonly string ipSaveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FastGPUP_IPs.json");

		private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

		private void LoadSavedIPs()
		{
			if (File.Exists(ipSaveFile))
			{
				try
				{
					string json = File.ReadAllText(ipSaveFile);
					var clients = JsonSerializer.Deserialize<List<ClientIP>>(json);
					if (clients != null)
					{
						foreach (var client in clients)
						{
							ipListBox.Items.Add(client, client.IsEnabled);
						}
					}
				}
				catch { /* Ignore corrupt file */ }
			}

			if (ipListBox.Items.Count == 0)
			{
				ipListBox.Items.Add(new ClientIP { IP = "192.168.0.80", Description = "Default", IsEnabled = true }, true);
			}
		}

		private void SaveIPs()
		{
			var clientsToSave = new List<ClientIP>();

			for (int i = 0; i < ipListBox.Items.Count; i++)
			{
				var client = (ClientIP)ipListBox.Items[i];
				client.IsEnabled = ipListBox.GetItemChecked(i);
				clientsToSave.Add(client);
			}

			string json = JsonSerializer.Serialize(clientsToSave, _jsonOptions);
			File.WriteAllText(ipSaveFile, json);
		}

		private static ClientIP? PromptForClientIP()
		{
			using Form prompt = new()
			{
				Width = 350,
				Height = 220,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				Text = "Add Whitelist IP",
				StartPosition = FormStartPosition.CenterParent,
				MaximizeBox = false
			};

			Label ipLabel = new() { Left = 20, Top = 20, Text = "Local IP (e.g., 192.168.0.80):", AutoSize = true };
			TextBox ipBox = new() { Left = 20, Top = 45, Width = 290 };

			Label descLabel = new() { Left = 20, Top = 80, Text = "Description (e.g., Living Room TV):", AutoSize = true };
			TextBox descBox = new() { Left = 20, Top = 105, Width = 290 };

			Button confirmation = new() { Text = "Add", Left = 210, Width = 100, Top = 140, DialogResult = DialogResult.OK };

			prompt.Controls.Add(ipLabel); prompt.Controls.Add(ipBox);
			prompt.Controls.Add(descLabel); prompt.Controls.Add(descBox);
			prompt.Controls.Add(confirmation);
			prompt.AcceptButton = confirmation;

			if (prompt.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(ipBox.Text))
			{
				return new ClientIP
				{
					IP = ipBox.Text.Trim(),
					Description = descBox.Text.Trim(),
					IsEnabled = true
				};
			}
			return null;
		}

		private bool _isManualToggle = false;
		private void IpListBox_MouseDown(object sender, MouseEventArgs e)
		{
			int index = ipListBox.IndexFromPoint(e.Location);

			if (index == -1)
			{
				ipListBox.ClearSelected();
			}
			else if (e.X < 20)
			{
				_isManualToggle = true;

				bool currentState = ipListBox.GetItemChecked(index);
				ipListBox.SetItemChecked(index, !currentState);

				_isManualToggle = false;
			}
		}

		private void IpListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (!_isManualToggle)
			{
				e.NewValue = e.CurrentValue;
			}
		}

		private void AddIpButton_Click(object sender, EventArgs e)
		{
			var newClient = PromptForClientIP();
			if (newClient != null)
			{
				bool exists = ipListBox.Items.Cast<ClientIP>().Any(c => c.IP == newClient.IP);
				if (!exists)
				{
					ipListBox.Items.Add(newClient, true);
					SaveIPs();
				}
			}
		}

		private void RemoveIpButton_Click(object sender, EventArgs e)
		{
			if (ipListBox.SelectedIndex != -1)
			{
				ipListBox.Items.RemoveAt(ipListBox.SelectedIndex);
				SaveIPs();
			}
		}

		private async void ApplyNetworkButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(vmBox.Text) || string.IsNullOrEmpty(networkAdapterBox.Text))
			{
				MessageBox.Show("Please select both a VM and a Network Adapter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			ToggleActionButtons(false);
			applyNetworkButton.Text = "Applying... Please wait";

			string targetVm = vmBox.Text;
			string selectedAdapter = networkAdapterBox.Text;

			var enabledIps = ipListBox.CheckedItems.Cast<ClientIP>()
				.Select(c => c.IP.Contains('/') ? c.IP : $"{c.IP}/32")
				.ToArray();

			string ipListString = string.Join(",", enabledIps);

			try
			{
				await Task.Run(() =>
				{
					var scriptParams = new Dictionary<string, object>
					{
						{ "VMName", targetVm },
						{ "PhysicalAdapter", selectedAdapter },
						{ "WhitelistIPs", ipListString }
					};

					RunEmbeddedScript("SetupNetwork.ps1", scriptParams);
				});

				SaveIPs();

				MessageBox.Show($"Successfully bound to {selectedAdapter} and applied firewall!", "Network Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Network Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				ToggleActionButtons(true);
				applyNetworkButton.Text = "Apply Network Config";
			}
		}
	}
}

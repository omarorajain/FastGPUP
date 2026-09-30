namespace FastGPU_P
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            mainTabControl = new MetroFramework.Controls.MetroTabControl();
            gpuTab = new MetroFramework.Controls.MetroTabPage();
            vmLabel = new Label();
            vmBox = new ComboBox();
            gpuLabel = new Label();
            gpuBox = new ComboBox();
            allocationLabel = new Label();
            allocationBar = new MetroFramework.Controls.MetroTrackBar();
            allocPercent = new Label();
            allocateGpuButton = new Button();
            removeGpuButton = new Button();
            installDriverButton = new Button();
            networkTab = new MetroFramework.Controls.MetroTabPage();
            networkAdapterLabel = new Label();
            networkAdapterBox = new ComboBox();
            ipListLabel = new Label();
            ipListBox = new CheckedListBox();
            ipListBox.CheckOnClick = false;
            addIpButton = new Button();
            removeIpButton = new Button();
            applyNetworkButton = new Button();
            creditLabel = new Label();
            mainTabControl.SuspendLayout();
            gpuTab.SuspendLayout();
            networkTab.SuspendLayout();
            SuspendLayout();
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(gpuTab);
            mainTabControl.Controls.Add(networkTab);
            mainTabControl.CustomBackground = false;
            mainTabControl.FontSize = MetroFramework.MetroTabControlSize.Medium;
            mainTabControl.FontWeight = MetroFramework.MetroTabControlWeight.Light;
            mainTabControl.Location = new Point(20, 65);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.Padding = new Point(6, 8);
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(360, 420);
            mainTabControl.Style = MetroFramework.MetroColorStyle.Blue;
            mainTabControl.StyleManager = null;
            mainTabControl.TabIndex = 0;
            mainTabControl.TextAlign = ContentAlignment.MiddleLeft;
            mainTabControl.Theme = MetroFramework.MetroThemeStyle.Light;
            mainTabControl.UseStyleColors = false;
            // 
            // gpuTab
            // 
            gpuTab.BackColor = Color.White;
            gpuTab.Controls.Add(vmLabel);
            gpuTab.Controls.Add(vmBox);
            gpuTab.Controls.Add(gpuLabel);
            gpuTab.Controls.Add(gpuBox);
            gpuTab.Controls.Add(allocationLabel);
            gpuTab.Controls.Add(allocationBar);
            gpuTab.Controls.Add(allocPercent);
            gpuTab.Controls.Add(allocateGpuButton);
            gpuTab.Controls.Add(removeGpuButton);
            gpuTab.Controls.Add(installDriverButton);
            gpuTab.CustomBackground = false;
            gpuTab.HorizontalScrollbar = false;
            gpuTab.HorizontalScrollbarBarColor = true;
            gpuTab.HorizontalScrollbarHighlightOnWheel = false;
            gpuTab.HorizontalScrollbarSize = 10;
            gpuTab.Location = new Point(4, 39);
            gpuTab.Name = "gpuTab";
            gpuTab.Size = new Size(352, 377);
            gpuTab.Style = MetroFramework.MetroColorStyle.Blue;
            gpuTab.StyleManager = null;
            gpuTab.TabIndex = 0;
            gpuTab.Text = "GPU Setup";
            gpuTab.Theme = MetroFramework.MetroThemeStyle.Light;
            gpuTab.VerticalScrollbar = false;
            gpuTab.VerticalScrollbarBarColor = true;
            gpuTab.VerticalScrollbarHighlightOnWheel = false;
            gpuTab.VerticalScrollbarSize = 10;
            // 
            // vmLabel
            // 
            vmLabel.AutoSize = true;
            vmLabel.Location = new Point(15, 15);
            vmLabel.Name = "vmLabel";
            vmLabel.Size = new Size(31, 20);
            vmLabel.TabIndex = 2;
            vmLabel.Text = "VM";
            // 
            // vmBox
            // 
            vmBox.DropDownStyle = ComboBoxStyle.DropDownList;
            vmBox.FormattingEnabled = true;
            vmBox.Location = new Point(15, 40);
            vmBox.Name = "vmBox";
            vmBox.Size = new Size(317, 28);
            vmBox.TabIndex = 3;
            // 
            // gpuLabel
            // 
            gpuLabel.AutoSize = true;
            gpuLabel.Location = new Point(15, 85);
            gpuLabel.Name = "gpuLabel";
            gpuLabel.Size = new Size(37, 20);
            gpuLabel.TabIndex = 4;
            gpuLabel.Text = "GPU";
            // 
            // gpuBox
            // 
            gpuBox.DropDownStyle = ComboBoxStyle.DropDownList;
            gpuBox.FormattingEnabled = true;
            gpuBox.Location = new Point(15, 110);
            gpuBox.Name = "gpuBox";
            gpuBox.Size = new Size(317, 28);
            gpuBox.TabIndex = 5;
            // 
            // allocationLabel
            // 
            allocationLabel.AutoSize = true;
            allocationLabel.Location = new Point(15, 160);
            allocationLabel.Name = "allocationLabel";
            allocationLabel.Size = new Size(156, 20);
            allocationLabel.TabIndex = 6;
            allocationLabel.Text = "Allocation percentage";
            // 
            // allocationBar
            // 
            allocationBar.BackColor = Color.Transparent;
            allocationBar.CustomBackground = false;
            allocationBar.LargeChange = 5U;
            allocationBar.Location = new Point(15, 185);
            allocationBar.Maximum = 100;
            allocationBar.Minimum = 5;
            allocationBar.MouseWheelBarPartitions = 10;
            allocationBar.Name = "allocationBar";
            allocationBar.Size = new Size(255, 30);
            allocationBar.SmallChange = 1U;
            allocationBar.Style = MetroFramework.MetroColorStyle.Blue;
            allocationBar.StyleManager = null;
            allocationBar.TabIndex = 7;
            allocationBar.Theme = MetroFramework.MetroThemeStyle.Light;
            allocationBar.Value = 50;
            allocationBar.ValueChanged += AllocationBar_ValueChanged;
            // 
            // allocPercent
            // 
            allocPercent.AutoSize = true;
            allocPercent.Location = new Point(290, 190);
            allocPercent.Name = "allocPercent";
            allocPercent.Size = new Size(37, 20);
            allocPercent.TabIndex = 8;
            allocPercent.Text = "50%";
            // 
            // allocateGpuButton
            // 
            allocateGpuButton.Location = new Point(15, 240);
            allocateGpuButton.Name = "allocateGpuButton";
            allocateGpuButton.Size = new Size(144, 48);
            allocateGpuButton.TabIndex = 9;
            allocateGpuButton.Text = "Allocate";
            allocateGpuButton.UseVisualStyleBackColor = true;
            allocateGpuButton.Click += AllocateGpuButton_Click;
            // 
            // removeGpuButton
            // 
            removeGpuButton.Location = new Point(175, 240);
            removeGpuButton.Name = "removeGpuButton";
            removeGpuButton.Size = new Size(157, 48);
            removeGpuButton.TabIndex = 10;
            removeGpuButton.Text = "Remove";
            removeGpuButton.UseVisualStyleBackColor = true;
            removeGpuButton.Click += RemoveGpuButton_Click;
            // 
            // installDriverButton
            // 
            installDriverButton.Location = new Point(15, 305);
            installDriverButton.Name = "installDriverButton";
            installDriverButton.Size = new Size(317, 48);
            installDriverButton.TabIndex = 11;
            installDriverButton.Text = "Update driver";
            installDriverButton.UseVisualStyleBackColor = true;
            installDriverButton.Click += InstallDriverButton_Click;
            // 
            // networkTab
            // 
            networkTab.BackColor = Color.White;
            networkTab.Controls.Add(networkAdapterLabel);
            networkTab.Controls.Add(networkAdapterBox);
            networkTab.Controls.Add(ipListLabel);
            networkTab.Controls.Add(ipListBox);
            networkTab.Controls.Add(addIpButton);
            networkTab.Controls.Add(removeIpButton);
            networkTab.Controls.Add(applyNetworkButton);
            networkTab.CustomBackground = false;
            networkTab.HorizontalScrollbar = false;
            networkTab.HorizontalScrollbarBarColor = true;
            networkTab.HorizontalScrollbarHighlightOnWheel = false;
            networkTab.HorizontalScrollbarSize = 10;
            networkTab.Location = new Point(4, 39);
            networkTab.Name = "networkTab";
            networkTab.Size = new Size(352, 377);
            networkTab.Style = MetroFramework.MetroColorStyle.Blue;
            networkTab.StyleManager = null;
            networkTab.TabIndex = 1;
            networkTab.Text = "Network Sandbox";
            networkTab.Theme = MetroFramework.MetroThemeStyle.Light;
            networkTab.VerticalScrollbar = false;
            networkTab.VerticalScrollbarBarColor = true;
            networkTab.VerticalScrollbarHighlightOnWheel = false;
            networkTab.VerticalScrollbarSize = 10;
            // 
            // networkAdapterLabel
            // 
            networkAdapterLabel.AutoSize = true;
            networkAdapterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            networkAdapterLabel.Location = new Point(15, 15);
            networkAdapterLabel.Name = "networkAdapterLabel";
            networkAdapterLabel.Size = new Size(184, 20);
            networkAdapterLabel.TabIndex = 2;
            networkAdapterLabel.Text = "Active Network Adapter:";
            // 
            // networkAdapterBox
            // 
            networkAdapterBox.DropDownStyle = ComboBoxStyle.DropDownList;
            networkAdapterBox.FormattingEnabled = true;
            networkAdapterBox.Location = new Point(15, 40);
            networkAdapterBox.Name = "networkAdapterBox";
            networkAdapterBox.Size = new Size(317, 28);
            networkAdapterBox.TabIndex = 3;
            // 
            // ipListLabel
            // 
            ipListLabel.AutoSize = true;
            ipListLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ipListLabel.Location = new Point(15, 85);
            ipListLabel.Name = "ipListLabel";
            ipListLabel.Size = new Size(225, 20);
            ipListLabel.TabIndex = 4;
            ipListLabel.Text = "Allowed Client IPs (Xbox / TV):";
            // 
            // ipListBox
            // 
            ipListBox.FormattingEnabled = true;
            ipListBox.Location = new Point(15, 110);
            ipListBox.Name = "ipListBox";
            ipListBox.Size = new Size(317, 92);
            ipListBox.TabIndex = 5;
            ipListBox.ItemCheck += IpListBox_ItemCheck;
            ipListBox.MouseDown += IpListBox_MouseDown;
            // 
            // addIpButton
            // 
            addIpButton.Location = new Point(15, 230);
            addIpButton.Name = "addIpButton";
            addIpButton.Size = new Size(144, 40);
            addIpButton.TabIndex = 6;
            addIpButton.Text = "Add IP";
            addIpButton.UseVisualStyleBackColor = true;
            addIpButton.Click += AddIpButton_Click;
            // 
            // removeIpButton
            // 
            removeIpButton.Location = new Point(175, 230);
            removeIpButton.Name = "removeIpButton";
            removeIpButton.Size = new Size(157, 40);
            removeIpButton.TabIndex = 7;
            removeIpButton.Text = "Remove IP";
            removeIpButton.UseVisualStyleBackColor = true;
            removeIpButton.Click += RemoveIpButton_Click;
            // 
            // applyNetworkButton
            // 
            applyNetworkButton.BackColor = Color.FromArgb(0, 120, 215);
            applyNetworkButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            applyNetworkButton.ForeColor = Color.Black;
            applyNetworkButton.Location = new Point(15, 290);
            applyNetworkButton.Name = "applyNetworkButton";
            applyNetworkButton.Size = new Size(317, 60);
            applyNetworkButton.TabIndex = 8;
            applyNetworkButton.Text = "Apply Network Config";
            applyNetworkButton.UseVisualStyleBackColor = false;
            applyNetworkButton.Click += ApplyNetworkButton_Click;
            // 
            // creditLabel
            // 
            creditLabel.AutoSize = true;
            creditLabel.Location = new Point(115, 500);
            creditLabel.Name = "creditLabel";
            creditLabel.Size = new Size(176, 20);
            creditLabel.TabIndex = 1;
            creditLabel.Text = "with ❤ by @b1on1cdog";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 540);
            Controls.Add(mainTabControl);
            Controls.Add(creditLabel);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Location = new Point(0, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "Fast GPU-P";
            mainTabControl.ResumeLayout(false);
            gpuTab.ResumeLayout(false);
            gpuTab.PerformLayout();
            networkTab.ResumeLayout(false);
            networkTab.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // Tab Control
        private MetroFramework.Controls.MetroTabControl mainTabControl;
        private MetroFramework.Controls.MetroTabPage gpuTab;
        private MetroFramework.Controls.MetroTabPage networkTab;

        // GPU Tab
        private ComboBox gpuBox;
        private Label gpuLabel;
        private ComboBox vmBox;
        private Label vmLabel;
        private Button allocateGpuButton;
        private Label allocationLabel;
        private MetroFramework.Controls.MetroTrackBar allocationBar;
        private Label allocPercent;
        private Button installDriverButton;
        private Button removeGpuButton;

        // Network Tab
        private Label networkAdapterLabel;
        private ComboBox networkAdapterBox;
        private Label ipListLabel;
        private CheckedListBox ipListBox;
        private Button addIpButton;
        private Button removeIpButton;
        private Button applyNetworkButton;

        private Label creditLabel;
    }
}

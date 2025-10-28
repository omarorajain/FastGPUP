namespace FastGPU_P
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            gpuBox = new ComboBox();
            gpuLabel = new Label();
            vmBox = new ComboBox();
            vmLabel = new Label();
            addButton = new Button();
            AllocationLabel = new Label();
            allocationBar = new MetroFramework.Controls.MetroTrackBar();
            allocPercent = new Label();
            installDriverBtn = new Button();
            RemoveButton = new Button();
            creditLabel = new Label();
            SuspendLayout();
            // 
            // gpuBox
            // 
            gpuBox.DropDownStyle = ComboBoxStyle.DropDownList;
            gpuBox.FormattingEnabled = true;
            gpuBox.Location = new Point(23, 188);
            gpuBox.Margin = new Padding(3, 6, 3, 6);
            gpuBox.Name = "gpuBox";
            gpuBox.Size = new Size(315, 28);
            gpuBox.TabIndex = 0;
            // 
            // gpuLabel
            // 
            gpuLabel.AutoSize = true;
            gpuLabel.Location = new Point(23, 154);
            gpuLabel.Name = "gpuLabel";
            gpuLabel.Size = new Size(37, 20);
            gpuLabel.TabIndex = 1;
            gpuLabel.Text = "GPU";
            // 
            // vmBox
            // 
            vmBox.DropDownStyle = ComboBoxStyle.DropDownList;
            vmBox.FormattingEnabled = true;
            vmBox.Location = new Point(23, 106);
            vmBox.Margin = new Padding(3, 6, 3, 6);
            vmBox.Name = "vmBox";
            vmBox.Size = new Size(317, 28);
            vmBox.TabIndex = 2;
            // 
            // vmLabel
            // 
            vmLabel.AutoSize = true;
            vmLabel.Location = new Point(23, 70);
            vmLabel.Name = "vmLabel";
            vmLabel.Size = new Size(31, 20);
            vmLabel.TabIndex = 3;
            vmLabel.Text = "VM";
            // 
            // addButton
            // 
            addButton.Location = new Point(25, 318);
            addButton.Margin = new Padding(3, 6, 3, 6);
            addButton.Name = "addButton";
            addButton.Size = new Size(144, 48);
            addButton.TabIndex = 4;
            addButton.Text = "Allocate";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;
            // 
            // AllocationLabel
            // 
            AllocationLabel.AutoSize = true;
            AllocationLabel.Location = new Point(23, 233);
            AllocationLabel.Name = "AllocationLabel";
            AllocationLabel.Size = new Size(156, 20);
            AllocationLabel.TabIndex = 6;
            AllocationLabel.Text = "Allocation percentage";
            // 
            // allocationBar
            // 
            allocationBar.BackColor = Color.Transparent;
            allocationBar.CustomBackground = false;
            allocationBar.LargeChange = 5U;
            allocationBar.Location = new Point(25, 258);
            allocationBar.Margin = new Padding(3, 6, 3, 6);
            allocationBar.Maximum = 100;
            allocationBar.Minimum = 5;
            allocationBar.MouseWheelBarPartitions = 10;
            allocationBar.Name = "allocationBar";
            allocationBar.Size = new Size(255, 48);
            allocationBar.SmallChange = 1U;
            allocationBar.Style = MetroFramework.MetroColorStyle.Blue;
            allocationBar.StyleManager = null;
            allocationBar.TabIndex = 7;
            allocationBar.Text = "null";
            allocationBar.Theme = MetroFramework.MetroThemeStyle.Light;
            allocationBar.Value = 50;
            allocationBar.Scroll += AllocationBar_Scroll;
            // 
            // allocPercent
            // 
            allocPercent.AutoSize = true;
            allocPercent.Location = new Point(297, 272);
            allocPercent.Name = "allocPercent";
            allocPercent.Size = new Size(37, 20);
            allocPercent.TabIndex = 8;
            allocPercent.Text = "50%";
            // 
            // installDriverBtn
            // 
            installDriverBtn.Location = new Point(25, 377);
            installDriverBtn.Margin = new Padding(3, 6, 3, 6);
            installDriverBtn.Name = "installDriverBtn";
            installDriverBtn.Size = new Size(310, 48);
            installDriverBtn.TabIndex = 9;
            installDriverBtn.Text = "Update driver";
            installDriverBtn.UseVisualStyleBackColor = true;
            installDriverBtn.Click += InstallDriverBtn_Click;
            // 
            // RemoveButton
            // 
            RemoveButton.Location = new Point(175, 318);
            RemoveButton.Margin = new Padding(3, 6, 3, 6);
            RemoveButton.Name = "RemoveButton";
            RemoveButton.Size = new Size(159, 48);
            RemoveButton.TabIndex = 10;
            RemoveButton.Text = "Remove";
            RemoveButton.UseVisualStyleBackColor = true;
            RemoveButton.Click += RemoveButton_Click;
            // 
            // creditLabel
            // 
            creditLabel.AutoSize = true;
            creditLabel.Location = new Point(95, 436);
            creditLabel.Name = "creditLabel";
            creditLabel.Size = new Size(176, 20);
            creditLabel.TabIndex = 13;
            creditLabel.Text = "with ❤ by @b1on1cdog";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 490);
            Controls.Add(creditLabel);
            Controls.Add(RemoveButton);
            Controls.Add(installDriverBtn);
            Controls.Add(allocPercent);
            Controls.Add(allocationBar);
            Controls.Add(AllocationLabel);
            Controls.Add(addButton);
            Controls.Add(vmLabel);
            Controls.Add(vmBox);
            Controls.Add(gpuLabel);
            Controls.Add(gpuBox);
            Location = new Point(0, 0);
            Margin = new Padding(3, 6, 3, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Padding = new Padding(20, 100, 20, 34);
            Text = "Fast GPU-P";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox gpuBox;
        private Label gpuLabel;
        private ComboBox vmBox;
        private Label vmLabel;
        private Button addButton;
        private Label AllocationLabel;
        private MetroFramework.Controls.MetroTrackBar allocationBar;
        private Label allocPercent;
        private Button installDriverBtn;
        private Button RemoveButton;
        private Label creditLabel;
    }
}
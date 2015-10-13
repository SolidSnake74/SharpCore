namespace SharpCore.Utilities.EncryptionKeyGenerator
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.symmetricAlgorithmComboBox = new System.Windows.Forms.ComboBox();
			this.symmetricAlgorithmLabel = new System.Windows.Forms.Label();
			this.exitButton = new System.Windows.Forms.Button();
			this.generateButton = new System.Windows.Forms.Button();
			this.ivTextBox = new System.Windows.Forms.TextBox();
			this.keyTextBox = new System.Windows.Forms.TextBox();
			this.ivLabel = new System.Windows.Forms.Label();
			this.keyLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// symmetricAlgorithmComboBox
			// 
			this.symmetricAlgorithmComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.symmetricAlgorithmComboBox.Location = new System.Drawing.Point(126, 12);
			this.symmetricAlgorithmComboBox.Name = "symmetricAlgorithmComboBox";
			this.symmetricAlgorithmComboBox.Size = new System.Drawing.Size(252, 21);
			this.symmetricAlgorithmComboBox.TabIndex = 0;
			// 
			// symmetricAlgorithmLabel
			// 
			this.symmetricAlgorithmLabel.Location = new System.Drawing.Point(12, 18);
			this.symmetricAlgorithmLabel.Name = "symmetricAlgorithmLabel";
			this.symmetricAlgorithmLabel.Size = new System.Drawing.Size(114, 18);
			this.symmetricAlgorithmLabel.TabIndex = 12;
			this.symmetricAlgorithmLabel.Text = "Symmetric Algorithm:";
			// 
			// exitButton
			// 
			this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.exitButton.Location = new System.Drawing.Point(306, 114);
			this.exitButton.Name = "exitButton";
			this.exitButton.Size = new System.Drawing.Size(72, 23);
			this.exitButton.TabIndex = 4;
			this.exitButton.Text = "E&xit";
			this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
			// 
			// generateButton
			// 
			this.generateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.generateButton.Location = new System.Drawing.Point(222, 114);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(72, 24);
			this.generateButton.TabIndex = 3;
			this.generateButton.Text = "&Generate";
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// ivTextBox
			// 
			this.ivTextBox.HideSelection = false;
			this.ivTextBox.Location = new System.Drawing.Point(126, 84);
			this.ivTextBox.Name = "ivTextBox";
			this.ivTextBox.ReadOnly = true;
			this.ivTextBox.Size = new System.Drawing.Size(252, 20);
			this.ivTextBox.TabIndex = 2;
			// 
			// keyTextBox
			// 
			this.keyTextBox.HideSelection = false;
			this.keyTextBox.Location = new System.Drawing.Point(126, 48);
			this.keyTextBox.Name = "keyTextBox";
			this.keyTextBox.ReadOnly = true;
			this.keyTextBox.Size = new System.Drawing.Size(252, 20);
			this.keyTextBox.TabIndex = 1;
			// 
			// ivLabel
			// 
			this.ivLabel.Location = new System.Drawing.Point(12, 84);
			this.ivLabel.Name = "ivLabel";
			this.ivLabel.Size = new System.Drawing.Size(114, 16);
			this.ivLabel.TabIndex = 7;
			this.ivLabel.Text = "Initialization Vector:";
			// 
			// keyLabel
			// 
			this.keyLabel.Location = new System.Drawing.Point(12, 54);
			this.keyLabel.Name = "keyLabel";
			this.keyLabel.Size = new System.Drawing.Size(114, 18);
			this.keyLabel.TabIndex = 5;
			this.keyLabel.Text = "Key:";
			// 
			// MainForm2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(396, 151);
			this.Controls.Add(this.symmetricAlgorithmComboBox);
			this.Controls.Add(this.symmetricAlgorithmLabel);
			this.Controls.Add(this.exitButton);
			this.Controls.Add(this.generateButton);
			this.Controls.Add(this.ivTextBox);
			this.Controls.Add(this.keyTextBox);
			this.Controls.Add(this.ivLabel);
			this.Controls.Add(this.keyLabel);
			this.Name = "MainForm2";
			this.Text = "Encryption Key Generator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox symmetricAlgorithmComboBox;
		private System.Windows.Forms.Label symmetricAlgorithmLabel;
		private System.Windows.Forms.Button exitButton;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.TextBox ivTextBox;
		private System.Windows.Forms.TextBox keyTextBox;
		private System.Windows.Forms.Label ivLabel;
		private System.Windows.Forms.Label keyLabel;
	}
}
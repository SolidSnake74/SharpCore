using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SharpCore.Utilities.EncryptionKeyGenerator {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
			
			symmetricAlgorithmComboBox.DisplayMember = "DisplayMember";
			symmetricAlgorithmComboBox.ValueMember = "ValueMember";
			
			symmetricAlgorithmComboBox.Items.Add(new ComboBoxItem("DESCryptoServiceProvider", "System.Security.Cryptography.DESCryptoServiceProvider"));
			symmetricAlgorithmComboBox.Items.Add(new ComboBoxItem("RC2CryptoServiceProvider", "System.Security.Cryptography.RC2CryptoServiceProvider"));
			symmetricAlgorithmComboBox.Items.Add(new ComboBoxItem("RijndaelManaged", "System.Security.Cryptography.RijndaelManaged"));
			symmetricAlgorithmComboBox.Items.Add(new ComboBoxItem("TripleDESCryptoServiceProvider", "System.Security.Cryptography.TripleDESCryptoServiceProvider"));

			symmetricAlgorithmComboBox.SelectedIndex = 0;
		}
		
		private void generateButton_Click(object sender, EventArgs e) {
			ComboBoxItem comboBoxItem = (ComboBoxItem) symmetricAlgorithmComboBox.SelectedItem;
			Type type = Type.GetType(comboBoxItem.ValueMember);
			SymmetricAlgorithm symmetricAlgorithm = (SymmetricAlgorithm) Activator.CreateInstance(type);
			symmetricAlgorithm.GenerateIV();
			symmetricAlgorithm.GenerateKey();

			byte[] keyBytes = symmetricAlgorithm.Key;
			byte[] ivBytes = symmetricAlgorithm.IV;

			keyTextBox.Text = Convert.ToBase64String(keyBytes);
			ivTextBox.Text = Convert.ToBase64String(ivBytes);
		}
		
		private void exitButton_Click(object sender, EventArgs e) {
			base.Close();
		}
	}
}
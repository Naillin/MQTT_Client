namespace MQTT_Client
{
	partial class InputBox
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
			this.buttonInput = new System.Windows.Forms.Button();
			this.textBoxInput = new System.Windows.Forms.TextBox();
			this.labelInput = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonInput
			// 
			this.buttonInput.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonInput.Location = new System.Drawing.Point(102, 76);
			this.buttonInput.Name = "buttonInput";
			this.buttonInput.Size = new System.Drawing.Size(75, 23);
			this.buttonInput.TabIndex = 0;
			this.buttonInput.Text = "Input";
			this.buttonInput.UseVisualStyleBackColor = true;
			this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
			// 
			// textBoxInput
			// 
			this.textBoxInput.Location = new System.Drawing.Point(12, 41);
			this.textBoxInput.Name = "textBoxInput";
			this.textBoxInput.Size = new System.Drawing.Size(260, 20);
			this.textBoxInput.TabIndex = 1;
			// 
			// labelInput
			// 
			this.labelInput.AutoSize = true;
			this.labelInput.Location = new System.Drawing.Point(9, 9);
			this.labelInput.Name = "labelInput";
			this.labelInput.Size = new System.Drawing.Size(29, 13);
			this.labelInput.TabIndex = 2;
			this.labelInput.Text = "label";
			// 
			// InputBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 111);
			this.Controls.Add(this.labelInput);
			this.Controls.Add(this.textBoxInput);
			this.Controls.Add(this.buttonInput);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "InputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "InputBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonInput;
		private System.Windows.Forms.TextBox textBoxInput;
		private System.Windows.Forms.Label labelInput;
	}
}
namespace MQTT_Client.FormElements
{
	partial class RuleControl
	{
		/// <summary> 
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxFirebaseReference = new System.Windows.Forms.TextBox();
			this.textBoxMQTTtopic = new System.Windows.Forms.TextBox();
			this.buttonSwitch = new System.Windows.Forms.Button();
			this.labelFirebaseReference = new System.Windows.Forms.Label();
			this.labelMQTTtopic = new System.Windows.Forms.Label();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.checkBoxNewField = new System.Windows.Forms.CheckBox();
			this.checkBoxTimestamp = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textBoxFirebaseReference
			// 
			this.textBoxFirebaseReference.BackColor = System.Drawing.Color.Cornsilk;
			this.textBoxFirebaseReference.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic);
			this.textBoxFirebaseReference.Location = new System.Drawing.Point(3, 48);
			this.textBoxFirebaseReference.Name = "textBoxFirebaseReference";
			this.textBoxFirebaseReference.Size = new System.Drawing.Size(350, 25);
			this.textBoxFirebaseReference.TabIndex = 0;
			this.textBoxFirebaseReference.TextChanged += new System.EventHandler(this.textBoxFirebaseReference_TextChanged);
			// 
			// textBoxMQTTtopic
			// 
			this.textBoxMQTTtopic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMQTTtopic.BackColor = System.Drawing.Color.Cornsilk;
			this.textBoxMQTTtopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic);
			this.textBoxMQTTtopic.Location = new System.Drawing.Point(465, 48);
			this.textBoxMQTTtopic.Name = "textBoxMQTTtopic";
			this.textBoxMQTTtopic.Size = new System.Drawing.Size(350, 25);
			this.textBoxMQTTtopic.TabIndex = 1;
			this.textBoxMQTTtopic.TextChanged += new System.EventHandler(this.textBoxMQTTtopic_TextChanged);
			// 
			// buttonSwitch
			// 
			this.buttonSwitch.BackColor = System.Drawing.Color.Wheat;
			this.buttonSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSwitch.Font = new System.Drawing.Font("Franklin Gothic Demi Cond", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonSwitch.Location = new System.Drawing.Point(359, 48);
			this.buttonSwitch.Name = "buttonSwitch";
			this.buttonSwitch.Size = new System.Drawing.Size(100, 25);
			this.buttonSwitch.TabIndex = 2;
			this.buttonSwitch.Text = ">>";
			this.buttonSwitch.UseVisualStyleBackColor = false;
			this.buttonSwitch.Click += new System.EventHandler(this.buttonSwitch_Click);
			// 
			// labelFirebaseReference
			// 
			this.labelFirebaseReference.AutoSize = true;
			this.labelFirebaseReference.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelFirebaseReference.Location = new System.Drawing.Point(3, 25);
			this.labelFirebaseReference.Name = "labelFirebaseReference";
			this.labelFirebaseReference.Size = new System.Drawing.Size(142, 21);
			this.labelFirebaseReference.TabIndex = 3;
			this.labelFirebaseReference.Text = "Firebase reference:";
			// 
			// labelMQTTtopic
			// 
			this.labelMQTTtopic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMQTTtopic.AutoSize = true;
			this.labelMQTTtopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelMQTTtopic.Location = new System.Drawing.Point(727, 25);
			this.labelMQTTtopic.Name = "labelMQTTtopic";
			this.labelMQTTtopic.Size = new System.Drawing.Size(88, 21);
			this.labelMQTTtopic.TabIndex = 4;
			this.labelMQTTtopic.Text = "MQTT topic:";
			// 
			// buttonDelete
			// 
			this.buttonDelete.BackColor = System.Drawing.Color.Red;
			this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDelete.Font = new System.Drawing.Font("Franklin Gothic Demi", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonDelete.ForeColor = System.Drawing.Color.White;
			this.buttonDelete.Location = new System.Drawing.Point(793, 0);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(25, 25);
			this.buttonDelete.TabIndex = 5;
			this.buttonDelete.Text = "X";
			this.buttonDelete.UseVisualStyleBackColor = false;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// checkBoxNewField
			// 
			this.checkBoxNewField.AutoSize = true;
			this.checkBoxNewField.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic);
			this.checkBoxNewField.Location = new System.Drawing.Point(648, 0);
			this.checkBoxNewField.Name = "checkBoxNewField";
			this.checkBoxNewField.Size = new System.Drawing.Size(139, 25);
			this.checkBoxNewField.TabIndex = 6;
			this.checkBoxNewField.Text = "Create new field";
			this.checkBoxNewField.UseVisualStyleBackColor = true;
			this.checkBoxNewField.CheckedChanged += new System.EventHandler(this.checkBoxNewField_CheckedChanged);
			// 
			// checkBoxTimestamp
			// 
			this.checkBoxTimestamp.AutoSize = true;
			this.checkBoxTimestamp.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic);
			this.checkBoxTimestamp.Location = new System.Drawing.Point(509, 0);
			this.checkBoxTimestamp.Name = "checkBoxTimestamp";
			this.checkBoxTimestamp.Size = new System.Drawing.Size(133, 25);
			this.checkBoxTimestamp.TabIndex = 7;
			this.checkBoxTimestamp.Text = "Add timestamp";
			this.checkBoxTimestamp.UseVisualStyleBackColor = true;
			this.checkBoxTimestamp.CheckedChanged += new System.EventHandler(this.checkBoxTimestamp_CheckedChanged);
			// 
			// RuleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.PapayaWhip;
			this.Controls.Add(this.checkBoxTimestamp);
			this.Controls.Add(this.checkBoxNewField);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.labelMQTTtopic);
			this.Controls.Add(this.labelFirebaseReference);
			this.Controls.Add(this.buttonSwitch);
			this.Controls.Add(this.textBoxMQTTtopic);
			this.Controls.Add(this.textBoxFirebaseReference);
			this.Name = "RuleControl";
			this.Size = new System.Drawing.Size(818, 79);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxFirebaseReference;
		private System.Windows.Forms.TextBox textBoxMQTTtopic;
		private System.Windows.Forms.Button buttonSwitch;
		private System.Windows.Forms.Label labelFirebaseReference;
		private System.Windows.Forms.Label labelMQTTtopic;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.CheckBox checkBoxNewField;
		private System.Windows.Forms.CheckBox checkBoxTimestamp;
	}
}

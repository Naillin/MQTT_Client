namespace MQTT_Client
{
	partial class FormMain
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

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.textBoxInfo = new System.Windows.Forms.TextBox();
			this.buttonAddTopic = new System.Windows.Forms.Button();
			this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
			this.labelStatus = new System.Windows.Forms.Label();
			this.buttonSend = new System.Windows.Forms.Button();
			this.treeViewMain = new System.Windows.Forms.TreeView();
			this.textBoxMessage = new System.Windows.Forms.TextBox();
			this.labelTopic = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxInfo
			// 
			this.textBoxInfo.BackColor = System.Drawing.Color.PapayaWhip;
			this.textBoxInfo.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBoxInfo.Location = new System.Drawing.Point(272, 43);
			this.textBoxInfo.Multiline = true;
			this.textBoxInfo.Name = "textBoxInfo";
			this.textBoxInfo.ReadOnly = true;
			this.textBoxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxInfo.Size = new System.Drawing.Size(566, 375);
			this.textBoxInfo.TabIndex = 0;
			// 
			// buttonAddTopic
			// 
			this.buttonAddTopic.BackColor = System.Drawing.Color.Wheat;
			this.buttonAddTopic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddTopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonAddTopic.Location = new System.Drawing.Point(272, 501);
			this.buttonAddTopic.Name = "buttonAddTopic";
			this.buttonAddTopic.Size = new System.Drawing.Size(566, 37);
			this.buttonAddTopic.TabIndex = 1;
			this.buttonAddTopic.Text = "Add topic";
			this.buttonAddTopic.UseVisualStyleBackColor = false;
			this.buttonAddTopic.Click += new System.EventHandler(this.buttonAddTopic_Click);
			// 
			// pictureBoxStatus
			// 
			this.pictureBoxStatus.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxStatus.BackgroundImage = global::MQTT_Client.Properties.Resources.red;
			this.pictureBoxStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBoxStatus.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxStatus.Name = "pictureBoxStatus";
			this.pictureBoxStatus.Size = new System.Drawing.Size(25, 25);
			this.pictureBoxStatus.TabIndex = 2;
			this.pictureBoxStatus.TabStop = false;
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.BackColor = System.Drawing.Color.Transparent;
			this.labelStatus.Font = new System.Drawing.Font("Franklin Gothic Medium", 15.75F, System.Drawing.FontStyle.Italic);
			this.labelStatus.Location = new System.Drawing.Point(43, 11);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(113, 26);
			this.labelStatus.TabIndex = 3;
			this.labelStatus.Text = "labelStatus";
			// 
			// buttonSend
			// 
			this.buttonSend.BackColor = System.Drawing.Color.Wheat;
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSend.Font = new System.Drawing.Font("Franklin Gothic Medium", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonSend.Location = new System.Drawing.Point(272, 458);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(566, 37);
			this.buttonSend.TabIndex = 4;
			this.buttonSend.Text = "Send message";
			this.buttonSend.UseVisualStyleBackColor = false;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// treeViewMain
			// 
			this.treeViewMain.BackColor = System.Drawing.Color.OldLace;
			this.treeViewMain.Font = new System.Drawing.Font("Franklin Gothic Medium", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.treeViewMain.Location = new System.Drawing.Point(12, 43);
			this.treeViewMain.Name = "treeViewMain";
			this.treeViewMain.Size = new System.Drawing.Size(254, 495);
			this.treeViewMain.TabIndex = 5;
			this.treeViewMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMain_AfterSelect);
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.BackColor = System.Drawing.Color.Cornsilk;
			this.textBoxMessage.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic);
			this.textBoxMessage.Location = new System.Drawing.Point(272, 424);
			this.textBoxMessage.Multiline = true;
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.Size = new System.Drawing.Size(566, 28);
			this.textBoxMessage.TabIndex = 6;
			this.textBoxMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMessage_KeyDown);
			// 
			// labelTopic
			// 
			this.labelTopic.AutoSize = true;
			this.labelTopic.BackColor = System.Drawing.Color.Transparent;
			this.labelTopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 15.75F, System.Drawing.FontStyle.Italic);
			this.labelTopic.Location = new System.Drawing.Point(267, 12);
			this.labelTopic.Name = "labelTopic";
			this.labelTopic.Size = new System.Drawing.Size(102, 26);
			this.labelTopic.TabIndex = 7;
			this.labelTopic.Text = "labelTopic";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.BurlyWood;
			this.ClientSize = new System.Drawing.Size(850, 550);
			this.Controls.Add(this.labelTopic);
			this.Controls.Add(this.textBoxMessage);
			this.Controls.Add(this.treeViewMain);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.pictureBoxStatus);
			this.Controls.Add(this.buttonAddTopic);
			this.Controls.Add(this.textBoxInfo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MQTT Client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxInfo;
		private System.Windows.Forms.Button buttonAddTopic;
		private System.Windows.Forms.PictureBox pictureBoxStatus;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.TreeView treeViewMain;
		private System.Windows.Forms.TextBox textBoxMessage;
		private System.Windows.Forms.Label labelTopic;
	}
}


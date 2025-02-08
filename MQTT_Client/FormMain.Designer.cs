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
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageClient = new System.Windows.Forms.TabPage();
			this.tabPageRules = new System.Windows.Forms.TabPage();
			this.buttonAddRule = new System.Windows.Forms.Button();
			this.buttonStartStop = new System.Windows.Forms.Button();
			this.flowLayoutPanelRules = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
			this.tabControlMain.SuspendLayout();
			this.tabPageClient.SuspendLayout();
			this.tabPageRules.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxInfo
			// 
			this.textBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxInfo.BackColor = System.Drawing.Color.PapayaWhip;
			this.textBoxInfo.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBoxInfo.Location = new System.Drawing.Point(12, 599);
			this.textBoxInfo.Multiline = true;
			this.textBoxInfo.Name = "textBoxInfo";
			this.textBoxInfo.ReadOnly = true;
			this.textBoxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxInfo.Size = new System.Drawing.Size(844, 282);
			this.textBoxInfo.TabIndex = 0;
			// 
			// buttonAddTopic
			// 
			this.buttonAddTopic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddTopic.BackColor = System.Drawing.Color.Wheat;
			this.buttonAddTopic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddTopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonAddTopic.Location = new System.Drawing.Point(353, 112);
			this.buttonAddTopic.Name = "buttonAddTopic";
			this.buttonAddTopic.Size = new System.Drawing.Size(477, 37);
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
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.BackColor = System.Drawing.Color.Wheat;
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSend.Font = new System.Drawing.Font("Franklin Gothic Medium", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonSend.Location = new System.Drawing.Point(353, 69);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(477, 37);
			this.buttonSend.TabIndex = 4;
			this.buttonSend.Text = "Send message";
			this.buttonSend.UseVisualStyleBackColor = false;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// treeViewMain
			// 
			this.treeViewMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.treeViewMain.BackColor = System.Drawing.Color.OldLace;
			this.treeViewMain.Font = new System.Drawing.Font("Franklin Gothic Medium", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.treeViewMain.Location = new System.Drawing.Point(6, 6);
			this.treeViewMain.Name = "treeViewMain";
			this.treeViewMain.Size = new System.Drawing.Size(341, 495);
			this.treeViewMain.TabIndex = 5;
			this.treeViewMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMain_AfterSelect);
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMessage.BackColor = System.Drawing.Color.Cornsilk;
			this.textBoxMessage.Font = new System.Drawing.Font("Franklin Gothic Medium", 11.25F, System.Drawing.FontStyle.Italic);
			this.textBoxMessage.Location = new System.Drawing.Point(353, 35);
			this.textBoxMessage.Multiline = true;
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.Size = new System.Drawing.Size(477, 28);
			this.textBoxMessage.TabIndex = 6;
			this.textBoxMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMessage_KeyDown);
			// 
			// labelTopic
			// 
			this.labelTopic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTopic.AutoSize = true;
			this.labelTopic.BackColor = System.Drawing.Color.Transparent;
			this.labelTopic.Font = new System.Drawing.Font("Franklin Gothic Medium", 15.75F, System.Drawing.FontStyle.Italic);
			this.labelTopic.Location = new System.Drawing.Point(353, 6);
			this.labelTopic.Name = "labelTopic";
			this.labelTopic.Size = new System.Drawing.Size(102, 26);
			this.labelTopic.TabIndex = 7;
			this.labelTopic.Text = "labelTopic";
			// 
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabPageClient);
			this.tabControlMain.Controls.Add(this.tabPageRules);
			this.tabControlMain.Font = new System.Drawing.Font("Franklin Gothic Medium", 18F, System.Drawing.FontStyle.Italic);
			this.tabControlMain.Location = new System.Drawing.Point(12, 43);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(844, 550);
			this.tabControlMain.TabIndex = 8;
			// 
			// tabPageClient
			// 
			this.tabPageClient.BackColor = System.Drawing.Color.OldLace;
			this.tabPageClient.Controls.Add(this.treeViewMain);
			this.tabPageClient.Controls.Add(this.textBoxMessage);
			this.tabPageClient.Controls.Add(this.buttonSend);
			this.tabPageClient.Controls.Add(this.labelTopic);
			this.tabPageClient.Controls.Add(this.buttonAddTopic);
			this.tabPageClient.Location = new System.Drawing.Point(4, 39);
			this.tabPageClient.Name = "tabPageClient";
			this.tabPageClient.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageClient.Size = new System.Drawing.Size(836, 507);
			this.tabPageClient.TabIndex = 0;
			this.tabPageClient.Text = "MQTT Client";
			// 
			// tabPageRules
			// 
			this.tabPageRules.BackColor = System.Drawing.Color.OldLace;
			this.tabPageRules.Controls.Add(this.buttonAddRule);
			this.tabPageRules.Controls.Add(this.buttonStartStop);
			this.tabPageRules.Controls.Add(this.flowLayoutPanelRules);
			this.tabPageRules.Location = new System.Drawing.Point(4, 39);
			this.tabPageRules.Name = "tabPageRules";
			this.tabPageRules.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRules.Size = new System.Drawing.Size(836, 507);
			this.tabPageRules.TabIndex = 1;
			this.tabPageRules.Text = "Rules";
			// 
			// buttonAddRule
			// 
			this.buttonAddRule.BackColor = System.Drawing.Color.Wheat;
			this.buttonAddRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddRule.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonAddRule.Location = new System.Drawing.Point(680, 6);
			this.buttonAddRule.Name = "buttonAddRule";
			this.buttonAddRule.Size = new System.Drawing.Size(150, 32);
			this.buttonAddRule.TabIndex = 2;
			this.buttonAddRule.Text = "Add Rule";
			this.buttonAddRule.UseVisualStyleBackColor = false;
			this.buttonAddRule.Click += new System.EventHandler(this.buttonAddRule_Click);
			// 
			// buttonStartStop
			// 
			this.buttonStartStop.BackColor = System.Drawing.Color.Wheat;
			this.buttonStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonStartStop.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.buttonStartStop.Location = new System.Drawing.Point(6, 6);
			this.buttonStartStop.Name = "buttonStartStop";
			this.buttonStartStop.Size = new System.Drawing.Size(150, 32);
			this.buttonStartStop.TabIndex = 1;
			this.buttonStartStop.Text = "Start";
			this.buttonStartStop.UseVisualStyleBackColor = false;
			this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
			// 
			// flowLayoutPanelRules
			// 
			this.flowLayoutPanelRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanelRules.AutoScroll = true;
			this.flowLayoutPanelRules.BackColor = System.Drawing.Color.Tan;
			this.flowLayoutPanelRules.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.flowLayoutPanelRules.Location = new System.Drawing.Point(6, 44);
			this.flowLayoutPanelRules.Name = "flowLayoutPanelRules";
			this.flowLayoutPanelRules.Size = new System.Drawing.Size(824, 457);
			this.flowLayoutPanelRules.TabIndex = 0;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.BurlyWood;
			this.ClientSize = new System.Drawing.Size(870, 893);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.textBoxInfo);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.pictureBoxStatus);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MQTT Client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
			this.tabControlMain.ResumeLayout(false);
			this.tabPageClient.ResumeLayout(false);
			this.tabPageClient.PerformLayout();
			this.tabPageRules.ResumeLayout(false);
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
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageClient;
		private System.Windows.Forms.TabPage tabPageRules;
		private System.Windows.Forms.Button buttonStartStop;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRules;
		private System.Windows.Forms.Button buttonAddRule;
	}
}


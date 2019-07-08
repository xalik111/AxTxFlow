namespace AuthenticTxFlow
{
	partial class MainForm
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
			this.CurrentTimeLabel = new System.Windows.Forms.Label();
			this.LastTransactionTimeLabel = new System.Windows.Forms.Label();
			this.PerSecLabel = new System.Windows.Forms.Label();
			this.PerMinLabel = new System.Windows.Forms.Label();
			this.PerHourLabel = new System.Windows.Forms.Label();
			this.FromMidnightLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// CurrentTimeLabel
			// 
			this.CurrentTimeLabel.AutoSize = true;
			this.CurrentTimeLabel.ForeColor = System.Drawing.Color.White;
			this.CurrentTimeLabel.Location = new System.Drawing.Point(13, 13);
			this.CurrentTimeLabel.Name = "CurrentTimeLabel";
			this.CurrentTimeLabel.Size = new System.Drawing.Size(0, 13);
			this.CurrentTimeLabel.TabIndex = 0;
			// 
			// LastTransactionTimeLabel
			// 
			this.LastTransactionTimeLabel.AutoSize = true;
			this.LastTransactionTimeLabel.ForeColor = System.Drawing.Color.White;
			this.LastTransactionTimeLabel.Location = new System.Drawing.Point(52, 13);
			this.LastTransactionTimeLabel.Name = "LastTransactionTimeLabel";
			this.LastTransactionTimeLabel.Size = new System.Drawing.Size(0, 13);
			this.LastTransactionTimeLabel.TabIndex = 1;
			// 
			// PerSecLabel
			// 
			this.PerSecLabel.AutoSize = true;
			this.PerSecLabel.ForeColor = System.Drawing.Color.White;
			this.PerSecLabel.Location = new System.Drawing.Point(88, 13);
			this.PerSecLabel.Name = "PerSecLabel";
			this.PerSecLabel.Size = new System.Drawing.Size(0, 13);
			this.PerSecLabel.TabIndex = 2;
			this.PerSecLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpenStatFormSec);
			// 
			// PerMinLabel
			// 
			this.PerMinLabel.AutoSize = true;
			this.PerMinLabel.ForeColor = System.Drawing.Color.White;
			this.PerMinLabel.Location = new System.Drawing.Point(119, 13);
			this.PerMinLabel.Name = "PerMinLabel";
			this.PerMinLabel.Size = new System.Drawing.Size(0, 13);
			this.PerMinLabel.TabIndex = 3;
			this.PerMinLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpenStatFormMin);
			// 
			// PerHourLabel
			// 
			this.PerHourLabel.AutoSize = true;
			this.PerHourLabel.ForeColor = System.Drawing.Color.White;
			this.PerHourLabel.Location = new System.Drawing.Point(139, 13);
			this.PerHourLabel.Name = "PerHourLabel";
			this.PerHourLabel.Size = new System.Drawing.Size(0, 13);
			this.PerHourLabel.TabIndex = 4;
			this.PerHourLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpenStatFormHour);
			// 
			// FromMidnightLabel
			// 
			this.FromMidnightLabel.AutoSize = true;
			this.FromMidnightLabel.ForeColor = System.Drawing.Color.White;
			this.FromMidnightLabel.Location = new System.Drawing.Point(155, 13);
			this.FromMidnightLabel.Name = "FromMidnightLabel";
			this.FromMidnightLabel.Size = new System.Drawing.Size(0, 13);
			this.FromMidnightLabel.TabIndex = 5;
			this.FromMidnightLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpenStatFormDay);
			// 
			// mainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(520, 341);
			this.Controls.Add(this.FromMidnightLabel);
			this.Controls.Add(this.PerHourLabel);
			this.Controls.Add(this.PerMinLabel);
			this.Controls.Add(this.PerSecLabel);
			this.Controls.Add(this.LastTransactionTimeLabel);
			this.Controls.Add(this.CurrentTimeLabel);
			this.DoubleBuffered = true;
			this.Icon = global::AuthenticTxFlow.Properties.Resources.favicon;
			this.Name = "mainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Authentic Transaction Flow";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ClientSizeChanged += new System.EventHandler(this.Resized);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label CurrentTimeLabel;
        private System.Windows.Forms.Label LastTransactionTimeLabel;
        private System.Windows.Forms.Label PerSecLabel;
        private System.Windows.Forms.Label PerMinLabel;
        private System.Windows.Forms.Label PerHourLabel;
        private System.Windows.Forms.Label FromMidnightLabel;
    }
}


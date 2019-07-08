namespace AuthenticTxFlow
{
	partial class MaxStatsForm
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.StatGridView = new System.Windows.Forms.DataGridView();
			this.statBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.astimestampDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.asnumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.StatGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// StatGridView
			// 
			this.StatGridView.AllowUserToAddRows = false;
			this.StatGridView.AllowUserToDeleteRows = false;
			this.StatGridView.AllowUserToResizeColumns = false;
			this.StatGridView.AllowUserToResizeRows = false;
			this.StatGridView.AutoGenerateColumns = false;
			this.StatGridView.BackgroundColor = System.Drawing.Color.White;
			this.StatGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.StatGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.astimestampDataGridViewTextBoxColumn,
            this.asnumberDataGridViewTextBoxColumn});
			this.StatGridView.DataSource = this.statBindingSource;
			this.StatGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StatGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.StatGridView.Location = new System.Drawing.Point(0, 0);
			this.StatGridView.Name = "StatGridView";
			this.StatGridView.ReadOnly = true;
			this.StatGridView.RowHeadersVisible = false;
			this.StatGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.StatGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.StatGridView.Size = new System.Drawing.Size(400, 286);
			this.StatGridView.TabIndex = 1;
			// 
			// statBindingSource
			// 
			this.statBindingSource.DataSource = typeof(AuthenticTxFlow.Stat);
			// 
			// astimestampDataGridViewTextBoxColumn
			// 
			this.astimestampDataGridViewTextBoxColumn.DataPropertyName = "as_timestamp";
			dataGridViewCellStyle1.NullValue = null;
			this.astimestampDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.astimestampDataGridViewTextBoxColumn.HeaderText = "Timestamp";
			this.astimestampDataGridViewTextBoxColumn.Name = "astimestampDataGridViewTextBoxColumn";
			this.astimestampDataGridViewTextBoxColumn.ReadOnly = true;
			this.astimestampDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.astimestampDataGridViewTextBoxColumn.Width = 200;
			// 
			// asnumberDataGridViewTextBoxColumn
			// 
			this.asnumberDataGridViewTextBoxColumn.DataPropertyName = "as_number";
			this.asnumberDataGridViewTextBoxColumn.HeaderText = "Number";
			this.asnumberDataGridViewTextBoxColumn.Name = "asnumberDataGridViewTextBoxColumn";
			this.asnumberDataGridViewTextBoxColumn.ReadOnly = true;
			this.asnumberDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.asnumberDataGridViewTextBoxColumn.Width = 200;
			// 
			// MaxStatsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 286);
			this.Controls.Add(this.StatGridView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MaxStatsForm";
			this.Text = "MaxStatsForm";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MaxStatsClosing);
			((System.ComponentModel.ISupportInitialize)(this.StatGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.DataGridView StatGridView;
		private System.Windows.Forms.BindingSource statBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn astimestampDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn asnumberDataGridViewTextBoxColumn;
	}
}
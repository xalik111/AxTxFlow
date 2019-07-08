namespace AuthenticTxFlow
{
    partial class IAPForm
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
			this.iapParameters = new System.Windows.Forms.TabControl();
			this.iapStatus = new System.Windows.Forms.TabPage();
			this.transLog = new System.Windows.Forms.TabPage();
			this.transLogGrid = new System.Windows.Forms.DataGridView();
			this.timestTransLogGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pANDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.responseCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.routeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.transactionBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.iapEvents = new System.Windows.Forms.TabPage();
			this.eventsGrid = new System.Windows.Forms.DataGridView();
			this.eventBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.responseCodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timestEventGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.iapParameters.SuspendLayout();
			this.transLog.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.transLogGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.transactionBindingSource)).BeginInit();
			this.iapEvents.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.eventsGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.eventBindingSource)).BeginInit();
			this.responseCodeContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// iapParameters
			// 
			this.iapParameters.Controls.Add(this.iapStatus);
			this.iapParameters.Controls.Add(this.transLog);
			this.iapParameters.Controls.Add(this.iapEvents);
			this.iapParameters.Location = new System.Drawing.Point(0, 3);
			this.iapParameters.Name = "iapParameters";
			this.iapParameters.SelectedIndex = 0;
			this.iapParameters.Size = new System.Drawing.Size(485, 306);
			this.iapParameters.TabIndex = 0;
			this.iapParameters.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabSelecting);
			// 
			// iapStatus
			// 
			this.iapStatus.AutoScroll = true;
			this.iapStatus.AutoScrollMinSize = new System.Drawing.Size(477, 0);
			this.iapStatus.Location = new System.Drawing.Point(4, 22);
			this.iapStatus.Name = "iapStatus";
			this.iapStatus.Padding = new System.Windows.Forms.Padding(3);
			this.iapStatus.Size = new System.Drawing.Size(477, 280);
			this.iapStatus.TabIndex = 0;
			this.iapStatus.Text = "Status";
			this.iapStatus.UseVisualStyleBackColor = true;
			this.iapStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.iapStatusDraw);
			// 
			// transLog
			// 
			this.transLog.Controls.Add(this.transLogGrid);
			this.transLog.Location = new System.Drawing.Point(4, 22);
			this.transLog.Name = "transLog";
			this.transLog.Padding = new System.Windows.Forms.Padding(3);
			this.transLog.Size = new System.Drawing.Size(477, 280);
			this.transLog.TabIndex = 1;
			this.transLog.Text = "Transaction log";
			this.transLog.UseVisualStyleBackColor = true;
			// 
			// transLogGrid
			// 
			this.transLogGrid.AllowUserToAddRows = false;
			this.transLogGrid.AllowUserToDeleteRows = false;
			this.transLogGrid.AllowUserToResizeColumns = false;
			this.transLogGrid.AllowUserToResizeRows = false;
			this.transLogGrid.AutoGenerateColumns = false;
			this.transLogGrid.BackgroundColor = System.Drawing.Color.White;
			this.transLogGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.transLogGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timestTransLogGridColumn,
            this.pANDataGridViewTextBoxColumn,
            this.responseCodeDataGridViewTextBoxColumn,
            this.routeDataGridViewTextBoxColumn});
			this.transLogGrid.DataSource = this.transactionBindingSource;
			this.transLogGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.transLogGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.transLogGrid.Location = new System.Drawing.Point(3, 3);
			this.transLogGrid.Name = "transLogGrid";
			this.transLogGrid.ReadOnly = true;
			this.transLogGrid.RowHeadersVisible = false;
			this.transLogGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.transLogGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.transLogGrid.Size = new System.Drawing.Size(471, 274);
			this.transLogGrid.TabIndex = 0;
			this.transLogGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.transLogGridCellFormatting);
			// 
			// timestTransLogGridColumn
			// 
			this.timestTransLogGridColumn.DataPropertyName = "Time";
			this.timestTransLogGridColumn.HeaderText = "Timestamp";
			this.timestTransLogGridColumn.Name = "timestTransLogGridColumn";
			this.timestTransLogGridColumn.ReadOnly = true;
			this.timestTransLogGridColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.timestTransLogGridColumn.Width = 130;
			// 
			// pANDataGridViewTextBoxColumn
			// 
			this.pANDataGridViewTextBoxColumn.DataPropertyName = "PAN";
			this.pANDataGridViewTextBoxColumn.HeaderText = "Masked card number";
			this.pANDataGridViewTextBoxColumn.Name = "pANDataGridViewTextBoxColumn";
			this.pANDataGridViewTextBoxColumn.ReadOnly = true;
			this.pANDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.pANDataGridViewTextBoxColumn.Width = 120;
			// 
			// responseCodeDataGridViewTextBoxColumn
			// 
			this.responseCodeDataGridViewTextBoxColumn.DataPropertyName = "ResponseCode";
			this.responseCodeDataGridViewTextBoxColumn.HeaderText = "Response code";
			this.responseCodeDataGridViewTextBoxColumn.Name = "responseCodeDataGridViewTextBoxColumn";
			this.responseCodeDataGridViewTextBoxColumn.ReadOnly = true;
			this.responseCodeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.responseCodeDataGridViewTextBoxColumn.Width = 150;
			// 
			// routeDataGridViewTextBoxColumn
			// 
			this.routeDataGridViewTextBoxColumn.DataPropertyName = "Route";
			this.routeDataGridViewTextBoxColumn.HeaderText = "Route";
			this.routeDataGridViewTextBoxColumn.Name = "routeDataGridViewTextBoxColumn";
			this.routeDataGridViewTextBoxColumn.ReadOnly = true;
			this.routeDataGridViewTextBoxColumn.Width = 80;
			// 
			// transactionBindingSource
			// 
			this.transactionBindingSource.DataSource = typeof(AuthenticTxFlow.Transaction);
			// 
			// iapEvents
			// 
			this.iapEvents.Controls.Add(this.eventsGrid);
			this.iapEvents.Location = new System.Drawing.Point(4, 22);
			this.iapEvents.Name = "iapEvents";
			this.iapEvents.Size = new System.Drawing.Size(477, 280);
			this.iapEvents.TabIndex = 2;
			this.iapEvents.Text = "Events";
			this.iapEvents.UseVisualStyleBackColor = true;
			// 
			// eventsGrid
			// 
			this.eventsGrid.AllowUserToAddRows = false;
			this.eventsGrid.AllowUserToDeleteRows = false;
			this.eventsGrid.AllowUserToResizeColumns = false;
			this.eventsGrid.AllowUserToResizeRows = false;
			this.eventsGrid.AutoGenerateColumns = false;
			this.eventsGrid.BackgroundColor = System.Drawing.Color.White;
			this.eventsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.eventsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timestEventGridColumn,
            this.Message});
			this.eventsGrid.DataSource = this.eventBindingSource;
			this.eventsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.eventsGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.eventsGrid.Location = new System.Drawing.Point(0, 0);
			this.eventsGrid.Name = "eventsGrid";
			this.eventsGrid.ReadOnly = true;
			this.eventsGrid.RowHeadersVisible = false;
			this.eventsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.eventsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.eventsGrid.Size = new System.Drawing.Size(477, 280);
			this.eventsGrid.TabIndex = 1;
			this.eventsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.eventsGrid_CellDoubleClick);
			// 
			// eventBindingSource
			// 
			this.eventBindingSource.DataSource = typeof(AuthenticTxFlow.Event);
			// 
			// responseCodeContextMenu
			// 
			this.responseCodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem,
            this.showErrorsToolStripMenuItem});
			this.responseCodeContextMenu.Name = "contextMenuStrip1";
			this.responseCodeContextMenu.ShowCheckMargin = true;
			this.responseCodeContextMenu.ShowImageMargin = false;
			this.responseCodeContextMenu.Size = new System.Drawing.Size(137, 48);
			// 
			// showAllToolStripMenuItem
			// 
			this.showAllToolStripMenuItem.Checked = true;
			this.showAllToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
			this.showAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.showAllToolStripMenuItem.Text = "Show all";
			this.showAllToolStripMenuItem.Click += new System.EventHandler(this.showAllToolStripMenuItem_Click);
			// 
			// showErrorsToolStripMenuItem
			// 
			this.showErrorsToolStripMenuItem.Name = "showErrorsToolStripMenuItem";
			this.showErrorsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.showErrorsToolStripMenuItem.Text = "Show errors";
			this.showErrorsToolStripMenuItem.Click += new System.EventHandler(this.showErrorsToolStripMenuItem_Click);
			// 
			// timestEventGridColumn
			// 
			this.timestEventGridColumn.DataPropertyName = "Time";
			this.timestEventGridColumn.HeaderText = "Timestamp";
			this.timestEventGridColumn.Name = "timestEventGridColumn";
			this.timestEventGridColumn.ReadOnly = true;
			this.timestEventGridColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.timestEventGridColumn.Width = 130;
			// 
			// Message
			// 
			this.Message.DataPropertyName = "ShortMessage";
			this.Message.HeaderText = "Message";
			this.Message.Name = "Message";
			this.Message.ReadOnly = true;
			this.Message.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Message.Width = 345;
			// 
			// IAPForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 311);
			this.Controls.Add(this.iapParameters);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "IAPForm";
			this.Opacity = 0.95D;
			this.Text = "IAPForm";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IAPFormClosing);
			this.iapParameters.ResumeLayout(false);
			this.transLog.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.transLogGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.transactionBindingSource)).EndInit();
			this.iapEvents.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.eventsGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.eventBindingSource)).EndInit();
			this.responseCodeContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl iapParameters;
        private System.Windows.Forms.TabPage iapStatus;
        private System.Windows.Forms.TabPage transLog;
        private System.Windows.Forms.TabPage iapEvents;
        private System.Windows.Forms.DataGridView transLogGrid;
        private System.Windows.Forms.BindingSource transactionBindingSource;
        private System.Windows.Forms.DataGridView eventsGrid;
        private System.Windows.Forms.BindingSource eventBindingSource;
        private System.Windows.Forms.ContextMenuStrip responseCodeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showErrorsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn timestTransLogGridColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pANDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn responseCodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn routeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn timestEventGridColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Message;
	}
}
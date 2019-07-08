using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	public partial class IAPForm : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static Dictionary<int, string> axResponseCodeName = new Dictionary<int, string>();
		private IAP iap;
		private System.Threading.Timer updateStatus = null;
		private System.Threading.Timer updateEvent = null;
		private Dictionary<string, Node> IAPStatus = new Dictionary<string, Node>();
		private string ProcessName;

		public IAPForm(IAP origin)
		{
			InitializeComponent();
			iap = origin;
			this.Text = iap.Text;
			this.Icon = Properties.Resources.favicon;
			this.timestTransLogGridColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
			this.timestEventGridColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
			this.responseCodeDataGridViewTextBoxColumn.HeaderCell.ContextMenuStrip = responseCodeContextMenu;

			CreateTimer(this.GetStatus, 1000, ref updateStatus);
		}

		private void iapStatusDraw(object sender, PaintEventArgs e)
		{
			e.Graphics.TranslateTransform(iapStatus.AutoScrollPosition.X, iapStatus.AutoScrollPosition.Y);
			int start_y = 8;
			e.Graphics.FillRectangle(TrafficChart.greenBrushes[0], 10, start_y, 20, IAP.rectHeight);
			e.Graphics.DrawString("Success", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), new Point(32, start_y + (IAP.rectHeight - 16) / 2));
			e.Graphics.FillRectangle(TrafficChart.yellowBrush, 150, start_y, 20, IAP.rectHeight);
			e.Graphics.DrawString("No traffic for 5 minutes", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), new Point(172, start_y + (IAP.rectHeight - 16) / 2));
			start_y += IAP.rectHeight + 4;
			e.Graphics.FillRectangle(TrafficChart.redBrushes[0], 10, start_y, 20, IAP.rectHeight);
			e.Graphics.DrawString("Error", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), new Point(32, start_y + (IAP.rectHeight - 16) / 2));
			e.Graphics.FillRectangle(TrafficChart.purpleBrush, 150, start_y, 20, IAP.rectHeight);
			e.Graphics.DrawString("No traffic for 15 minutes", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), new Point(172, start_y + (IAP.rectHeight - 16) / 2));
			start_y += IAP.rectHeight + 8;
			e.Graphics.DrawLine(new Pen(Color.FromArgb(0x99, 0x99, 0x99)), new Point(10, start_y), new Point(iapStatus.Width - 10, start_y));
			start_y += 8;

			int connStatusTextLength = (int)Math.Ceiling(e.Graphics.MeasureString("Connection status: ", new Font("Arial", 9, FontStyle.Regular)).Width);
			int iapStatusTextLength = (int)Math.Ceiling(e.Graphics.MeasureString("IAP Status: ", new Font("Arial", 9, FontStyle.Regular)).Width);
			int nodeTextLength = (int)Math.Ceiling(e.Graphics.MeasureString("Node: ", new Font("Arial", 9, FontStyle.Regular)).Width);
			e.Graphics.DrawString($"Process: {ProcessName}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), new Point(10, start_y));
			start_y += 20;
			foreach (KeyValuePair<string, Node> Node in IAPStatus)
			{
				e.Graphics.DrawString("Node: ", new Font("Arial", 9, FontStyle.Regular), new SolidBrush(Color.Black), new Point(10, start_y));
				e.Graphics.DrawString(Node.Key, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Navy), new Point(10 + nodeTextLength, start_y));
				start_y += 16;
				e.Graphics.DrawString("IAP Status: ", new Font("Arial", 9, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, start_y));
				switch (Node.Value.Status.ToLower())
				{
					case "in-session":
						e.Graphics.DrawString(Node.Value.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x00, 0xD6, 0x00)), new Point(25 + iapStatusTextLength, start_y));
						break;

					case "disconnected":
						e.Graphics.DrawString(Node.Value.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Red), new Point(25 + iapStatusTextLength, start_y));
						break;

					default:
						e.Graphics.DrawString(Node.Value.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.DarkOrange), new Point(25 + iapStatusTextLength, start_y));
						break;
				}
				start_y += 16;
				if (Node.Value.Status == "IN-SESSION")
				{
					foreach (Connection conn in Node.Value.Connections)
					{
						e.Graphics.DrawString($"Connection: {conn.ID}", new Font("Arial", 9, FontStyle.Regular), new SolidBrush(Color.Black), new Point(40, start_y));
						e.Graphics.DrawString("Connection status: ", new Font("Arial", 9, FontStyle.Regular), new SolidBrush(Color.Black), new Point(250, start_y));
						switch (conn.Status.ToLower())
						{
							case "insession":
							case "advicesstarted":
							case "groupsignedon":
							case "signedon":
							case "sessionactivated":
								e.Graphics.DrawString(conn.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(0x00, 0xD6, 0x00)), new Point(250 + connStatusTextLength, start_y));
								break;

							case "disconnected":
							case "overloaded":
							case "notconnected":
							case "signedoff":
							case "groupsignedoff":
							case "sessiondeactivated":
							case "timeoutsoccuring":
								e.Graphics.DrawString(conn.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Red), new Point(250 + connStatusTextLength, start_y));
								break;

							default:
								e.Graphics.DrawString(conn.Status, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.DarkOrange), new Point(250 + connStatusTextLength, start_y));
								break;
						}
						start_y += 16;
						e.Graphics.DrawString(conn.Address, new Font("Arial", 9, FontStyle.Regular), new SolidBrush(Color.Black), new Point(40, start_y));
						start_y += 16;
					}
					start_y += 5;
				}
			}
			this.iapStatus.AutoScrollMinSize = new System.Drawing.Size(this.iapStatus.Width - 30, start_y + 30);
		}

		private void TabSelecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPage == iapStatus)
			{
				DisposeTimer(ref updateEvent);
				CreateTimer(this.GetStatus, 1000, ref updateStatus);
			}
			else if (e.TabPage == transLog)
			{
				DisposeTimer(ref updateStatus);
				DisposeTimer(ref updateEvent);
				GetTrans();
				this.ActiveControl = transLogGrid;
			}
			else if (e.TabPage == iapEvents)
			{
				DisposeTimer(ref updateStatus);
				CreateTimer(this.GetEvent, 1000, ref updateEvent);
			}
		}

		private void transLogGridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (this.transLogGrid.Columns[e.ColumnIndex] == routeDataGridViewTextBoxColumn)
			{
				if (e.Value != null)
				{
					e.Value = e.Value.ToString() == "A" ? "inc." : "out.";
				}
			}
			else if (this.transLogGrid.Columns[e.ColumnIndex] == responseCodeDataGridViewTextBoxColumn)
			{
				if (e.Value != null)
				{
					int code = Convert.ToInt32(e.Value);
					if (axResponseCodeName.ContainsKey(code))
					{
						e.Value = $"{code.ToString()} ({axResponseCodeName[code]})";
					}
				}
			}
		}

		private void IAPFormClosing(object sender, FormClosingEventArgs e)
		{
			iap.IAPParamDisposed(sender, e);
		}

		private void CreateTimer(System.Threading.TimerCallback callback, int interval, ref System.Threading.Timer timer)
		{
			if (timer == null)
			{
				timer = new System.Threading.Timer(callback, null, 0, interval);
			}
		}

		private void DisposeTimer(ref System.Threading.Timer timer)
		{
			if (timer != null)
			{
				timer.Dispose();
				timer = null;
			}
		}

		public void GetTrans()
		{
			List<Transaction> transactionLog = new List<Transaction>();
			DateTime minuteAgo = DateTime.Now.AddMinutes(-10);
			foreach (TrafficChart chart in iap.Charts.Values)
			{
				transactionLog.AddRange(chart.lastTransactions.Where(t => t.Time != default(DateTime)));
			}
			transactionLog = transactionLog.OrderByDescending(t => t.Time).ToList();
			transLogGrid.DataSource = transactionLog.GetRange(0, Math.Min(10000, transactionLog.Count));
		}

		public void GetStatus(object form)
		{
			using (OracleConnection con = new OracleConnection(Database.GetConnURL("ax")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.CommandText = $"SELECT SST_URI_SCHEMA, SST_URI_SSP, SST_STATUS, SST_PROPERTIES FROM {Database.getSchema("ax")}.SYSTEM_STATUS" +
						$" WHERE (SST_URI_SCHEMA = 'iap' OR SST_URI_SCHEMA = 'connection') AND SST_URI_SSP LIKE '%' || :iapname || '%' " +
						"ORDER BY SST_URI_SCHEMA DESC, SST_URI_SSP ASC";

						cmd.Parameters.Add(new OracleParameter(":iapname", iap.name));

						Dictionary<string, Node> status = new Dictionary<string, Node>();
						string ProcName = "";

						using (OracleDataReader data = cmd.ExecuteReader())
						{
							while (data.Read())
							{
								string URI_SCHEMA = data.GetString(0);
								string URI_SSP = data.GetString(1);
								string[] URI_SSP_Splitted = URI_SSP.Split('/');
								string NodeName = URI_SSP_Splitted[1];
								ProcName = URI_SSP_Splitted[2];
								if (URI_SCHEMA == "iap")
								{
									status[NodeName] = new Node
									{
										Status = data.GetString(2),
										Connections = new List<Connection>()
									};
								}
								else if (URI_SCHEMA == "connection")
								{
									if (status.ContainsKey(NodeName))
									{
										string connStatus = data.GetString(2);
										string properties = data.GetString(3);
										string address = "";
										Match matchGroup = new Regex(@"addr=\/(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}),port=(\d{1,5}),localport=(\d{1,5})").Match(properties);
										if (matchGroup.Success)
										{
											address = "Local port: " + matchGroup.Groups[3].Value + "                Remote: " + matchGroup.Groups[1].Value + ":" + matchGroup.Groups[2].Value;
										}
										else
										{
											matchGroup = new Regex(@"connected local=\/(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}) remote=\/(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5})").Match(properties);
											address = matchGroup.Success ? "Local: " + matchGroup.Groups[1].Value + "                Remote: " + matchGroup.Groups[2].Value : "N/A";
										}

										status[NodeName].Connections.Add(
												new Connection { Status = connStatus, Address = address, ID = "#" + URI_SSP.Split('#')[1] }
										);
									}
								}
							}
						}

						try
						{
							if (this.InvokeRequired)
							{
								this.Invoke((MethodInvoker)delegate
								{
									this.IAPStatus = status;
									this.ProcessName = ProcName;
									this.iapStatus.Invalidate();
								});
							}
							else
							{
								this.IAPStatus = status;
								this.ProcessName = ProcName;
								this.iapStatus.Invalidate();
							}
						}
						catch (ObjectDisposedException)
						{
							// ¯\_(ツ)_/¯
						}
					}
					con.Close();
				}
				catch (OracleException ex)
				{
					logger.Fatal(ex);
					Database.MailNotif(ex);
				}
				catch (KeyNotFoundException ex)
				{
					logger.Fatal(ex);
					Database.MailNotif(ex);
				}
			}
		}

		public void GetEvent(object form)
		{
			using (OracleConnection con = new OracleConnection(Database.GetConnURL("ax")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.CommandText = $"SELECT EVE_TIMESTAMP, EVE_SHORT_MSG, EVE_LONG_MSG FROM {Database.getSchema("ax")}.EVENT" +
						" WHERE EVE_IAP_NAME = :iapname AND " +
						"EVE_LAST_UPDATE_TS BETWEEN sysdate - 1 AND sysdate" +
						" ORDER BY EVE_LAST_UPDATE_TS DESC";

						cmd.Parameters.Add(new OracleParameter(":iapname", iap.name));

						List<Event> events = new List<Event>();

						using (OracleDataReader data = cmd.ExecuteReader())
						{
							while (data.Read())
							{
								events.Add(new Event { Time = data.GetDateTime(0), ShortMessage = data.GetString(1), FullMessage = data.IsDBNull(2) ? "" : data.GetString(2) });
							}
						}

						try
						{
							if (this.eventsGrid.InvokeRequired)
							{
								this.Invoke((MethodInvoker)delegate
								{
									int rowIndex = eventsGrid.FirstDisplayedScrollingRowIndex;
									this.eventsGrid.DataSource = events;
									eventsGrid.FirstDisplayedScrollingRowIndex = rowIndex;
								});
							}
							else
							{
								int rowIndex = eventsGrid.FirstDisplayedScrollingRowIndex;
								this.eventsGrid.DataSource = events;
								eventsGrid.FirstDisplayedScrollingRowIndex = rowIndex;
							}
						}
						catch (ObjectDisposedException)
						{
							// ¯\_(ツ)_/¯
						}
						catch (ArgumentOutOfRangeException)
						{
							// ¯\_(ツ)_/¯
						}
					}
					con.Close();
				}
				catch (OracleException ex)
				{
					logger.Fatal(ex);
					Database.MailNotif(ex);
				}
				catch (KeyNotFoundException ex)
				{
					logger.Fatal(ex);
					Database.MailNotif(ex);
				}
			}
		}

		private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showAllToolStripMenuItem.Checked) return;
			showErrorsToolStripMenuItem.Checked = false;
			showAllToolStripMenuItem.Checked = true;

			GetTrans();
		}

		private void showErrorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showErrorsToolStripMenuItem.Checked) return;
			showErrorsToolStripMenuItem.Checked = true;
			showAllToolStripMenuItem.Checked = false;

			List<Transaction> transactionLog = new List<Transaction>();
			DateTime minuteAgo = DateTime.Now.AddMinutes(-10);
			foreach (TrafficChart chart in iap.Charts.Values)
			{
				transactionLog.AddRange(chart.lastTransactions.Where(t => (t.Time != default(DateTime)) && chart.IsFailed(t.ResponseCode)));
			}
			transactionLog = transactionLog.OrderByDescending(t => t.Time).ToList();
			transLogGrid.DataSource = transactionLog.GetRange(0, Math.Min(10000, transactionLog.Count));
		}

		private void eventsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			var eventObject = eventsGrid.Rows[e.RowIndex].DataBoundItem;
			if (eventObject == null)
			{
				return;
			}
			MessageBox.Show(((Event)eventObject).FullMessage, "Long event description", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}

	public class Node
	{
		public string Status { get; set; }
		public List<Connection> Connections { get; set; }
	}

	public class Connection
	{
		public string Status { get; set; }
		public string Address { get; set; }
		public string ID { get; set; }
	}

	public class Event
	{
		public DateTime Time { get; set; }
		public string ShortMessage { get; set; }
		public string FullMessage { get; set; }
	}
}
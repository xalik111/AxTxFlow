using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	public partial class MaxStatsForm : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static MainForm mainForm;
		private char as_type;

		public MaxStatsForm(char type)
		{
			InitializeComponent();
			as_type = type;
			this.Icon = Properties.Resources.favicon;
			switch (type)
			{
				case 'S':
					this.Text = "Records for second";
					this.astimestampDataGridViewTextBoxColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
					break;

				case 'M':
					this.Text = "Records for minute";
					this.astimestampDataGridViewTextBoxColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
					break;

				case 'H':
					this.Text = "Records for hour";
					this.astimestampDataGridViewTextBoxColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
					break;

				case 'D':
					this.Text = "Records for all day";
					this.astimestampDataGridViewTextBoxColumn.DefaultCellStyle.Format = "dd/MM/yyyy";
					break;
			}
			Stats(type);
		}

		public void Stats(char type)
		{
			using (OracleConnection con = new OracleConnection(Database.GetConnURL("app")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						DateTime localDate = DateTime.Now;
						cmd.CommandText = $"SELECT as_timestamp, as_number FROM {Database.getSchema("app")}.AX_STATS" +
						" WHERE AS_TYPE = :as_type AND AS_TIMESTAMP BETWEEN trunc(sysdate - 365) AND trunc(sysdate)";
						cmd.Parameters.Add(new OracleParameter(":as_type", type));
						List<Stat> Maxstat = new List<Stat>();
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							while (data.Read())
							{
								Maxstat.Add(new Stat { as_timestamp = data.GetDateTime(0), as_number = MainForm.beautifyNumber(data.GetInt32(1)), as_type = this.as_type });
							}
						}
						statBindingSource.DataSource = Maxstat.OrderByDescending(o => o.as_timestamp);
					}
				}
				catch (Exception ex)
				{
					logger.Fatal(ex);
					Database.MailNotif(ex);
				}
			}
		}

		private void MaxStatsClosing(object sender, FormClosingEventArgs e)
		{
			mainForm.MaxStatsDisposed(as_type);
		}
	}

	public class Stat
	{
		public DateTime as_timestamp { get; set; }
		public string as_number { get; set; }
		public Char as_type { get; set; }
	}
}
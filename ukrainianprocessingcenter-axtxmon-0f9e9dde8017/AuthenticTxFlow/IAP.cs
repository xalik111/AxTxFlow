using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	public class Transaction
	{
		public int ID { get; set; }
		public string PAN { get; set; }
		public int ResponseCode { get; set; }
		public DateTime Time { get; set; }
		public char Route { get; set; }
	}

	public class TrafficChart
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static MainForm mainForm;

		private IAP ParentIAP;
		private char role;
		private int chartCount;
		public int lastID = Int32.MinValue;
		private int prevID = Int32.MinValue;
		private DateTime lastTime = default(DateTime);
		public List<Transaction> lastTransactions = new List<Transaction>();

		private int moveGreen = 0;

		public static SolidBrush[]
			greenBrushes = {
			new SolidBrush(Color.FromArgb(0x00, 0xB6, 0x00)),
			new SolidBrush(Color.FromArgb(0x00, 0xD6, 0x00)),
			new SolidBrush(Color.FromArgb(0x00, 0xF6, 0x00)),
			new SolidBrush(Color.FromArgb(0x00, 0xD6, 0x00))
		},
			redBrushes = {
			new SolidBrush(Color.FromArgb(0xC6, 0x00, 0x00)),
			new SolidBrush(Color.FromArgb(0xB6, 0x00, 0x00))
		};

		public static SolidBrush
			yellowBrush = new SolidBrush(Color.Yellow),
			purpleBrush = new SolidBrush(Color.Purple)
		;

		public TrafficChart(IAP parentIap, char hostRole)
		{
			ParentIAP = parentIap;
			role = hostRole;
			mainForm = IAP.mainForm;

			if (role == ParentIAP.role)
			{
				chartCount = 1;
			}
			else if (ParentIAP.role == 'B')
			{
				chartCount = (role == 'I') ? 1 : 2;
			}

			for (int i = 0; i < 10; i++)
			{
				lastTransactions.Add(new Transaction { ID = Int32.MinValue, ResponseCode = -1, Time = default(DateTime) });
			}
		}

		public void Draw(Graphics canvas)
		{
			try
			{
				int formWidth = mainForm.ClientRectangle.Width;
				int size_x = (int)(Math.Ceiling(formWidth / (double)IAP.MaxCols) - 80) / 10;
				int start_y = 18 + (IAP.rectHeight + 16 * 3) * (chartCount - 1);

				DateTime timeNow = DateTime.Now;
				timeNow = timeNow.AddTicks(-(timeNow.Ticks % TimeSpan.TicksPerSecond));

				if (lastID != prevID && timeNow <= lastTime)
				{
					if (++moveGreen == 4) moveGreen = 0;
				}

				int freshness = 0;
				if (timeNow.AddMinutes(-15) >= lastTime)
				{
					freshness = 2;
				}
				else if (timeNow.AddMinutes(-5) >= lastTime)
				{
					freshness = 1;
				}
				int x = 60, t_num = 0;
				SolidBrush currentColor;
				if (lastTransactions.Count <= 10)
				{
					if (lastTransactions[9].ResponseCode == -1)
					{
						canvas.FillRectangle(purpleBrush, x, start_y, size_x * 10, IAP.rectHeight);
					}
					else
					{
						foreach (Transaction t in lastTransactions)
						{
							if (t.ResponseCode > -1)
							{
								if (IsFailed(t.ResponseCode))
								{
									currentColor = redBrushes[t_num % 2];
								}
								else
								{
									if (freshness == 2)
									{
										currentColor = purpleBrush;
									}
									else if (freshness == 1)
									{
										currentColor = yellowBrush;
									}
									else
									{
										currentColor = greenBrushes[(4 + t_num - moveGreen) % 4];
									}
								}
								canvas.FillRectangle(currentColor, x, start_y, size_x, IAP.rectHeight);
							}
							t_num++;
							x += size_x;
						}
					}
				}
				else
				{
					t_num = lastTransactions.Count - 10;
					foreach (Transaction t in lastTransactions.GetRange(t_num, 10))
					{
						if (t.ResponseCode > -1)
						{
							if (IsFailed(t.ResponseCode))
							{
								currentColor = redBrushes[t_num % 2];
							}
							else
							{
								if (freshness == 2)
								{
									currentColor = purpleBrush;
								}
								else if (freshness == 1)
								{
									currentColor = yellowBrush;
								}
								else
								{
									currentColor = greenBrushes[(4 + t_num - moveGreen) % 4];
								}
							}
							canvas.FillRectangle(currentColor, x, start_y, size_x, IAP.rectHeight);
						}
						t_num++;
						x += size_x;
					}
				}

				StringFormat sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;
				canvas.DrawString(role == 'A' ? "inc." : "out.", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(Color.White), new Rectangle(10, start_y, 50, IAP.rectHeight), sf);

				timeNow = DateTime.Now;
				string Text;
				int all = lastTransactions.Count(o => o.Time >= timeNow.AddMinutes(-1));
				int errors = lastTransactions.Count(o => o.Time >= timeNow.AddMinutes(-1) && IsFailed(o.ResponseCode));
				if (all > 0)
				{
					Text = "Err./All (%) per min.: " + errors.ToString() + " / " + all.ToString() + " (" + String.Format("{0:0.00}", ((double)errors / (double)all * 100.0)) + "%)";
				}
				else
				{
					Text = "Err./All (%) per min.: " + errors.ToString() + " / " + all.ToString() + " (N/A)";
				}
				canvas.DrawString(Text, new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(10, start_y + IAP.rectHeight));

				all = lastTransactions.Count(o => o.Time >= timeNow.AddHours(-1));
				errors = lastTransactions.Count(o => o.Time >= timeNow.AddHours(-1) && IsFailed(o.ResponseCode));
				if (all > 0)
				{
					Text = "Err./All (%) per hour.: " + errors.ToString() + " / " + all.ToString() + " (" + String.Format("{0:0.00}", ((double)errors / (double)all * 100.0)) + "%)";
				}
				else
				{
					Text = "Err./All (%) per hour.: " + errors.ToString() + " / " + all.ToString() + " (N/A)";
				}
				canvas.DrawString(Text, new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(10, start_y + IAP.rectHeight + 16));

				Text = "Last Transaction: " + (lastTime == default(DateTime) ? "N/A" : lastTime.ToString("HH:mm:ss"));
				canvas.DrawString(Text, new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(10, start_y + IAP.rectHeight + 32));
			}
			catch (Exception ex)
			{
				logger.Fatal(ex);
				Database.MailNotif(ex);
			}
		}

		public bool IsFailed(int response_code)
		{
			return (response_code >= 126 && response_code <= 128) || (response_code >= 900 && response_code <= 999);
		}

		public void Update(List<Transaction> newTransactions)
		{
			lastTransactions.AddRange(newTransactions);
			DateTime hourAgo = DateTime.Now.AddHours(-1);
			int indexHourAgo = lastTransactions.FindLastIndex(o => o.Time <= hourAgo && o.Time != default(DateTime));

			if (indexHourAgo > 9 && indexHourAgo < lastTransactions.Count - 11)
			{
				lastTransactions.RemoveRange(10, indexHourAgo - 9);
			}
			if (lastTransactions.Count > 0)
			{
				prevID = lastID;
				lastID = lastTransactions[lastTransactions.Count - 1].ID;
				lastTime = lastTransactions[lastTransactions.Count - 1].Time;
			}
		}
	}

	public class IAP : GroupBox
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static MainForm mainForm;
		public static int Count;
		public static int MaxCols = 0;
		public static Dictionary<string, IAP> IAPs = new Dictionary<string, IAP>();
		public static Dictionary<int, Dictionary<int, char>> grid = new Dictionary<int, Dictionary<int, char>>();
		public static readonly int rectHeight = 15;

		public static int hostsCount = 0;
		public int order = hostsCount;

		public string name;
		public char role;
		public int row, col;

		public int lastID;

		public Dictionary<char, TrafficChart> Charts = new Dictionary<char, TrafficChart>();

		private IAPForm IAPParamForm = null;

		public IAP(string hostName, char hostRole, int hostRow, int hostCol)
		{
			this.DoubleBuffered = true;
			IAPs[hostName] = this;

			name = hostName;
			role = hostRole;
			row = hostRow;
			col = hostCol;

			if (role == 'B')
			{
				Charts['I'] = new TrafficChart(this, 'I');
				Charts['A'] = new TrafficChart(this, 'A');
			}
			else
			{
				Charts[role] = new TrafficChart(this, role);
			}

			this.Text = name;
			this.Font = new Font("Arial", 10, FontStyle.Bold);
			this.ForeColor = Color.White;
			this.Paint += this.Draw;
			this.MouseDoubleClick += this.Click;
			mainForm.AddControl(this);

			hostsCount++;
		}

		public int CalculateHeight()
		{
			return (rectHeight + 48) * (this.role == 'B' ? 2 : 1) + 20;
		}

		public void Draw(object sender, PaintEventArgs e)
		{
			try
			{
				int formWidth = mainForm.ClientRectangle.Width, formHeight = mainForm.ClientRectangle.Height - 10;
				int x = (formWidth / MaxCols) * (col - 1) + 5, y = 0;

				foreach (var iap in IAPs.Values.Where(o => (o.col == col) && (o.row < row)))
				{
					y += ((IAP)iap).CalculateHeight() + 5;
				}

				foreach (var caption in Caption.Captions.Where(o => (o.col == col) && (o.row < row)))
				{
					y += ((Caption)caption).Height;
				}

				this.Width = formWidth / MaxCols - 10;
				this.Height = CalculateHeight();
				this.Location = new Point(x, y);

				foreach (TrafficChart Chart in Charts.Values)
				{
					Chart.Draw(e.Graphics);
				}
			}
			catch (Exception ex)
			{
				logger.Fatal(ex);
				Database.MailNotif(ex);
			}
		}

		public void Update(List<Transaction> newTransactions)
		{
			foreach (KeyValuePair<char, TrafficChart> Chart in Charts)
			{
				Chart.Value.Update(newTransactions.Where(o => o.Route == Chart.Key).ToList());
			}

			if (newTransactions.Count > 0)
				lastID = newTransactions[newTransactions.Count - 1].ID;
		}

		new public void Click(Object sender, MouseEventArgs e)
		{
			if (IAPParamForm == null)
				IAPParamForm = new IAPForm(this);
			IAPParamForm.Show();
		}

		public void IAPParamDisposed(object sender, EventArgs e)
		{
			IAPParamForm = null;
		}
	}
}
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	public partial class MainForm : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public Graphics graphics;

		public int transForSec;
		public int transForMinute;
		public int transForHour;
		public int transFromMidnight;
		private Database db;
		private System.Threading.Timer UpdateTimer = null;
		private System.Threading.Timer StatsTimer = null;
		private System.Threading.Timer DayStatsTimer = null;
		private System.Threading.Timer MaxStatsTimer = null;
		private MaxStatsForm StatsFormSec = null;
		private MaxStatsForm StatsFormMin = null;
		private MaxStatsForm StatsFormHour = null;
		private MaxStatsForm StatsFormDay = null;

		public MainForm()
		{
			InitializeComponent();
			IAP.mainForm = this;
			Database.mainForm = this;
			Caption.mainForm = this;
			MaxStatsForm.mainForm = this;
		}

		public void AddControl(Control control)
		{
			this.Controls.Add(control);
		}

		private void Resized(object sender, EventArgs e)
		{
			CurrentTimeLabel.Location = new Point(20, this.Height - 65);
			LastTransactionTimeLabel.Location = new Point(170, this.Height - 65);
			PerSecLabel.Location = new Point(400, this.Height - 65);
			PerMinLabel.Location = new Point(550, this.Height - 65);
			PerHourLabel.Location = new Point(730, this.Height - 65);
			FromMidnightLabel.Location = new Point(910, this.Height - 65);
			foreach (var caption in Caption.Captions)
			{
				((Caption)caption).SetPosition();
			}
			Invalidate();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Maximized;
			this.Left = 0;
			this.Top = 0;
			this.MinimumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

			CurrentTimeLabel.Location = new Point(20, this.Height - 65);

			logger.Debug("Программа запущена на " + System.Environment.MachineName + " пользователем " + System.Environment.UserName);

			db = new Database();
			if (IAP.Count > 0)
			{
				this.MinimumSize = new Size(300 * IAP.MaxCols, Screen.PrimaryScreen.Bounds.Height);
				UpdateTimer = new System.Threading.Timer(db.GetTransactions, null, 0, System.Threading.Timeout.Infinite);
				StatsTimer = new System.Threading.Timer(db.GetStatistics, null, 0, System.Threading.Timeout.Infinite);
				DayStatsTimer = new System.Threading.Timer(db.GetPerDay, null, 0, System.Threading.Timeout.Infinite);

				if (System.Environment.UserName == "inform")
				{
					MaxStatsTimer = new System.Threading.Timer(db.MaxStatsInform, null, 0, 1000);
				}
				else
				{
					MaxStatsTimer = new System.Threading.Timer(db.MaxStats, null, 0, 1000);
				}
			}
			else
			{
				logger.Fatal("Таймер мониторинга не запущен. В базе данных отсутствуют IAP либо не установилось подллючение к базе данных.");
			}
		}

		private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (UpdateTimer != null)
			{
				UpdateTimer.Dispose();
			}
			if (StatsTimer != null)
			{
				StatsTimer.Dispose();
			}
			if (DayStatsTimer != null)
			{
				DayStatsTimer.Dispose();
			}
			if (MaxStatsTimer != null)
			{
				MaxStatsTimer.Dispose();
			}

			logger.Debug("Выход из программы на " + System.Environment.MachineName + ", пользователь " + System.Environment.UserName);
		}

		public void GetFromTimer(Dictionary<string, List<Transaction>> TransForIAP)
		{
			Database.firstExecution = false;
			DateTime maxLastTime = DateTime.MinValue;
			foreach (KeyValuePair<string, List<Transaction>> transForIAP in TransForIAP)
			{
				if (IAP.IAPs.ContainsKey(transForIAP.Key))
				{
					if (transForIAP.Value[transForIAP.Value.Count - 1].Time > maxLastTime)
						maxLastTime = transForIAP.Value[transForIAP.Value.Count - 1].Time;
					int lastIndex = transForIAP.Value.FindIndex(o => o.ID == IAP.IAPs[transForIAP.Key].lastID);

					if (lastIndex > -1 && lastIndex + 1 <= transForIAP.Value.Count)
					{
						transForIAP.Value.RemoveRange(0, lastIndex + 1);
					}
					IAP.IAPs[transForIAP.Key].Update(transForIAP.Value);
				}
			}
			foreach (IAP IAP in IAP.IAPs.Values)
			{
				IAP.Invalidate(false);
			}

			UpdateTimer = new System.Threading.Timer(db.GetTransactions, null, 1000, System.Threading.Timeout.Infinite);

			CurrentTimeLabel.Text = $"Current time: {DateTime.Now.ToString("HH:mm:ss")}";
			LastTransactionTimeLabel.Text = $"Last transaction time: {maxLastTime.ToString("HH:mm:ss")}";
			PerSecLabel.Text = $"All per second: {beautifyNumber(transForSec)} (max. {beautifyNumber(db.forSecondMax)})";

			PerMinLabel.Text = $"All per minute: {beautifyNumber(transForMinute)} (max. {beautifyNumber(db.forMinuteMax)})";
			PerHourLabel.Text = $"All per hour: {beautifyNumber(transForHour)} (max. {beautifyNumber(db.forHourMax)})";
			FromMidnightLabel.Text = $"All from midnight: {beautifyNumber(transFromMidnight)}";
		}

		public void GetFromStatsTimer(int forMinute, int forHour, int forSecond)
		{
			transForMinute = forMinute;
			transForHour = forHour;

			transForSec = forSecond;
			StatsTimer = new System.Threading.Timer(db.GetStatistics, null, 1000, System.Threading.Timeout.Infinite);
		}

		public void GetFromStatsTimerDay(int fromMidnight)
		{
			transFromMidnight = fromMidnight;
			DayStatsTimer = new System.Threading.Timer(db.GetPerDay, null, 15000, System.Threading.Timeout.Infinite);
		}

		private void OpenStatFormSec(object sender, MouseEventArgs e)
		{
			if (StatsFormSec == null)
			{
				StatsFormSec = new MaxStatsForm('S');
				StatsFormSec.Show();
			}
		}

		private void OpenStatFormMin(object sender, MouseEventArgs e)
		{
			if (StatsFormMin == null)
				StatsFormMin = new MaxStatsForm('M');
			StatsFormMin.Show();
		}

		private void OpenStatFormHour(object sender, MouseEventArgs e)
		{
			if (StatsFormHour == null)
				StatsFormHour = new MaxStatsForm('H');
			StatsFormHour.Show();
		}

		private void OpenStatFormDay(object sender, MouseEventArgs e)
		{
			if (StatsFormDay == null)
				StatsFormDay = new MaxStatsForm('D');
			StatsFormDay.Show();
		}

		public void MaxStatsDisposed(char type)
		{
			switch (type)
			{
				case 'S':
					StatsFormSec = null;
					break;

				case 'M':
					StatsFormMin = null;
					break;

				case 'H':
					StatsFormHour = null;
					break;

				case 'D':
					StatsFormDay = null;
					break;
			}
		}

		public static string beautifyNumber(int number)
		{
			string result = number.ToString();
			int length = result.Length;
			while ((length -= 3) > 0)
			{
				result = result.Insert(length, " ");
			}
			return result;
		}
	}
}
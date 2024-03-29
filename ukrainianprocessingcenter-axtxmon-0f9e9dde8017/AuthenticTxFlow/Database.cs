using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using config = System.Configuration.ConfigurationManager;

namespace AuthenticTxFlow
{
	internal class Database
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static MainForm mainForm;
		public static bool firstExecution = true;
		public int forSecondMax, forMinuteMax, forHourMax;
		public bool secRowExists = false;
		public bool minRowExists = false;
		public bool hourRowExists = false;
		public string mailException = null;
		public DateTime lastStatUpdate = DateTime.Now.Date;

		public static string getSchema(string cfgPrefix)
		{
			try
			{
				return config.AppSettings[$"{cfgPrefix}_TableSchema"];
			}
			catch (Exception)
			{
				return "UNKNOWN";
			}
		}

		public static void MailNotif(Exception ex)
		{
			try
			{
				MailMessage m = new MailMessage();
				m.From = new MailAddress("mail", "AxTxFlow");
				m.To.Add(new MailAddress("mail"));
				m.Subject = "EXCEPTION!!!";
				m.Body = ex.ToString();
				SmtpClient smtp = new SmtpClient("intranet", 25);

				smtp.Send(m);
			}
			catch (Exception e)
			{
				logger.Fatal(e);
			}
		}

		public static string GetConnURL(string cfgPrefix)
		{
			OracleConnectionStringBuilder conBuilder = new OracleConnectionStringBuilder();
			try
			{
				if (config.AppSettings["useConfig"] == "true")
				{
					conBuilder.DataSource = config.AppSettings[$"{cfgPrefix}_DB_Host"]
						+ ":" + config.AppSettings[$"{cfgPrefix}_DB_Port"] + "/" + config.AppSettings[$"{cfgPrefix}_TableDB"];
					conBuilder.UserID = config.AppSettings["user"];
					conBuilder.Password = config.AppSettings["password"];
				}
				else
				{
					conBuilder.DataSource = "adress";
					conBuilder.UserID = "user";
					conBuilder.Password = "pass";
				}
			}
			catch (ConfigurationErrorsException ex)
			{
				logger.Fatal(ex);
				logger.Info("Используются стандартные реквизиты подключения");
				conBuilder.DataSource = "adress";
				conBuilder.UserID = "user";
				conBuilder.Password = "pass";
			}

			return conBuilder.ToString();
		}

		public Database()
		{
			try
			{
				OracleConnection axConn = new OracleConnection(GetConnURL("ax"));
				OracleConnection appConn = new OracleConnection(GetConnURL("app"));
				OracleConnection trlConn = new OracleConnection(GetConnURL("trl"));
				axConn.Open();
				appConn.Open();

				OracleCommand cmd = new OracleCommand();
				cmd.Connection = axConn;
				cmd.CommandType = System.Data.CommandType.Text;

				// Добавляет в AUTHENTIC_MON IAP'ы которых в ней нет, но есть в INTERCHANGE_ACCESS_POINT
				cmd.CommandText = $"SELECT IAP_NAME FROM {getSchema("ax")}.INTERCHANGE_ACCESS_POINT";
				List<string> AxIap = new List<string>();
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						AxIap.Add(data.GetString(0));
					}
				}
				cmd.Connection = appConn;
				cmd.CommandText = $"SELECT AM_ITEM_TEXT FROM {getSchema("app")}.AUTHENTIC_MON WHERE AM_ITEM_TYPE = 'iap'";
				List<string> AppIap = new List<string>();
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						AppIap.Add(data.GetString(0));
					}
				}
				string[] diffIap = AxIap.Except(AppIap).ToArray();
				if (diffIap.Length != 0)
				{
					cmd.BindByName = true;
					cmd.ArrayBindCount = diffIap.Length;

					cmd.CommandText = $"INSERT INTO {getSchema("app")}.AUTHENTIC_MON (AM_ITEM_TEXT, AM_ITEM_TYPE, AM_IAP_ROLE) VALUES (:iaps, 'iap', 'B')";
					cmd.Parameters.Add(":iaps", OracleDbType.Varchar2);
					cmd.Parameters[":iaps"].Value = diffIap;
					cmd.ExecuteNonQuery();
				}

				cmd.BindByName = false;
				cmd.ArrayBindCount = 0;
				cmd.Parameters.Clear();
				diffIap = AppIap.Except(AxIap).ToArray();
				if (diffIap.Length != 0)
				{
					cmd.CommandText = $"DELETE FROM {getSchema("app")}.AUTHENTIC_MON WHERE AM_ITEM_TYPE = 'iap' AND AM_ITEM_TEXT IN ('{String.Join("','", diffIap)}')";
				}
				cmd.ExecuteNonQuery();

				// Получаем количество всех IAP'ов
				cmd.CommandText = $"SELECT count(1) FROM {getSchema("app")}.AUTHENTIC_MON WHERE AM_ITEM_VISIBLE = 1 AND AM_ITEM_TYPE = 'iap' AND AM_IAP_ROLE IS NOT NULL";
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					data.Read();
					IAP.Count = data.GetInt32(0);
				}

				// Выбираем IAP'ы и создаем для каждого экземпляр
				cmd.CommandText = $"SELECT AM_ITEM_TEXT, AM_IAP_ROLE, AM_ROW, AM_COL FROM {getSchema("app")}" +
				".AUTHENTIC_MON WHERE AM_ITEM_VISIBLE = 1 AND AM_ITEM_TYPE = 'iap' AND AM_IAP_ROLE IS NOT NULL";
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						IAP.MaxCols = Math.Max(IAP.MaxCols, data.IsDBNull(3) ? 0 : data.GetInt32(3));
						new IAP(
							data.GetString(0),
							data.GetString(1)[0],
							data.IsDBNull(2) ? 0 : data.GetInt32(2),
							data.IsDBNull(3) ? 0 : data.GetInt32(3)
						);
					}
				}

				// Выбираем Caption'ы и создаем для каждого экземпляр
				cmd.CommandText = $"SELECT AM_ITEM_TEXT, AM_ROW, AM_COL FROM {getSchema("app")}" +
				".AUTHENTIC_MON WHERE AM_ITEM_VISIBLE = 1 AND AM_ITEM_TYPE = 'caption'";
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						new Caption(
							data.GetString(0),
							data.IsDBNull(1) ? 0 : data.GetInt32(1),
							data.IsDBNull(2) ? 0 : data.GetInt32(2)
						);
					}
				}

				DateTime localDate = DateTime.Now.Date;
				cmd.CommandText = $"SELECT AS_TYPE, AS_NUMBER FROM {getSchema("app")}.AX_STATS " +
				"WHERE AS_TIMESTAMP BETWEEN trunc(sysdate) AND trunc(sysdate+1)-1/24/60/60";
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						switch (data.GetString(0)[0])
						{
							case 'S':
								secRowExists = true;
								forSecondMax = data.GetInt32(1);
								break;

							case 'M':
								minRowExists = true;
								forMinuteMax = data.GetInt32(1);
								break;

							case 'H':
								hourRowExists = true;
								forHourMax = data.GetInt32(1);
								break;
						}
					}
				}

				trlConn.Open();
				cmd.Connection = trlConn;
				cmd.CommandText = $"SELECT ARC_CODE, ARC_NAME FROM {getSchema("trl")}.AUTH_RESULT_CODE";
				using (OracleDataReader data = cmd.ExecuteReader())
				{
					while (data.Read())
					{
						IAPForm.axResponseCodeName.Add(data.GetInt32(0), data.GetString(1));
					}
				}
				cmd.Dispose();
				trlConn.Close();
				axConn.Close();
				appConn.Close();
				axConn.Dispose();
				appConn.Dispose();
				trlConn.Dispose();
			}
			catch (Exception ex)
			{
				logger.Fatal(ex, "Message: ");
				MailNotif(ex);
			}
		}

		public void GetTransactions(object form)
		{
			using (OracleConnection con = new OracleConnection(GetConnURL("trl")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						DateTime localDate = DateTime.Now;
						if (firstExecution)
						{
							cmd.CommandText = $"SELECT TRL_ORIGIN_IAP_NAME, TRL_DEST_IAP_NAME, TRL_ACTION_RESPONSE_CODE, TRL_SYSTEM_TIMESTAMP, TRL_ID, TRL_MASKED_PAN FROM {getSchema("trl")}.TRANSACTION_LOG" +
							" TS WHERE TS.TRL_SYSTEM_TIMESTAMP BETWEEN sysdate-5*1/24 AND sysdate ORDER BY TRL_SYSTEM_TIMESTAMP ASC";
						}
						else
						{
							cmd.CommandText = $"SELECT TRL_ORIGIN_IAP_NAME, TRL_DEST_IAP_NAME, TRL_ACTION_RESPONSE_CODE, TRL_SYSTEM_TIMESTAMP, TRL_ID, TRL_MASKED_PAN FROM {getSchema("trl")}.TRANSACTION_LOG" +
							" TS WHERE TS.TRL_SYSTEM_TIMESTAMP BETWEEN sysdate-5*1/24/60/60 AND sysdate ORDER BY TRL_SYSTEM_TIMESTAMP ASC";
						}

						Dictionary<string, List<Transaction>> TransForIAP = new Dictionary<string, List<Transaction>>();
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							DateTime localDate1 = DateTime.Now;
							int totalmsec = Convert.ToInt32((localDate1 - localDate).TotalMilliseconds);
							if (totalmsec > 500)
							{
								logger.Debug("Too long SQL query in GetTransactions: " + totalmsec + "ms");
							}
							string TRL_OriginIAP, TRL_DestinationIAP;
							int TRL_Response, TRL_ID;
							DateTime SystemTimestamp;

							while (data.Read())
							{
								if (data.IsDBNull(2) || data.IsDBNull(3) || data.IsDBNull(4) || data.IsDBNull(5))
								{
									continue;
								}

								TRL_Response = data.GetInt32(2);
								SystemTimestamp = data.GetDateTime(3);
								TRL_ID = data.GetInt32(4);

								if (!data.IsDBNull(0))
								{
									TRL_OriginIAP = data.GetString(0);
									if (!TransForIAP.ContainsKey(TRL_OriginIAP))
									{
										TransForIAP[TRL_OriginIAP] = new List<Transaction>();
									}
									TransForIAP[TRL_OriginIAP].Add(
										new Transaction { ID = TRL_ID, PAN = data.GetString(5), ResponseCode = TRL_Response, Time = SystemTimestamp, Route = 'A' }
									);
								}

								if (!data.IsDBNull(1))
								{
									TRL_DestinationIAP = data.GetString(1);
									if (!TransForIAP.ContainsKey(TRL_DestinationIAP))
									{
										TransForIAP[TRL_DestinationIAP] = new List<Transaction>();
									}
									TransForIAP[TRL_DestinationIAP].Add(
										new Transaction { ID = TRL_ID, PAN = data.GetString(5), ResponseCode = TRL_Response, Time = SystemTimestamp, Route = 'I' }
									);
								}
							}

							if (mainForm.IsDisposed) return;
							try
							{
								if (mainForm.InvokeRequired)
								{
									mainForm.Invoke((MethodInvoker)delegate
									{
										mainForm.GetFromTimer(TransForIAP);
									});
								}
								else
								{
									mainForm.GetFromTimer(TransForIAP);
								}
							}
							catch (ObjectDisposedException)
							{
								// ¯\_(ツ)_/¯
							}
						}
					}
					con.Close();
				}
				catch (Exception ex)
				{
					logger.Fatal(ex, "Message: ");
					MailNotif(ex);
				}
			}
		}

		public void GetStatistics(object form)
		{
			using (OracleConnection con = new OracleConnection(GetConnURL("trl")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						int forSecond, forHour, forMinute;
						DateTime localDate = DateTime.Now;
						cmd.CommandText = $"SELECT count(1) FROM {getSchema("trl")}.TRANSACTION_LOG" +
						" WHERE TRL_SYSTEM_TIMESTAMP BETWEEN sysdate-1/24/60 AND sysdate";
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							data.Read();
							forMinute = data.GetInt32(0);
						}

						cmd.CommandText = $"SELECT count(1) FROM {getSchema("trl")}.TRANSACTION_LOG" +
						" WHERE TRL_SYSTEM_TIMESTAMP BETWEEN sysdate-1/24 AND sysdate";
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							data.Read();
							forHour = data.GetInt32(0);
						}

						cmd.CommandText = $"SELECT count(1) FROM {getSchema("trl")}.TRANSACTION_LOG" +
						" WHERE TRL_SYSTEM_TIMESTAMP BETWEEN sysdate-1/24/60/60 AND sysdate";
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							data.Read();
							forSecond = data.GetInt32(0);
						}

						if (mainForm.IsDisposed) return;
						try
						{
							if (mainForm.InvokeRequired)
							{
								mainForm.Invoke((MethodInvoker)delegate
								{
									mainForm.GetFromStatsTimer(forMinute, forHour, forSecond);
								});
							}
							else
							{
								mainForm.GetFromStatsTimer(forMinute, forHour, forSecond);
							}
						}
						catch (ObjectDisposedException)
						{
							// ¯\_(ツ)_/¯
						}
					}
					con.Close();
				}
				catch (Exception ex)
				{
					logger.Fatal(ex);
					MailNotif(ex);
					new System.Threading.Timer(GetStatistics, null, 1000, System.Threading.Timeout.Infinite);
				}
			}
		}

		public void GetPerDay(object form)
		{
			using (OracleConnection con = new OracleConnection(GetConnURL("trl")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						int fromMidnight;

						DateTime localDate = DateTime.Now;
						cmd.CommandText = $"SELECT count(1) FROM {getSchema("trl")}.TRANSACTION_LOG WHERE TRL_SYSTEM_TIMESTAMP BETWEEN trunc(sysdate) AND sysdate";
						using (OracleDataReader data = cmd.ExecuteReader())
						{
							data.Read();
							fromMidnight = data.GetInt32(0);
						}
						DateTime localDate1 = DateTime.Now;
						int totalmsec = Convert.ToInt32((localDate1 - localDate).TotalMilliseconds);
						if (totalmsec > 10000)
						{
							logger.Debug("Sql query for 'All from midnight' = " + totalmsec + "ms");
						}
						if (mainForm.IsDisposed) return;
						try
						{
							if (mainForm.InvokeRequired)
							{
								mainForm.Invoke((MethodInvoker)delegate
								{
									mainForm.GetFromStatsTimerDay(fromMidnight);
								});
							}
							else
							{
								mainForm.GetFromStatsTimerDay(fromMidnight);
							}
						}
						catch (ObjectDisposedException)
						{
							// ¯\_(ツ)_/¯
						}
					}
					con.Close();
				}
				catch (Exception ex)
				{
					logger.Fatal(ex);
					MailNotif(ex);
					new System.Threading.Timer(GetPerDay, null, 15000, System.Threading.Timeout.Infinite);
				}
			}
		}

		public void MaxStatsInform(object form)
		{
			using (OracleConnection con = new OracleConnection(GetConnURL("app")))
			{
				try
				{
					con.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = con;
						cmd.CommandType = System.Data.CommandType.Text;
						DateTime localDate = DateTime.Now;

						if (lastStatUpdate < localDate.Date)
						{
							lastStatUpdate = localDate.Date;
							secRowExists = false;
							minRowExists = false;
							hourRowExists = false;
							forSecondMax = 0;
							forMinuteMax = 0;
							forHourMax = 0;
							int lastDayCount;

							cmd.CommandText = $"SELECT count(1) FROM {getSchema("trl")}.TRANSACTION_LOG WHERE TRL_SYSTEM_TIMESTAMP BETWEEN trunc(sysdate-1) AND trunc(sysdate)";
							using (OracleDataReader data = cmd.ExecuteReader())
							{
								data.Read();
								lastDayCount = data.GetInt32(0);
							}

							cmd.CommandText = $"INSERT INTO {getSchema("app")}.AX_STATS VALUES (:as_date, :as_number, :as_type)";
							cmd.Parameters.Add(new OracleParameter(":as_date", localDate.AddMinutes(-5)));
							cmd.Parameters.Add(new OracleParameter(":as_number", lastDayCount));
							cmd.Parameters.Add(new OracleParameter(":as_type", 'D'));
							cmd.ExecuteNonQuery();
						}
						cmd.Parameters.Clear();

						if (mainForm.transForSec > forSecondMax)
						{
							forSecondMax = mainForm.transForSec;

							if (secRowExists)
							{
								cmd.CommandText = $"UPDATE {getSchema("app")}.AX_STATS SET AS_NUMBER = :as_number," +
								"AS_TIMESTAMP = :as_date " +
								"WHERE (AS_TIMESTAMP >= trunc(sysdate) AND " +
								"AS_TIMESTAMP < trunc(sysdate+1)-1/24/60/60) AND AS_TYPE = :as_type";
								cmd.Parameters.Add(new OracleParameter(":as_number", forSecondMax));
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'S'));
								cmd.ExecuteNonQuery();
							}
							else
							{
								cmd.CommandText = $"INSERT INTO {getSchema("app")}.AX_STATS VALUES (:as_date, :as_number, :as_type)";
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_number", forSecondMax));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'S'));
								cmd.ExecuteNonQuery();
								secRowExists = true;
							}
						}
						cmd.Parameters.Clear();
						if (mainForm.transForMinute > forMinuteMax)
						{
							forMinuteMax = mainForm.transForMinute;

							if (minRowExists)
							{
								cmd.CommandText = $"UPDATE {getSchema("app")}.AX_STATS SET AS_NUMBER = :as_number," +
								"AS_TIMESTAMP = :as_date " +
								"WHERE (AS_TIMESTAMP >= trunc(sysdate) AND " +
								"AS_TIMESTAMP < trunc(sysdate+1)-1/24/60/60) AND AS_TYPE = :as_type";
								cmd.Parameters.Add(new OracleParameter(":as_number", forMinuteMax));
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'M'));
								cmd.ExecuteNonQuery();
							}
							else
							{
								cmd.CommandText = $"INSERT INTO {getSchema("app")}.AX_STATS VALUES (:as_date, :as_number, :as_type)";
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_number", forMinuteMax));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'M'));
								cmd.ExecuteNonQuery();
								minRowExists = true;
							}
						}
						cmd.Parameters.Clear();
						if (mainForm.transForHour > forHourMax)
						{
							forHourMax = mainForm.transForHour;

							if (hourRowExists)
							{
								cmd.CommandText = $"UPDATE {getSchema("app")}.AX_STATS SET AS_NUMBER = :as_number," +
								"AS_TIMESTAMP = :as_date " +
								"WHERE (AS_TIMESTAMP >= trunc(sysdate) AND " +
								"AS_TIMESTAMP < trunc(sysdate+1)-1/24/60/60) AND AS_TYPE = :as_type";
								cmd.Parameters.Add(new OracleParameter(":as_number", forHourMax));
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'H'));
								cmd.ExecuteNonQuery();
							}
							else
							{
								cmd.CommandText = $"INSERT INTO {getSchema("app")}.AX_STATS VALUES (:as_date, :as_number, :as_type)";
								cmd.Parameters.Add(new OracleParameter(":as_date", localDate));
								cmd.Parameters.Add(new OracleParameter(":as_number", forHourMax));
								cmd.Parameters.Add(new OracleParameter(":as_type", 'H'));
								cmd.ExecuteNonQuery();
								hourRowExists = true;
							}
						}
					}
				}
				catch (Exception ex)
				{
					logger.Fatal(ex);
					MailNotif(ex);
					new System.Threading.Timer(MaxStatsInform, null, 1000, System.Threading.Timeout.Infinite);
				}
			}
		}

		public void MaxStats(object form)
		{
			DateTime localDate = DateTime.Now;

			if (lastStatUpdate < localDate.Date)
			{
				forSecondMax = 0;
				forMinuteMax = 0;
				forHourMax = 0;
				lastStatUpdate = localDate.Date;
			}

			if (mainForm.transForSec > forSecondMax)
			{
				forSecondMax = mainForm.transForSec;
			}

			if (mainForm.transForMinute > forMinuteMax)
			{
				forMinuteMax = mainForm.transForMinute;
			}

			if (mainForm.transForHour > forHourMax)
			{
				forHourMax = mainForm.transForHour;
			}
		}
	}
}
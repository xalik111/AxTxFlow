using System;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	internal static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form mainForm = new MainForm();
			Application.Run(mainForm);
		}
	}
}
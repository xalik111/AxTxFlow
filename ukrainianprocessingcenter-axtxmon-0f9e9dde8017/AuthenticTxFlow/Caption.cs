using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AuthenticTxFlow
{
	public partial class Caption : UserControl
	{
		public Caption(string captionText, int captionRow, int captionCol)
		{
			InitializeComponent();
			this.captionLabel.Text = captionText;
			this.row = captionRow;
			this.col = captionCol;
			Captions.Add(this);
			mainForm.Controls.Add(this);
			SetPosition();
		}

		public void SetPosition()
		{
			int formWidth = mainForm.ClientRectangle.Width, formHeight = mainForm.ClientRectangle.Height - 10;
			int x = formWidth / IAP.MaxCols * (col - 1), y = 0;

			foreach (var iap in IAP.IAPs.Values.Where(o => (o.col == col) && (o.row < row)))
			{
				y += ((IAP)iap).CalculateHeight() + 5;
			}

			foreach (var caption in Captions.Where(o => (o.col == col) && (o.row < row)))
			{
				y += ((Caption)caption).Height;
			}

			this.Width = formWidth / IAP.MaxCols;
			this.captionLabel.Width = this.Width;
			this.Location = new Point(x, y);
		}

		public static List<Caption> Captions = new List<Caption>();
		public static MainForm mainForm;
		public int row, col;
	}
}
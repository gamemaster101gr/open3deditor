using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Open3DEditor.Win
{
	public partial class GridResizer : UserControl
	{
		private readonly TableLayoutPanel _viewsGrid;
		private readonly bool _vertical;
		private Point _lastKnownMousePos;
		private int[] originalSizes;

		public GridResizer(TableLayoutPanel viewsGrid, bool vertical)
		{
			_viewsGrid = viewsGrid;
			_vertical = vertical;
			Dock = DockStyle.Fill;
			Cursor = vertical?Cursors.VSplit:Cursors.HSplit;
			InitializeComponent();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Capture = true;
			_lastKnownMousePos = e.Location;
			if (_vertical)
			{
				originalSizes = _viewsGrid.GetColumnWidths();
			}
			else
			{
				originalSizes = _viewsGrid.GetRowHeights();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!Capture)
				return;
			ResizeGrid(e.Location);
			base.OnMouseMove(e);
		}

		private void ResizeGrid(Point e)
		{
			var mousePos = e;
			if (_vertical)
			{
				ResizeVertical(mousePos.X - _lastKnownMousePos.X);
			}
			else
			{
				ResizeHorizontal(mousePos.Y - _lastKnownMousePos.Y);
			}
			//_lastKnownMousePos = mousePos;
		}

		private void ResizeHorizontal(int delta)
		{
			if (delta == 0)
				return;

			var w = _viewsGrid.GetRowHeights();
			if (originalSizes.Length != 3)
				return;
			var totalW = w[0] + w[2];
			float width0 = (w[0] + delta)*100.0f/totalW;
			float width2 = (w[2] - delta) * 100.0f / totalW;
			//Debug.WriteLine(string.Format("{0}-{1}-{2}  ->   {3} {4}", w[0], w[1], w[2], width0, width2));
			if (width0 > 0 && width2 > 0)
			{
				_viewsGrid.RowStyles[0] = new RowStyle(SizeType.Percent, width0);
				_viewsGrid.RowStyles[2] = new RowStyle(SizeType.Percent, width2);
			}
		}

		private void ResizeVertical(int delta)
		{
			if (delta == 0)
				return;
			var w = _viewsGrid.GetColumnWidths();
			if (originalSizes.Length != 3)
				return;
			var totalW = w[0] + w[2];
			float width0 = (w[0] + delta)*100.0f/totalW;
			float width2 = (w[2] - delta) * 100.0f / totalW;
			//Debug.WriteLine(string.Format("{0}-{1}-{2}  ->   {3} {4}", w[0], w[1], w[2], width0, width2));
			if (width0 > 0 && width2 > 0)
			{
				_viewsGrid.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, width0);
				_viewsGrid.ColumnStyles[2] = new ColumnStyle(SizeType.Percent, width2);
			}

		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			ResizeGrid(e.Location);
			Capture = false;
			base.OnMouseUp(e);
		}
	}
}

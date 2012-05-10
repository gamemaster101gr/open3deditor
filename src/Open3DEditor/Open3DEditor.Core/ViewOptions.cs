using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Open3DEditor.Core
{
	/// <summary>
	/// Viewport configuration
	/// </summary>
	public class ViewOptions: INotifyPropertyChanged
	{
		private Color _background = Color.Gray;
		public Color Background
		{
			get { return _background; }
			set { _background = value; }
		}

		protected void RisePropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, args);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

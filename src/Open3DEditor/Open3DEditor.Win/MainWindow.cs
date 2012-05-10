using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Open3DEditor.Core;

namespace Open3DEditor.Win
{
	public partial class MainWindow : Form, IApplicationWindow
	{
		private readonly IComponentContext _context;
		private readonly List<IView> _activeViews = new List<IView>();

		protected override void OnLoad(EventArgs e)
		{
			this.viewsGrid.Controls.Add(new GridResizer(this.viewsGrid,true), 1, 0);
			this.viewsGrid.Controls.Add(new GridResizer(this.viewsGrid, true), 1, 2);
			this.viewsGrid.Controls.Add(new GridResizer(this.viewsGrid,false), 0, 1);
			this.viewsGrid.Controls.Add(new GridResizer(this.viewsGrid, false), 2, 1);

			IView view;
			view = _context.Resolve<IView>(new[] {TypedParameter.From(new ViewOptions())});
			_activeViews.Add(view);
			this.viewsGrid.Controls.Add(_activeViews.Last().GetControl(), 0, 0);

			view = _context.Resolve<IView>(new[] { TypedParameter.From(new ViewOptions()) });
			_activeViews.Add(view);
			this.viewsGrid.Controls.Add(_activeViews.Last().GetControl(), 2, 0);

			view = _context.Resolve<IView>(new[] { TypedParameter.From(new ViewOptions()) });
			_activeViews.Add(view);
			this.viewsGrid.Controls.Add(_activeViews.Last().GetControl(), 0, 2);

			view = _context.Resolve<IView>(new[] { TypedParameter.From(new ViewOptions()) });
			_activeViews.Add(view);
			this.viewsGrid.Controls.Add(_activeViews.Last().GetControl(), 2, 2);
			base.OnLoad(e);
		}

		public MainWindow(Autofac.IComponentContext context)
		{
			_context = context;
			InitializeComponent();
		}

		private void CloseApp(object sender, EventArgs e)
		{
			this.Close();
		}

		public IntPtr GetHandle()
		{
			return this.Handle;
		}
	}
}

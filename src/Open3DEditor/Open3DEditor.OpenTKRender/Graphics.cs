using System;
using Open3DEditor.Core;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace Open3DEditor.OpenTKRender
{
	public class Graphics: IGraphics
	{
		private readonly IApplicationWindow _window;
		private GraphicsContext _graphicsContext;
		private IWindowInfo wi;

		public Graphics(IApplicationWindow window)
		{
			_window = window;
			IntPtr handle = _window.GetHandle();
			wi = GetWindowInfo(handle);
			//GraphicsMode graphicsMode = GraphicsMode.Default;
			GraphicsMode graphicsMode = new GraphicsMode(new ColorFormat(32), 16);
			_graphicsContext = new GraphicsContext(
				graphicsMode, 
				wi);
		}
		public GraphicsContext GraphicsContext
		{
			get { return _graphicsContext; }
		}

		public GraphicsMode Mode
		{
			get { return _graphicsContext.GraphicsMode; }
		}

		public IWindowInfo GetWindowInfo(IntPtr handle)
		{
			IWindowInfo wi = null;
			if (OpenTK.Configuration.RunningOnWindows)
				wi = Utilities.CreateWindowsWindowInfo(handle);
			else if (Configuration.RunningOnX11)
			{
				throw new NotSupportedException();
				//wi = Utilities.CreateX11WindowInfo(display, screen, handle, root, visual);
			}
			else if (Configuration.RunningOnMacOS)
				wi = Utilities.CreateMacOSCarbonWindowInfo(handle, false, false);
			return wi;
		}

		public void MakeCurrent()
		{
			_graphicsContext.MakeCurrent(wi);
		}
	}
}
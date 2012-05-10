using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Open3DEditor.Core;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Open3DEditor.OpenTKRender
{
	public class View : IView
	{
		private readonly ViewOptions _options;
		private readonly Graphics _graphics;
		private ViewControl _control;

		public View (IGraphics graphics, ViewOptions options)
		{
			_options = options;
			_options.PropertyChanged += OnViewPropertyChanged;
			_graphics = graphics as Graphics;
		}

		private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			
		}

		public Control GetControl()
		{
			//if (Configuration.RunningOnWindows) return new WinGLControl(GraphicsMode.Default, control);
			//else if (Configuration.RunningOnMacOS) return new CarbonGLControl(mode, control);
			//else if (Configuration.RunningOnX11) return new X11GLControl(mode, control);
			//else throw new PlatformNotSupportedException();

			if (_control == null)
			{
				_control = new ViewControl(_graphics);
				_control.Paint += OnPaint;
			}
			return _control;
		}
		private void OnPaint(object sender, PaintEventArgs e)
		{
			try
			{
				MakeCurrent();
				GL.ClearColor(_options.Background);
				GL.Clear(ClearBufferMask.ColorBufferBit);
				Flush();
				SwapBuffers();
			} 
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void Flush()
		{
			GL.Flush();
		}

		public void SwapBuffers()
		{
			_graphics.GraphicsContext.SwapBuffers();
		}

		public void MakeCurrent()
		{
			_control.MakeCurrent();
		}
	}
}

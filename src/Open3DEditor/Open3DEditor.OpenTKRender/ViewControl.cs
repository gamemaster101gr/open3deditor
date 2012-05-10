using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Open3DEditor.OpenTKRender
{
	public partial class ViewControl : UserControl
	{
		private readonly Graphics _graphics;
		IGLControl implementation;
		bool? initial_vsync_value;
		bool resize_event_suppressed;
		readonly bool design_mode;
		private IGraphicsContext _dummyContext;

		public ViewControl(Graphics graphics)
		{
			_graphics = graphics;

			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			Dock = DockStyle.Fill;
			DoubleBuffered = false;
			design_mode =
				DesignMode ||
				LicenseManager.UsageMode == LicenseUsageMode.Designtime;
			InitializeComponent();
		}

		public GraphicsContext Context
		{
			get
			{
				if (_graphics == null)
					return null;
				return _graphics.GraphicsContext;
			}
		}

		/// <summary>Raises the HandleCreated event.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			if (_dummyContext != null)
				_dummyContext.Dispose();

			if (implementation != null)
				implementation.WindowInfo.Dispose();

			if (Configuration.RunningOnWindows) 
				implementation = new WinGLControl(_graphics.Mode, this);
			//else if (Configuration.RunningOnMacOS) return new CarbonGLControl(mode, control);
			//else if (Configuration.RunningOnX11) return new X11GLControl(mode, control);
			else throw new PlatformNotSupportedException();

			_dummyContext = implementation.CreateContext(1, 1, GraphicsContextFlags.Default);
			MakeCurrent();

			if (!design_mode)
			    ((IGraphicsContextInternal)Context).LoadAll();

			// Deferred setting of vsync mode. See VSync property for more information.
			if (initial_vsync_value.HasValue)
			{
				_graphics.GraphicsContext.SwapInterval = initial_vsync_value.Value ? 1 : 0;
				initial_vsync_value = null;
			}

			base.OnHandleCreated(e);

			if (resize_event_suppressed)
			{
				OnResize(EventArgs.Empty);
				resize_event_suppressed = false;
			}
		}
		IGLControl Implementation
		{
			get
			{
				ValidateState();

				return implementation;
			}
		}
		void ValidateState()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().Name);

			if (!IsHandleCreated)
				CreateControl();

			if (implementation == null || Context == null || Context.IsDisposed)
				RecreateHandle();
		}


		/// <summary>Raises the HandleDestroyed event.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (_dummyContext != null)
			{
				_dummyContext.Dispose();
				_dummyContext = null;
			}

			if (implementation != null)
			{
				implementation.WindowInfo.Dispose();
				implementation = null;
			}

			base.OnHandleDestroyed(e);
		}

		/// <summary>
		/// Raises the System.Windows.Forms.Control.Paint event.
		/// </summary>
		/// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			ValidateState();

			if (design_mode)
				e.Graphics.Clear(BackColor);

			base.OnPaint(e);
		}

		/// <summary>
		/// Raises the Resize event.
		/// Note: this method may be called before the OpenGL context is ready.
		/// Check that IsHandleCreated is true before using any OpenGL methods.
		/// </summary>
		/// <param name="e">A System.EventArgs that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			// Do not raise OnResize event before the handle and context are created.
			if (!IsHandleCreated)
			{
				resize_event_suppressed = true;
				return;
			}

			if (Context != null)
				Context.Update(Implementation.WindowInfo);

			base.OnResize(e);
		}

		/// <summary>
		/// Raises the ParentChanged event.
		/// </summary>
		/// <param name="e">A System.EventArgs that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			if (Context != null)
				Context.Update(Implementation.WindowInfo);

			base.OnParentChanged(e);
		}

		/// <summary>
		/// Swaps the front and back buffers, presenting the rendered scene to the screen.
		/// </summary>
		public void SwapBuffers()
		{
			ValidateState();
			Context.SwapBuffers();
		}

		/// <summary>
		/// Makes the underlying this GLControl current in the calling thread.
		/// All OpenGL commands issued are hereafter interpreted by this GLControl.
		/// </summary>
		public void MakeCurrent()
		{
			ValidateState();
			Context.MakeCurrent(Implementation.WindowInfo);
		}

		/// <summary>
		/// Gets a value indicating whether the current thread contains pending system messages.
		/// </summary>
		[Browsable(false)]
		public bool IsIdle
		{
			get
			{
				ValidateState();
				return Implementation.IsIdle;
			}
		}
		/// <summary>
		/// Gets the aspect ratio of this GLControl.
		/// </summary>
		[Description("The aspect ratio of the client area of this GLControl.")]
		public float AspectRatio
		{
			get
			{
				ValidateState();
				return ClientSize.Width / (float)ClientSize.Height;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether vsync is active for this GLControl.
		/// </summary>
		[Description("Indicates whether GLControl updates are synced to the monitor's refresh rate.")]
		public bool VSync
		{
			get
			{
				if (!IsHandleCreated)
					return false;

				ValidateState();
				return Context.VSync;
			}
			set
			{
				// The winforms designer sets this to false by default which forces control creation.
				// However, events are typically connected after the VSync = false assignment, which
				// can lead to "event xyz is not fired" issues.
				// Work around this issue by deferring VSync mode setting to the HandleCreated event.
				if (!IsHandleCreated)
				{
					initial_vsync_value = value;
					return;
				}

				ValidateState();
				Context.VSync = value;
			}
		}


		/// <summary>
		/// Gets the GraphicsMode of the GraphicsContext attached to this GLControl.
		/// </summary>
		/// <remarks>
		/// To change the GraphicsMode, you must destroy and recreate the GLControl.
		/// </remarks>
		public GraphicsMode GraphicsMode
		{
			get
			{
				ValidateState();
				return Context.GraphicsMode;
			}
		}
		/// <summary>
		/// Gets the <see cref="OpenTK.Platform.IWindowInfo"/> for this instance.
		/// </summary>
		public IWindowInfo WindowInfo
		{
			get { return implementation.WindowInfo; }
		}

		/// <summary>Grabs a screenshot of the frontbuffer contents.</summary>
		/// <returns>A System.Drawing.Bitmap, containing the contents of the frontbuffer.</returns>
		/// <exception cref="OpenTK.Graphics.GraphicsContextException">
		/// Occurs when no OpenTK.Graphics.GraphicsContext is current in the calling thread.
		/// </exception>
		[Obsolete("This method will not work correctly with OpenGL|ES. Please use GL.ReadPixels to capture the contents of the framebuffer (refer to http://www.opentk.com/doc/graphics/save-opengl-rendering-to-disk for more information).")]
		public Bitmap GrabScreenshot()
		{
			ValidateState();

			Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			System.Drawing.Imaging.BitmapData data =
				bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
							 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte,
						  data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return bmp;
		}
	}
}

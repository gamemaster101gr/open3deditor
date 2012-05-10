using Open3DEditor.Core;
using OpenTK.Graphics.OpenGL;

namespace Open3DEditor.OpenTKRender
{
	public class VertexBuffer : IVertexBuffer
	{
		private Graphics _graphics;
		private uint id;

		~VertexBuffer()
		{
			Dispose(false);
		}

		public VertexBuffer(IGraphics graphics)
		{
			_graphics = graphics as Graphics;

			_graphics.MakeCurrent();
			GL.GenBuffers(1, out id);
		}


		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool b)
		{
			if (id != 0)
			{
				GL.DeleteBuffers(1,ref id);
				id = 0;
			}
		}
	}
}
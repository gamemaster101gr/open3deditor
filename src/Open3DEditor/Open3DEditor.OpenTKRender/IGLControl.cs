using OpenTK.Graphics;
using OpenTK.Platform;

namespace Open3DEditor.OpenTKRender
{
	internal interface IGLControl
	{
		IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags);
		bool IsIdle { get; }
		IWindowInfo WindowInfo { get; }
	}
}
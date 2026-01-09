using DoodleJump.Rendering;

namespace DoodleJump.Objects
{
	internal class BouncePlatform : Platform
	{
		public BouncePlatform(SpriteSheet visualization) : base(visualization)
		{
			BounceForce = 40f;
		}
	}
}

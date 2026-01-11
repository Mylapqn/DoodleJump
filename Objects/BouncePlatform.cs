using DoodleJump.Core;
using DoodleJump.Rendering;

namespace DoodleJump.Objects
{
	internal class BouncePlatform : Platform
	{
		public BouncePlatform(SpriteSheet visualization) : base(visualization)
		{
			BounceForce = 40f;
		}
		internal override void PlayBounceSound()
		{
			GameSettings.Assets.Sounds["jump"].Play(.3f, .5f, 0f);	
			base.PlayBounceSound();
		}
	}
}

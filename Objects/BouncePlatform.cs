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
			GameSettings.Assets.Sounds["jump"].Play(.6f, .8f, 0f);	
			base.PlayBounceSound();
		}
	}
}

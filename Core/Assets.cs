using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	public class Assets
	{
		Game1 game;
		public Dictionary<string, Texture2D> Textures { get; private set; } = [];
		public Dictionary<string, SpriteFont> Fonts { get; private set; } = [];
		public Dictionary<string, SoundEffect> Sounds { get; private set; } = [];
		public Assets(Game1 game)
		{
			this.game = game;
		}
		public void LoadAsset<T>(string path, string name)
		{
			if (!path.EndsWith("/"))
			{
				path += "/";
			}
			T asset = game.Content.Load<T>(path+name);
			if (typeof(T) == typeof(Texture2D))
			{
				Textures[name] = asset as Texture2D;
			}
			else if (typeof(T) == typeof(SpriteFont))
			{
				Fonts[name] = asset as SpriteFont;
			}
			else if (typeof(T) == typeof(SoundEffect))
			{
				Sounds[name] = asset as SoundEffect;
			}
			else
			{
				Debug.WriteLine($"[Assets] Unsupported asset type: {typeof(T)}");
			}
		}
		public void LoadAllAssets()
		{
			LoadAsset<Texture2D>("textures/sprites", "fire_circles_100x100");
		}
	}
}

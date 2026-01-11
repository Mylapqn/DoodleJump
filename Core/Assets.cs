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
		public Dictionary<string, Texture2D> Textures { get; private set; } = [];
		public Dictionary<string, SpriteFont> Fonts { get; private set; } = [];
		public Dictionary<string, SoundEffect> Sounds { get; private set; } = [];
		public Assets()
		{
		}
		public void LoadAsset<T>(string path, string name)
		{
			if (!path.EndsWith("/"))
			{
				path += "/";
			}
			T asset = Game1.Instance.Content.Load<T>(path+name);
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
				Debug.WriteLine($"Unsupported asset type: {typeof(T)}");
			}
		}
		public void LoadAllAssets()
		{
			LoadAsset<Texture2D>("textures/sprites", "platform");
			LoadAsset<Texture2D>("textures/sprites", "platform_bounce");
			LoadAsset<Texture2D>("textures/sprites", "platform_moving");
			LoadAsset<Texture2D>("textures/sprites", "cat_jump");
			LoadAsset<Texture2D>("textures/sprites", "vignette");
			LoadAsset<Texture2D>("textures/sprites", "pixel");
			LoadAsset<SpriteFont>("fonts", "default_font");
			LoadAsset<Texture2D>("textures/background", "fg_city");
			LoadAsset<Texture2D>("textures/background", "bg_city_1");
			LoadAsset<Texture2D>("textures/background", "bg_city_2");
			LoadAsset<Texture2D>("textures/background", "bg_sky");
			LoadAsset<SoundEffect>("sounds", "jump");
			LoadAsset<SoundEffect>("sounds", "meow");
			LoadAsset<SoundEffect>("sounds", "jump2");
			LoadAsset<SoundEffect>("sounds", "brick1");
			LoadAsset<SoundEffect>("sounds", "brick2");
			LoadAsset<SoundEffect>("sounds", "success");
			LoadAsset<SoundEffect>("sounds", "fail");
		}
	}
}

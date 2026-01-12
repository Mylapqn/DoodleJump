using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	internal class Assets
	{
		public Dictionary<string, Texture2D> Textures { get; private set; } = [];
		public Dictionary<string, SpriteFont> Fonts { get; private set; } = [];
		public Dictionary<string, SoundEffect> Sounds { get; private set; } = [];
		public Dictionary<string, Song> Songs { get; private set; } = [];
		private ContentManager content;
		public Assets(ContentManager content)
		{
			this.content = content;
		}
		public void LoadAsset<T>(string path, string name)
		{
			if (!path.EndsWith("/"))
			{
				path += "/";
			}
			T asset = content.Load<T>(path + name);
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
			else if (typeof(T) == typeof(Song))
			{
				Songs[name] = asset as Song;
			}
			else
			{
				Debug.WriteLine($"Unsupported asset type: {typeof(T)}");
			}
		}
		public void LoadAllAssets()
		{
			LoadAsset<Texture2D>("textures/sprites", "platform");
			LoadAsset<Texture2D>("textures/sprites", "platform_ac");
			LoadAsset<Texture2D>("textures/sprites", "platform_bounce");
			LoadAsset<Texture2D>("textures/sprites", "platform_moving");
			LoadAsset<Texture2D>("textures/sprites", "platform_cloud");
			LoadAsset<Texture2D>("textures/sprites", "platform_cloud_bounce");
			LoadAsset<Texture2D>("textures/sprites", "platform_cloud_move");
			LoadAsset<Texture2D>("textures/sprites", "cat_jump");
			LoadAsset<Texture2D>("textures/sprites", "vignette");
			LoadAsset<Texture2D>("textures/sprites", "pixel");
			LoadAsset<SpriteFont>("fonts", "default_font");
			LoadAsset<Texture2D>("textures/background", "bg_city_0");
			LoadAsset<Texture2D>("textures/background", "bg_city_1");
			LoadAsset<Texture2D>("textures/background", "bg_city_2");
			LoadAsset<Texture2D>("textures/background", "bg_city_3");
			LoadAsset<Texture2D>("textures/background", "bg_sky");
			LoadAsset<SoundEffect>("sounds", "jump");
			LoadAsset<SoundEffect>("sounds", "meow");
			LoadAsset<SoundEffect>("sounds", "jump2");
			LoadAsset<SoundEffect>("sounds", "brick1");
			LoadAsset<SoundEffect>("sounds", "brick2");
			LoadAsset<SoundEffect>("sounds", "success");
			LoadAsset<SoundEffect>("sounds", "fail");
			LoadAsset<Song>("sounds", "play_music");
		}
	}
}

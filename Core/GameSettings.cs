using DoodleJump.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	public static partial class GameSettings
	{
		public static int WindowWidth { get; set; } = 880;
		public static int WindowHeight { get; set; } = 1280;
		public static int GameWidth { get; set; } = 880;

		public static float TimeScale { get; set; } = 1.0f;
		public static float ScoreDecimal { get; set; } = 0;
		public static int PlatformStreak { get; set; } = 1;

		public static bool DebugDraw { get; set; } = false;

		public static Assets Assets { get; set; }
		public static Random Random { get; set; } = new Random();

		public static Screen ActiveScreen { get; set; }

		public static PlayScreen PlayScreen { get; set; }
		public static MenuScreen MenuScreen { get; set; }
		public static EndScreen EndScreen { get; set; }

		public static void Initialize()
		{
			PlayScreen = new PlayScreen();
			MenuScreen = new MenuScreen();
			EndScreen = new EndScreen();
			TimeScale = 1f;
			ScoreDecimal = 0;
			PlatformStreak = 0;
			Random = new Random();
		}
	}
}

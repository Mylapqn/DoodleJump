using DoodleJump.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	internal static partial class GameSettings
	{
		public const int PIXEL_SCALE = 4;
		public const int HEIGHT_PER_METER = 100;

		public static int WindowWidth { get; set; } = 880;
		public static int WindowHeight { get; set; } = 1360;
		public static int GameWidth { get; set; } = WindowWidth;
		/*
		public static int WindowWidth { get; set; } = 800;
		public static int WindowHeight { get; set; } = 1000;
		public static int GameWidth { get; set; } = 1000;
		*/

		public static int BombCount { get; set; } = 0;
		public static int BounceCount { get; set; } = 0;

		public static float TimeScale { get; set; } = 1.0f;
		public static float ScoreDecimal { get; set; } = 0;
		public static float MaxHeightReached { get; set; } = 0;
		public static int PlatformStreak { get; set; } = 1;
		public static int MaxPlatformStreak { get; set; } = 0;

		public static float ElapsedGameTime { get; set; } = 0f;

		public static bool DebugDraw { get; set; } = false;

		public static Assets Assets { get; set; }
		public static Random Random { get; set; } = new Random();

		private static Screen _activeScreen;
		public static Screen ActiveScreen
		{
			get => _activeScreen;
			set
			{
				SetActiveScreen(value);
			}
		}

		public static PlayScreen PlayScreen { get; set; }
		public static MenuScreen MenuScreen { get; set; }
		public static EndScreen EndScreen { get; set; }

		public static SaveSystem SaveSystem { get; set; }

		public static void Initialize()
		{
			PlayScreen = new PlayScreen();
			MenuScreen = new MenuScreen();
			EndScreen = new EndScreen();
			SaveSystem = new SaveSystem("high_scores.txt");
			TimeScale = 1f;
			ScoreDecimal = 0;
			PlatformStreak = 0;
			MaxPlatformStreak = 0;
			MaxHeightReached = 0;
			Random = new Random();
			ElapsedGameTime = 0f;

			BounceCount = 0;
			BombCount = 0;
		}
		public static void SetActiveScreen(Screen screen)
		{
			if (_activeScreen != null)
			{
				_activeScreen.Deinitialize();
			}
			_activeScreen = screen;
		}
	}
}

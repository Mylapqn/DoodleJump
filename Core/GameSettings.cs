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
		// Window dimensions used everywhere
		public static int WindowWidth { get; set; } = 1280;
		public static int WindowHeight { get; set; } = 720;

		// Global access to the currently active Screen
		public static Screen ActiveScreen { get; set; }
	}
}

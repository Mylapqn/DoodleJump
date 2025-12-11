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
		public static int WindowWidth { get; set; } = 1280;
		public static int WindowHeight { get; set; } = 720;

		public static PlayScreen ActiveScreen { get; set; }
	}
}

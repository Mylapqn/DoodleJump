using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	public class HighScore
	{
		public string PlayerName { get; set; }
		public int Score { get; set; }
		public int ReachedHeight { get; set; }
		public HighScore(string playerName, int score, int reachedHeight)
		{
			PlayerName = playerName;
			Score = score;
			ReachedHeight = reachedHeight;
		}
	}
}

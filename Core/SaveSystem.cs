using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	public class SaveSystem
	{
		public string SaveFilePath { get; private set; }
		const char Separator = ';';
		public SaveSystem(string saveFilePath)
		{
			SaveFilePath = saveFilePath;
		}
		public List<HighScore> LoadHighScores()
		{
			if (!File.Exists(SaveFilePath))
			{
				return new List<HighScore>();
			}
			List<HighScore> highScores = new();
			string[] lines = File.ReadAllLines(SaveFilePath);
			foreach (string line in lines)
			{
				if (TryParseHighScore(line, out HighScore highScore))
				{
					highScores.Add(highScore);
				}
			}
			return highScores.OrderByDescending(hs => hs.Score).ToList();

		}
		private bool TryParseHighScore(string line, out HighScore highScore)
		{
			string[] parts = line.Split(Separator);
			if (parts.Length == 3 && int.TryParse(parts[1], out int score) && int.TryParse(parts[2], out int height))
			{
				highScore = new HighScore(parts[0], score, height);
				return true;
			}
			else
			{
				highScore = null;
				return false;
			}

		}
		public void SaveHighScores(List<HighScore> highScores)
		{
			string[] lines = highScores.OrderByDescending(hs => hs.Score).Take(10).Select(hs => $"{hs.PlayerName}{Separator}{hs.Score}{Separator}{hs.ReachedHeight}").ToArray();
			File.WriteAllLines(SaveFilePath, lines);
		}
	}
}

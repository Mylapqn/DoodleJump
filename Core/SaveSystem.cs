using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{
	internal class SaveSystem
	{
		public string SaveFilePath { get; private set; }
		const char CSV_SPERATAOR = ';';
		private const string DATE_FORMAT = "yyyy-MM-dd";

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
			string[] parts = line.Split(CSV_SPERATAOR);
			if (parts.Length == 4 && int.TryParse(parts[1], out int score) && int.TryParse(parts[2], out int height) && DateOnly.TryParseExact(parts[3], DATE_FORMAT, out DateOnly date))
			{
				highScore = new HighScore(parts[0], score, height, date);
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
			string[] lines = highScores.OrderByDescending(hs => hs.Score).Take(10).Select(hs => $"{hs.PlayerName}{CSV_SPERATAOR}{hs.Score}{CSV_SPERATAOR}{hs.ReachedHeight}{CSV_SPERATAOR}{hs.Date.ToString(DATE_FORMAT)}").ToArray();
			File.WriteAllLines(SaveFilePath, lines);
		}
	}
}

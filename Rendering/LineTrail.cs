using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	public class LineTrail
	{
		public struct TrailPoint
		{
			public Vector2 position;
			public Color color;
			public TrailPoint(Vector2 position, Color color)
			{
				this.position = position;
				this.color = color;
			}
		}
		public List<List<TrailPoint>> segments;
		public Color color;
		public int maxPoints;
		public float minDistance;
		public LineTrail(int maxPoints = 50, float minDistance = 20f, Color color = new Color())
		{
			segments = new();
			this.color = color;
			this.maxPoints = maxPoints;
			this.minDistance = minDistance;
		}
		public void Draw(PolygonDrawer drawer)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				var currentSegment = segments[i];
				if (currentSegment.Count < 2)
				{
					continue;
				}
				drawer.DrawTrail(points: currentSegment, alphaTo:.3f, widthTo:15);
			}

		}
		public void Update(Vector2 position, Color color, bool interrupt = false)
		{
			if (segments.Count == 0 || interrupt)
			{
				segments.Add(new List<TrailPoint>());
			}
			List<TrailPoint> currentSegment = segments[segments.Count - 1];
			if (currentSegment.Count > 0) currentSegment.RemoveAt(currentSegment.Count - 1);
			if (currentSegment.Count == 0) currentSegment.Add(new TrailPoint(position, color));
			else
			{
				if (Vector2.DistanceSquared(position, currentSegment[currentSegment.Count - 1].position) > minDistance * minDistance)
				{
					currentSegment.Add(new TrailPoint(position, color));
				}
			}
			currentSegment.Add(new TrailPoint(position, color));
			int totalPoints = TotalPoints();
			while (totalPoints > maxPoints)
			{
				if (segments.Count == 0) break;
				List<TrailPoint> firstSegment = segments[0];
				if (firstSegment.Count < 2)
				{
					segments.RemoveAt(0);
					continue;
				}
				firstSegment.RemoveAt(0);
				totalPoints--;
				if (firstSegment.Count < 2)
				{
					segments.RemoveAt(0);
				}
			}
		}
		private int TotalPoints()
		{
			int total = 0;
			foreach (var segment in segments)
			{
				total += segment.Count;
			}
			return total;
		}
	}
}

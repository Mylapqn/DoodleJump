using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DoodleJump.Rendering
{
	public class PolygonDrawer
	{
		public List<VertexPositionColor[]> triangleListCache = [];
		public List<VertexPositionColor[]> triangleStripCache = [];
		public float circleResolution = 1f;
		public Matrix? currentViewMatrix = null;
		private BasicEffect basicEffect;
		private GraphicsDevice GraphicsDevice;
		public void Initialize(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
		{
			GraphicsDevice = graphicsDevice;
			this.basicEffect = basicEffect;
		}
		public void Begin(Matrix? viewMatrix = null)
		{
			currentViewMatrix = viewMatrix;
			triangleListCache = [];
			triangleStripCache = [];
		}
		public void End()
		{
			Matrix backupMatrix;
			backupMatrix = basicEffect.Parameters["WorldViewProj"].GetValueMatrix();
			if (currentViewMatrix != null)
			{
				basicEffect.Parameters["WorldViewProj"].SetValue((Matrix)currentViewMatrix);
			}
			else
			{
				Matrix world = Matrix.Identity;
				Matrix reflection = Matrix.CreateScale(new Vector3(1, -1, 1));
				Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, -GraphicsDevice.Viewport.Height, 0, 0, -2f);
				Matrix screenSpaceMatrix = world * reflection * projection;
				basicEffect.Parameters["WorldViewProj"].SetValue(screenSpaceMatrix);
			}
			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				DrawAll(GraphicsDevice);
			}
			basicEffect.Parameters["WorldViewProj"].SetValue(backupMatrix);
		}
		private void DrawAll(GraphicsDevice graphics)
		{
			foreach (var polygon in triangleListCache)
			{
				graphics.DrawUserPrimitives(PrimitiveType.TriangleList, polygon, 0, polygon.Length / 3);
			}
			foreach (var polygon in triangleStripCache)
			{
				graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, polygon, 0, polygon.Length - 2);
			}
			triangleListCache.Clear();
			triangleStripCache.Clear();
		}

		public void DrawCircle(Vector2 center, float radius, Color color, int outlineWidth = 0)
		{
			if (radius <= 0) return;
			VertexPositionColor[] vertices;
			int totalVertices = (int)(radius * circleResolution);
			totalVertices = Math.Max(totalVertices, 4);
			if (outlineWidth > 0)
			{
				vertices = new VertexPositionColor[(totalVertices + 1) * 2];
				for (int i = 0; i <= totalVertices; i++)
				{
					float angle = (float)(i + 1) / totalVertices * MathF.PI * 2;
					Vector2 dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
					Vector2 outerPoint = center + dir * radius;
					Vector2 innerPoint = center + dir * (radius - outlineWidth);
					vertices[i * 2 + 0] = new VertexPositionColor(new Vector3(innerPoint, 0), color);
					vertices[i * 2 + 1] = new VertexPositionColor(new Vector3(outerPoint, 0), color);
				}
				triangleStripCache.Add(vertices);
			}
			else
			{
				vertices = new VertexPositionColor[totalVertices * 3];
				Vector2 lastPoint = center + new Vector2(radius, 0);
				for (int i = 0; i < totalVertices; i++)
				{
					float angle = (float)(i + 1) / totalVertices * MathF.PI * 2;
					Vector2 point = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
					vertices[i * 3 + 0] = new VertexPositionColor(new Vector3(lastPoint, 0), color);
					vertices[i * 3 + 1] = new VertexPositionColor(new Vector3(point, 0), color);
					vertices[i * 3 + 2] = new VertexPositionColor(new Vector3(center, 0), color);
					lastPoint = point;
				}
				triangleListCache.Add(vertices);
			}
		}
		public void DrawLine(Vector2 start, Vector2 end, Color color, float width)
		{
			VertexPositionColor[] vertices = new VertexPositionColor[4];
			Vector2 direction = end - start;
			direction.Normalize();
			Vector2 normal = new Vector2(-direction.Y, direction.X);
			normal *= width / 2;
			vertices[0] = new VertexPositionColor(new Vector3(start + normal, 0), color);
			vertices[1] = new VertexPositionColor(new Vector3(start - normal, 0), color);
			vertices[2] = new VertexPositionColor(new Vector3(end + normal, 0), color);
			vertices[3] = new VertexPositionColor(new Vector3(end - normal, 0), color);
			triangleStripCache.Add(vertices);
		}
		public void DrawTrail(List<Vector2> points, Color color, float width)
		{
			DrawTrail(points.Select(x => new LineTrail.TrailPoint(x, color)).ToList(), widthFrom: width, widthTo: width);
		}
		public void DrawTrail(List<LineTrail.TrailPoint> points, float widthFrom = 0, float widthTo = 5, float alphaFrom = 0, float alphaTo = 1)
		{
			if (points.Count < 2) return;
			VertexPositionColor[] vertices = new VertexPositionColor[points.Count * 2];
			for (int i = 0; i < points.Count; i++)
			{
				LineTrail.TrailPoint trailPoint = points[i];
				Color color = trailPoint.color;
				//float width = widths[Math.Min(i, widths.Count - 1)];
				float width = MathHelper.Lerp(widthFrom, widthTo, (float)i / (points.Count - 1));
				float alpha = MathHelper.Lerp(alphaFrom, alphaTo, (float)i / (points.Count - 1));
				Vector2 p = trailPoint.position;
				Vector2 normal = Vector2.Zero;
				if (i > 0)
				{
					Vector2 prev = points[i - 1].position;
					Vector2 diff = prev - p;
					normal = new Vector2(diff.Y, -diff.X);
					normal.Normalize();
				}
				vertices[i * 2] = new VertexPositionColor(new Vector3(p + normal * width, 0), ColorUtilities.Premultiply(color, alpha));
				vertices[i * 2 + 1] = new VertexPositionColor(new Vector3(p - normal * width, 0), ColorUtilities.Premultiply(color, alpha));
			}
			triangleStripCache.Add(vertices);
		}
		public void DrawPolygon(List<Vector2> points, Color color)
		{
			if (points.Count < 3) return;
			VertexPositionColor[] vertices = new VertexPositionColor[(points.Count - 2) * 3];
			Vector2 firstPoint = points[0];
			for (int i = 0; i < points.Count - 2; i++)
			{
				Vector2 pointA = points[i + 1];
				Vector2 pointB = points[i + 2];
				vertices[i * 3 + 0] = new VertexPositionColor(new Vector3(firstPoint, 0), color);
				vertices[i * 3 + 1] = new VertexPositionColor(new Vector3(pointA, 0), color);
				vertices[i * 3 + 2] = new VertexPositionColor(new Vector3(pointB, 0), color);
			}
			triangleListCache.Add(vertices);
		}
		public void DrawRectangle(Vector2 topLeft, Vector2 dimensions, Color color, int strokeWidth = 0, float rotation = 0)
		{
			Vector2 topRight = new Vector2(topLeft.X + dimensions.X, topLeft.Y);
			Vector2 bottomRight = topLeft + dimensions;
			Vector2 bottomLeft = new Vector2(topLeft.X, topLeft.Y + dimensions.Y);
			if (rotation != 0)
			{
				Vector2 center = topLeft + dimensions / 2;
				Matrix rotationMatrix = Matrix.CreateTranslation(-center.X, -center.Y, 0) *
									   Matrix.CreateRotationZ(rotation) *
									   Matrix.CreateTranslation(center.X, center.Y, 0);
				topLeft = Vector2.Transform(topLeft, rotationMatrix);
				topRight = Vector2.Transform(topRight, rotationMatrix);
				bottomRight = Vector2.Transform(bottomRight, rotationMatrix);
				bottomLeft = Vector2.Transform(bottomLeft, rotationMatrix);
			}
			if (strokeWidth > 0)
			{
				DrawLine(topLeft, topRight, color, strokeWidth);
				DrawLine(topRight, bottomRight, color, strokeWidth);
				DrawLine(bottomRight, bottomLeft, color, strokeWidth);
				DrawLine(bottomLeft, topLeft, color, strokeWidth);
			}
			else
			{
				List<Vector2> points =
				[
					topLeft,
					topRight,
					bottomRight,
					bottomLeft,
				];
				DrawPolygon(points, color);
			}
		}
	}
}

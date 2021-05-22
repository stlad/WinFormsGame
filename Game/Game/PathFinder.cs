using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
	public class PathFinder
	{
		public static List<PointF> FindPaths(Model model, Creature seeker, PointF target)
		{
			var ways = new Dictionary<PointF, List<PointF>>();
			ways[seeker.Location] = new List<PointF>(new PointF[1] { seeker.Location});
			var visited = new HashSet<PointF>();
			var queue = new Queue<PointF>();
			queue.Enqueue(seeker.Location);

			while (queue.Count != 0)
			{
				var currentPoint = queue.Dequeue();
				if (IsPointWrong(currentPoint, model, visited)) continue;

				visited.Add(currentPoint);
				if (target == currentPoint)
					return ways[currentPoint];

				AddNextNodes(currentPoint, ways, queue,visited);
			}
			return null;
		}

		private static void AddNextNodes(PointF current, Dictionary<PointF, List<PointF>> ways, Queue<PointF> queue, HashSet<PointF> visited)
		{
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					//if ((Math.Abs(dir.Width) + Math.Abs(dir.Height) != 1)) continue;
					var newPoint = new PointF(current.X + dx*10, 
						current.Y + dy*10);
					if (visited.Contains(newPoint)) continue;
					queue.Enqueue(newPoint);
					ways[newPoint] = new List<PointF>(ways[current]);
					ways[newPoint].Add(newPoint);
				}
			}
		}

		private static bool IsPointWrong(PointF pt, Model model, HashSet<PointF> visited)
		{
			foreach(var wall in model.Terrains)
            {
				if (wall.HitBox.IntersectsWith(new RectangleF(pt.X, pt.Y, 0, 0))) 
					return true;
            }
			return false;
		}
	}
}

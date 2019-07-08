using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models.Geometry;
namespace EagleEye.Models
{
	public class NavigationContext
	{
		public NavigationContext(IEnumerable<Annotation> annotations)
		{

			Annotations = annotations;
			GenerateNodes();
		}
		private IEnumerable<Annotation> Annotations { get; set; }
		private HashSet<Vector2> Nodes { get; set; } = new HashSet<Vector2>();
		private Vector2 Start { get; set; }
		private Annotation Goal { get; set; }
		static private double NodeDistance { get => 0.05; }
		private void GenerateNodes()
		{
			foreach (Annotation annotation in Annotations.Where(a => a.Type == Annotation.AnnotationType.Isle))
			{
				List<Vector2> midpoints = annotation.Midpoints();
				for (int i = 0; i < 2; i++)
				{
					Vector2 start = midpoints[i];
					Vector2 end = midpoints[i + 2];
					Vector2 segment = (end - start);
					double step = segment.Length / Math.Floor(segment.Length / NodeDistance);
					for (double t = 0.0; t <= segment.Length;t += step)
					{
						Nodes.Add(start + segment.Normalized() * t);
					}
				}
			}
		}
		private List<Vector2> AStarReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
		{
			List<Vector2> totalPath = new List<Vector2> { current };
			while (cameFrom.ContainsKey(current))
			{
				current = cameFrom[current];
				totalPath.Add(current);
			}
			return totalPath;
		}
		private double AStartHeuristicCostEstimate(Vector2 a, Vector2 b)
		{
			var ab = a - b;
			if (a == b)
				return 0;
			return (a - b).Length;
		}
		private IEnumerable<Vector2> AStarNeighbors(Vector2 a)
		{
			return Nodes.Where(n => n != a && !Annotations.Where(an => an.Type == Annotation.AnnotationType.Parking).Any(an => an.Intersects(a, n - a)));
		}
		public List<Vector2> AStar(Vector2 start, List<Vector2> goal)
		{
			Nodes.Add(start);
			foreach (var g in goal)
				Nodes.Add(g);
			// The set of nodes already evaluated
			HashSet<Vector2> closedSet = new HashSet<Vector2>();

			// The set of currently discovered nodes that are not evaluated yet.
			// Initially, only the start node is known.
			HashSet<Vector2> openSet = new HashSet<Vector2> { start };

			// For each node, which node it can most efficiently be reached from.
			// If a node can be reached from many nodes, cameFrom will eventually contain the
			// most efficient previous step.
			Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

			// For each node, the cost of getting from the start node to that node.
			Dictionary<Vector2, double> gScore = new Dictionary<Vector2, double>();
			foreach (var node in Nodes)
				gScore.Add(node, double.PositiveInfinity);
			// The cost of going from start to start is zero.
			gScore[start] = 0;

			// For each node, the total cost of getting from the start node to the goal
			// by passing by that node. That value is partly known, partly heuristic.
			Dictionary<Vector2, double> fScore = new Dictionary<Vector2, double>();
			foreach (var node in Nodes)
				fScore.Add(node, double.PositiveInfinity);

			// For the first node, that value is completely heuristic.
			fScore[start] = AStartHeuristicCostEstimate(start, goal.First());

			Vector2 current = start;
			while (openSet.Count > 0)
			{

				current = fScore.ToList().OrderBy(p => p.Value).FirstOrDefault(p => openSet.Contains(p.Key)).Key;

				if (goal.Contains(current))
					return AStarReconstructPath(cameFrom, current);


				openSet.Remove(current);

				closedSet.Add(current);


				foreach (Vector2 neighbor in AStarNeighbors(current))
				{

					if (closedSet.Contains(neighbor))
						continue; // Ignore the neighbor which is already evaluated.

					// The distance from start to a neighbor
					double tentativeGScore = gScore[current] + (current - neighbor).Length;


					if (!openSet.Contains(neighbor))
					{  // Discover a new node

						openSet.Add(neighbor);

					}
					else if (tentativeGScore >= gScore[neighbor])
					{

						continue;
					}

					// This path is the best until now. Record it!
					if (cameFrom.ContainsKey(neighbor))
					{
						cameFrom[neighbor] = current;
					}
					else
					{
						cameFrom.Add(neighbor, current);
					}

					gScore[neighbor] = tentativeGScore;

					fScore[neighbor] = gScore[neighbor] + goal.Min(g => AStartHeuristicCostEstimate(neighbor, g));
				}
			}
			// A valid path was not found, return a path containing just the start node
			return new List<Vector2> { start };
		}
	}
}
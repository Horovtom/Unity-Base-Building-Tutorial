using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public class Path_AStar {
	Stack<Tile> total_path;

	public Path_AStar(World world, Tile tileStart, Tile tileEnd) {
		if (world.tileGraph == null) {
			world.tileGraph = new Path_TileGraph(world);
		}

		Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

		if (nodes.ContainsKey(tileStart) == false) {
			Debug.LogError("Path_AStar: The starting tile isn't in the list of nodes!");
			return;
		}
		if (nodes.ContainsKey(tileEnd) == false) {
			Debug.LogError("Path_AStar: The ending tile isn't in the list of nodes!");
			return;
		}

		Path_Node<Tile> start = nodes[tileStart];
		Path_Node<Tile> goal = nodes[tileEnd];



		List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();
		//List<Path_Node<Tile>> OpenSet = new List<Path_Node<Tile>>();
		//OpenSet.Add(start);

		SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>>();
		OpenSet.Enqueue(start, 0);

		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();

		Path_ScoreDict g_score = new Path_ScoreDict();
		g_score[start] = 0;
		Path_ScoreDict f_score = new Path_ScoreDict();
		f_score[start] = heuristic_cost_estimate(start, goal);

		while(OpenSet.Count > 0) {
			Path_Node<Tile> current = OpenSet.Dequeue();
			if (current.Equals(goal)) {
				reconstruct_path(Came_From, goal);
				return;
			}

			ClosedSet.Add(current);

			foreach(Path_Edge<Tile> neighbour in current.edges) {
				if (ClosedSet.Contains(neighbour.node)) {
					continue;
				}

				float tentative_g_score = g_score[current] + neighbour.cost;
				//float tentative_g_score = g_score[current] + dist_between(current, neighbour);

				if (OpenSet.Contains(neighbour.node)) {
					if (tentative_g_score >= g_score[neighbour.node]) {
						continue; //This is not a better path
					}
				}
				Came_From[neighbour.node] = current;
				g_score[neighbour.node] = tentative_g_score;
				f_score[neighbour.node] = g_score[neighbour.node] + heuristic_cost_estimate(neighbour.node, goal);

				if (OpenSet.Contains(neighbour.node)) {
					OpenSet.UpdatePriority(neighbour.node, f_score[neighbour.node]);
				} else {
					OpenSet.Enqueue(neighbour.node, f_score[neighbour.node]);
				}
			}
		}

		//We don't have a failure state, maybe? It's just that the path list will be null
		return;
	}

	void reconstruct_path(Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From, Path_Node<Tile> current) {
		total_path = new Stack<Tile>();

		while(Came_From.ContainsKey(current)) {
			total_path.Push(current.data);
			current = Came_From[current];
		}
	}

	/*float dist_between(Path_Node<Tile> a, Path_Edge<Tile> b) {
		
	}*/

	float heuristic_cost_estimate(Path_Node<Tile> start, Path_Node<Tile> goal) {
		return Mathf.Sqrt(Mathf.Pow(start.data.X - goal.data.X, 2) + Mathf.Pow(start.data.Y - goal.data.Y, 2));
	}

	public Tile Dequeue() {
		return total_path == null ? null : total_path.Pop();
	}

	public int Length() {
		return total_path == null ? 0 : total_path.Count;
	}
}
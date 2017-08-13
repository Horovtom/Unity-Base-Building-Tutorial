using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// This class constructs a simple path-finding compatible graph
/// of our world. Each tile is a node. Each WALKABLE neighbour 
/// from a tile is linked via an edge connection
/// </summary>
public class Path_TileGraph {

	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world, bool debug = false) {
		nodes = new Dictionary<Tile, Path_Node<Tile>>();
		//Loop thorugh all tiles of the world
		//For each tile, create a node
		//Do we create nodes for non.floor tiles? No 
		//Do we create nodes for tiles that are completely unwalkable (i.e. walls)? No

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {

				Tile t = world.GetTileAt(x, y);

				if (t.MovementCost > 0) { //Tiles with a move cost of 0 are unwalkable
					Path_Node<Tile> n = new Path_Node<Tile>();
					n.data = t;
					nodes.Add(t, n);
					if (debug) Debug.DrawLine(new Vector3(x+0.4f, y+0.25f, 0), new Vector3(x+0.6f, y+0.75f, 0), Color.red, 5f);
				}
			}
		}

		//Now loop through all tiles again
		//Create edges for neighbours

		foreach(Tile t in nodes.Keys) {
			Path_Node<Tile> n = nodes[t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

			//Get a list of neighbours for the tile
			ReadOnlyCollection<Tile> neighbours = t.GetNeighbours(true); //NOTE: Some of the array spots could be null

			//If a neighbour is walkable, create an edge to the relevant node.
			for (int i = 0; i < neighbours.Count; i++) {
				if (neighbours[i] != null && neighbours[i].MovementCost > 0) {
					//This neighbours exists and is walkable, so create an edge.

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbours[i].MovementCost * i < 4 ? 1 : 1.41421356237f;
					e.node = nodes[neighbours[i]];
					edges.Add(e);
					if (debug) Debug.DrawLine(new Vector3(t.X + 0.5f, t.Y + 0.5f, 0), new Vector3(e.node.data.X + 0.5f, e.node.data.Y+0.5f, 0), Color.green, 10f);
				}
			}



			n.edges = edges.ToArray();

		}
	}

}

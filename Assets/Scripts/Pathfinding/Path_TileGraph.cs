using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class constructs a simple path-finding compatible graph
/// of our world. Each tile is a node. Each WALKABLE neighbour 
/// from a tile is linked via an edge connection
/// </summary>
public class Path_TileGraph {

	Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world) {
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
				}
			}
		}

		//Now loop through all tiles again
		//Create edges for neighbours

		foreach(Tile t in nodes.Keys) {
			//Get a list of neighbours for the tile
			//If a neighbour is walkable, create an edge to the relevant node.


		}
	}

}

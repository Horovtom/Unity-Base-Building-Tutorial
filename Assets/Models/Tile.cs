using UnityEngine;
using System.Collections;

public class Tile {
	
	enum TileType {
		Empty,
		Floor
	};

	TileType type = TileType.Empty;

	LooseObject looseObject;
	InstalledObject installedObject;

	World world;
	int x, y;

	public Tile(World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}

}

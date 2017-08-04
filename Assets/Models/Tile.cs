using UnityEngine;
using System.Collections;
using System;

public class Tile {

	Action<Tile> cbTileTypeChanged;
	
	public enum TileType {
		Empty,
		Floor
	};

	TileType type = TileType.Empty;

	public TileType Type {
		get {
			return type;
		}
		set {
			if (type != value) {
				type = value;
				//Call the callback and let things know we've changed
				if (cbTileTypeChanged != null)
					cbTileTypeChanged(this);
			}
		}
	}

	LooseObject looseObject;
	InstalledObject installedObject;

	World world; 
	int x, y;

	public int X {
		get {
			return x;
		}
	}

	public int Y {
		get {
			return y;
		}
	}

	public Tile(World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}


	public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
		//It behaves like an array, you can call this function multiple times
		this.cbTileTypeChanged += callback;
	} 

	public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
		this.cbTileTypeChanged -= callback;
	} 

}

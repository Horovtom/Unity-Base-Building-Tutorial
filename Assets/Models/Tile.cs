using UnityEngine;
using System.Collections;
using System;

public class Tile {

	Action<Tile> cbTileTypeChanged;
	
	public enum TileType {
		Empty,
		Floor
	};

	TileType _type = TileType.Empty;

	public TileType Type {
		get {
			return _type;
		}
		set {
			if (_type != value) {
				_type = value;
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

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">A World instance.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile(World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Registers the tile type changed callback.
	/// </summary>
	public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
		//It behaves like an array, you can call this function multiple times
		this.cbTileTypeChanged += callback;
	} 

	/// <summary>
	/// Unregisters the tile type changed callback.
	/// </summary>
	public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
		this.cbTileTypeChanged -= callback;
	} 

}

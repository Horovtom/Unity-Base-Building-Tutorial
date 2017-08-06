using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;

	Action<InstalledObject> cbInstalledObjectCreated;

	Dictionary<string, InstalledObject> installedObjectPrototypes;

	public int Width {
		get;
		protected set;
	}

	public int Height {
		get;
		protected set;
	}

	public World (int width = 100, int height = 100) {
		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile(this, x, y);
			}
		}

		Debug.Log("World created with " + (width * height) + " tiles.");

		CreateInstalledObjectPrototypes();

	}

	void CreateInstalledObjectPrototypes () {
		installedObjectPrototypes = new Dictionary<string, InstalledObject>();

		installedObjectPrototypes.Add("Wall", 
			InstalledObject.CreatePrototype(
				"Wall",
				0, //Tile is impassable
				1,
				1,
				true //Links to neighbours and "sort of" becomes part of a large object
			)
		);
	}


	public Tile GetTileAt (int x, int y) {
		if (x > Width || x < 0 || y > Height || y < 0) {
			//Debug.LogError("Tile (" + x + ", " + y + ") is out of range!");
			return null;
		}
		return tiles[x, y];
	}

	public void RandomizeTiles () {
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				if (UnityEngine.Random.Range(0, 2) == 0) {
					tiles[x, y].Type = TileType.Floor;
				} else {
					tiles[x, y].Type = TileType.Empty;
				}
			}
		}
	}

	public void PlaceInstalledObject(string objectType, Tile t) {
		//TODO: This function asumes 1x1 tiles -- change this later!

		if (!installedObjectPrototypes.ContainsKey(objectType)) {
			Debug.LogError("installedObjectPrototypes doesn't contain a proto for key: " + objectType);
			return;
		}

		InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], t);

		if (obj == null) {
			//Failed to place object -- most likely there was already something there.
			return;
		}

		if (cbInstalledObjectCreated != null) {
			cbInstalledObjectCreated(obj);
		}
	}

	public void RegisterInstalledObjectCreated(Action<InstalledObject> callbackFunc) {
		cbInstalledObjectCreated += callbackFunc;
	}

	public void UnregisterInstalledObjectCreated(Action<InstalledObject> callbackFunc) {
		cbInstalledObjectCreated -= callbackFunc;
	}
}

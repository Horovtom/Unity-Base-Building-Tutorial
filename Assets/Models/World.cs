using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;

	Action<Furniture> cbFurnitureCreated;
	Action<Tile> cbTileChanged;
	Dictionary<string, Furniture> furniturePrototypes;

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
				tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
			}
		}

		Debug.Log("World created with " + (width * height) + " tiles.");

		CreateFurniturePrototypes();

	}

	void CreateFurniturePrototypes () {
		furniturePrototypes = new Dictionary<string, Furniture>();

		furniturePrototypes.Add("Wall", 
			Furniture.CreatePrototype(
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

	public void PlaceFurniture(string objectType, Tile t) {
		//TODO: This function asumes 1x1 tiles -- change this later!

		if (!furniturePrototypes.ContainsKey(objectType)) {
			Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
			return;
		}

		Furniture obj = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

		if (obj == null) {
			//Failed to place object -- most likely there was already something there.
			return;
		}

		if (cbFurnitureCreated != null) {
			cbFurnitureCreated(obj);
		}
	}

	public void RegisterFurnitureCreated(Action<Furniture> callbackFunc) {
		cbFurnitureCreated += callbackFunc;
	}

	public void UnregisterFurnitureCreated(Action<Furniture> callbackFunc) {
		cbFurnitureCreated -= callbackFunc;
	}

	public void RegisterTileChanged(Action<Tile> callbackFunc) {
		cbTileChanged += callbackFunc;
	}

	public void UnregisterTileChanged(Action<Tile> callbackFunc) {
		cbTileChanged -= callbackFunc;
	}

	void OnTileChanged(Tile t) {
		if (cbTileChanged == null)
			return;
		cbTileChanged(t);
	}
}

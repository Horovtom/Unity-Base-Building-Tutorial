using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;
	List<Character> characters;

	Action<Furniture> cbFurnitureCreated;
	Action<Tile> cbTileChanged;
	Action<Character> cbCharacterCreated;
	Dictionary<string, Furniture> furniturePrototypes;
	/// <summary>
	/// The tile graph used for pathfinding.
	/// </summary>
	public Path_TileGraph tileGraph;

	//TODO: Most likely this will be replaced with a dedicated class for managing job queues (plural!) that might also 
	//be semi-static or self initializing or some damn thing.
	//For now, this is just a PUBLIC meber of world

	public JobQueue jobQueue;

	public int Width {
		get;
		protected set;
	}

	public int Height {
		get;
		protected set;
	}

	public World (int width = 100, int height = 100) {
		jobQueue = new JobQueue();
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

		characters = new List<Character>();
	}

	public void Update(float deltaTime) {
		foreach(Character c in characters) {
			c.Update(deltaTime);
		}
	}

	public Character CreateCharacter(Tile t) {
		Character c = new Character(t);
	
		if (cbCharacterCreated != null)
			cbCharacterCreated(c);

		characters.Add(c);
		return c;
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
		if (x >= Width || x < 0 || y >= Height || y < 0) {
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
			InvalidateTileGraph();
		}
	}

	public void RegisterCharacterCreated(Action<Character> callbackFunc) {
		cbCharacterCreated += callbackFunc;
	}

	public void UnregisterCharacterCreated(Action<Character> callbackFunc) {
		cbCharacterCreated -= callbackFunc;
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


	/// <summary>
	/// Gets called whenever ANY tile changes
	/// </summary>
	void OnTileChanged(Tile t) {
		if (cbTileChanged == null)
			return;
		cbTileChanged(t);

		InvalidateTileGraph();
	}

	/// <summary>
	/// This should be called whenver a change to the world
	/// means that our old pathfinding info is invalid.
	/// </summary>
	public void InvalidateTileGraph() {
		tileGraph = null;
	}

	public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
		return furniturePrototypes[furnitureType].IsValidPosition(t);
	}

	public Furniture GetFurniturePrototype(string objectType ) {
		if (furniturePrototypes.ContainsKey(objectType) == false) {
			Debug.LogError("No furniture with type: " + objectType);
			return null;
		}
		return furniturePrototypes[objectType];
	}

	public void SetupPathfindingExample() {
		Debug.Log("SetupPathfindingExample");

		int l = Width / 2 - 5;
		int b = Height / 2 - 5;

		for (int x = l - 5; x < l + 15; x++) {
			for (int y = b - 5; y < b + 15; y++) {
				tiles[x, y].Type = TileType.Floor;

				if (x == l || x == (l + 9) || y == b || y == (b + 9)) {
					if (x != l + 9 &&  y != b + 4) {
						PlaceFurniture("Wall", tiles[x, y]);
					}
				}
			}
		}
	}


}

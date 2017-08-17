using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class World : IXmlSerializable {

	Tile[,] tiles;
	public List<Character> characters;
	public List<Furniture> furnitures;

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



	public World (int width, int height) {
		//Creates an empty world
		SetupWorld(width, height);

		//Make one character
		CreateCharacter(GetTileAt(Width/2, Height/2));
	}

	void SetupWorld(int width, int height) {
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
		furnitures = new List<Furniture>();
	}

	public void Update(float deltaTime) {
		foreach(Character c in characters) {
			c.Update(deltaTime);
		}

		foreach(Furniture f in furnitures) {
			f.Update(deltaTime);
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
		//This will be replaced by a function that reads all of our furniture data from a text file in the future

		furniturePrototypes = new Dictionary<string, Furniture>();

		furniturePrototypes.Add("Wall", 
			new Furniture(
				"Wall",
				0, //Tile is impassable
				1,
				1,
				true //Links to neighbours and "sort of" becomes part of a large object
			)
		);

		furniturePrototypes.Add("Door", 
			new Furniture(
				"Door",
				1, //Door pathfinding cost
				1,
				1,
				false //Links to neighbours and "sort of" becomes part of a large object
			)
		);


		furniturePrototypes["Door"].furnParameters["openness"] = 0;
		furniturePrototypes["Door"].furnParameters["is_opening"] = 0;
		furniturePrototypes["Door"].updateActions += FurnitureActions.Door_UpdateAction;
		furniturePrototypes["Door"].getEnterability = FurnitureActions.Door_IsEnterable;

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

	public Furniture PlaceFurniture(string objectType, Tile t) {
		//TODO: This function asumes 1x1 tiles -- change this later!

		if (!furniturePrototypes.ContainsKey(objectType)) {
			Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
			return null;
		}

		Furniture furn = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

		if (furn == null) {
			//Failed to place object -- most likely there was already something there.
			return null;
		}

		furnitures.Add(furn);

		if (cbFurnitureCreated != null) {
			cbFurnitureCreated(furn);
			InvalidateTileGraph();
		}
		return furn;
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

	//////////////////////////////////////////
	/// 		
	/// 			SAVING & LOADING
	/// 
	/////////////////////////////////////////

	#region Saving & Loading

	public World() {}

	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		//Save info here
		writer.WriteAttributeString("Width", Width.ToString());
		writer.WriteAttributeString("Height", Height.ToString());

		writer.WriteStartElement("Tiles");
		int count = 0;
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				if (tiles[x, y].Type == TileType.Empty)
					continue;
				writer.WriteStartElement("Tile");
				tiles[x, y].WriteXml(writer);
				writer.WriteEndElement();
				count++;
			}
		}
		writer.WriteEndElement();
		Debug.Log("Written " + count + " tiles");

		writer.WriteStartElement("Furnitures");
		foreach(Furniture furn in furnitures) {
			writer.WriteStartElement("Furniture");
			furn.WriteXml(writer);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();

		writer.WriteStartElement("Characters");
		foreach(Character character in characters) {
			writer.WriteStartElement("Character");
			character.WriteXml(writer);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader) {
		Debug.Log("Read XML runs!");
		//Load info here

		Width = int.Parse(reader.GetAttribute("Width"));
		Height = int.Parse(reader.GetAttribute("Height"));

		SetupWorld(Width, Height);

		while (reader.Read()) {
			switch (reader.Name) {
				case "Tiles":
					ReadXml_Tiles(reader);
					break;
				case "Furnitures":
					ReadXml_Furnitures(reader);
					break;
				case "Characters":
					ReadXml_Characters(reader);
					break;
			}
		}


	}

	void ReadXml_Tiles(XmlReader reader) {
		int x = -1, y = -1, count = 0;

		if (reader.ReadToDescendant("Tile")) {
			do {
				x = int.Parse(reader.GetAttribute("X"));
				y = int.Parse(reader.GetAttribute("Y"));
				tiles[x, y].ReadXml(reader);
				count++;
			} while (reader.ReadToNextSibling("Tile"));
		}
		Debug.Log("Read " + count + " tiles.");
	}

	void ReadXml_Furnitures(XmlReader reader) {
		int x = -1, y = -1, count = 0;

		if (reader.ReadToDescendant("Furniture")) {
			do {
				x = int.Parse(reader.GetAttribute("X"));
				y = int.Parse(reader.GetAttribute("Y"));

				Furniture furn = PlaceFurniture(reader.GetAttribute("objectType"), tiles[x, y]);
				furn.ReadXml(reader);
				count++;
			} while (reader.ReadToNextSibling("Furniture"));
		}
		Debug.Log("Read " + count + " furnitures.");
	}

	void ReadXml_Characters(XmlReader reader) {
		int x = -1, y = -1, count = 0;

		if (reader.ReadToDescendant("Character")) {
			do {
				x = int.Parse(reader.GetAttribute("X"));
				y = int.Parse(reader.GetAttribute("Y"));


				Character c = CreateCharacter(GetTileAt(x, y));
				c.ReadXml(reader);
				count++;
			} while (reader.ReadToNextSibling("Character"));
		}
		Debug.Log("Placed " + count + " characters");
	}

	#endregion

}

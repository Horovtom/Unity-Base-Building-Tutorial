using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {


	public static WorldController Instance {
		get;
		protected set;
	}

	public Sprite floorSprite;
	public Sprite emptySprite;
//FIXME

	Dictionary<Tile, GameObject> tileGameObjectMap;
	Dictionary<Furniture, GameObject> furnitureGameObjectMap;

	Dictionary<string, Sprite> furnitureSprites;

	public World World {
		get; 
		protected set;
	}

	// Use this for initialization
	void Start () {


		LoadSprites();


		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
		if (Instance != null) {
			Debug.LogError("There should be only one WorldController!!!");
		}
		Instance = this;

		World = new World();

		//Registering callback
		World.RegisterFurnitureCreated(OnFurnitureCreated);

		//Create a GameObject for each of our tiles, so they show visually
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				Tile tile_dataobject = World.GetTileAt(x, y);
				//Get our tile game object (used in graphics)
				GameObject tile_gameobject = new GameObject();
				tileGameObjectMap.Add(tile_dataobject, tile_gameobject);

				tile_gameobject.name = "Tile(" + x + "," + y + ")";
				tile_gameobject.transform.position = new Vector3(tile_dataobject.X, tile_dataobject.Y, 0);
				tile_gameobject.transform.SetParent(this.transform, true);

				//Add a Sprite Renderer
				//Add a default sprite for empty tiles.
				tile_gameobject.AddComponent<SpriteRenderer>().sprite = emptySprite;
				//Just save lambda there
				tile_dataobject.RegisterTileTypeChangedCallback(OnTileTypeChanged);

			}
		}

		//Center the Camera
		Camera.main.transform.position = new Vector3(World.Width /2, World.Height/2, Camera.main.transform.position.z);

		//World.RandomizeTiles();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnTileTypeChanged (Tile tile_data) {
		if (tileGameObjectMap.ContainsKey(tile_data) == false) {
			Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- " +
				"did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		GameObject tile_go = tileGameObjectMap[tile_data];

		if (tile_go == null) {
			Debug.LogError("tileGameObjectMap returned GameObject is null -- " +
				"did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		if (tile_data.Type == TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
		} else if (tile_data.Type == TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
		} else {
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type!");
		}
	}

	void LoadSprites() {
		furnitureSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

		foreach (Sprite s in sprites) {
			furnitureSprites[s.name] = s;
		}
	}

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		return WorldController.Instance.World.GetTileAt(x, y);
	}

	public void OnFurnitureCreated (Furniture obj) {
		//Create a visual GameObject linked to this data.
		GameObject furn_go = new GameObject();

		//FIXME: Does not consider multi-tile objects nor rotated objects

		furnitureGameObjectMap.Add(obj, furn_go);
		furn_go.name = obj.ObjectType + "(" + obj.Tile.X + ", " + obj.Tile.Y + ")";
		furn_go.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
		furn_go.transform.SetParent(this.transform, true);

		//FIXME: We assume that the object must be a wall, so use the hardcoded reference to the wall sprite.
		furn_go.AddComponent< SpriteRenderer>().sprite = GetSpriteForFurniture(obj); 

		furn_go.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";

		obj.RegisterOnChangedCallback(OnFurnitureChanged);
	}

	Sprite GetSpriteForFurniture (Furniture obj) {
		int x = obj.Tile.X, y = obj.Tile.Y;
		if (obj.LinksToNeighbour == false) {
			return furnitureSprites[obj.ObjectType];
		} else {
			//otherwise the sprite name is more complicated.

			string spriteName = obj.ObjectType + "_";

			//Check for neighbours North, East, South, West
			Tile t;
			t = World.GetTileAt(x, y + 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "N";
			t = World.GetTileAt(x + 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "E";
			t = World.GetTileAt(x, y - 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "S";
			t = World.GetTileAt(x - 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "W";

			if (furnitureSprites.ContainsKey(spriteName) == false) {
				Debug.LogError("GetSpriteForFurniture - Sprite: " + spriteName + " does not exist!");
				return null;
			}

			return furnitureSprites[spriteName];
		}
	}

	public void OnFurnitureChanged (Furniture furn) {
		//Make sure the furniture's graphics are correct.
		if (furnitureGameObjectMap.ContainsKey(furn) == false) {
			Debug.LogError("OnFurnitureChanged - Trying to change visuals for furniture not in our map.");
			return;
		}
	
		GameObject furn_go = furnitureGameObjectMap[furn];
		furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
	}
}

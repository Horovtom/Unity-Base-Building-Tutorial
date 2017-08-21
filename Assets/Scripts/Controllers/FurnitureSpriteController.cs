using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureSpriteController: MonoBehaviour {


	public static WorldController Instance {
		get;
		protected set;
	}

	Dictionary<Furniture, GameObject> furnitureGameObjectMap;

	Dictionary<string, Sprite> furnitureSprites;

	World world {
		get {return WorldController.Instance.world;}
	}

	// Use this for initialization
	void Start () {
		LoadSprites();

		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();


		//Registering callback
		world.RegisterFurnitureCreated(OnFurnitureCreated);

		//Go through any existing (i.e. from a save that was loaded OnEnable) furniture and call the OnCreated event manually?
		foreach(Furniture furn in world.furnitures) {
			OnFurnitureCreated(furn);
		}
	}
		
	void LoadSprites() {
		furnitureSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

		foreach (Sprite s in sprites) {
			furnitureSprites[s.name] = s;
		}
	}

	public void OnFurnitureCreated (Furniture furn) {
		//Create a visual GameObject linked to this data.
		GameObject furn_go = new GameObject();



		//FIXME: Does not consider multi-tile objects nor rotated objects

		furnitureGameObjectMap.Add(furn, furn_go);
		furn_go.name = furn.ObjectType + "(" + furn.Tile.X + ", " + furn.Tile.Y + ")";
		furn_go.transform.position = new Vector3(furn.Tile.X, furn.Tile.Y, 0);
		furn_go.transform.SetParent(this.transform, true);

		//FIXME: This hardconding is not ideal!
		if (furn.ObjectType == "Door") {
			//By default, the door graphic is meant for walls to the east & west
			//Check to see if we actually have a wall noirth/south, and if so, then 
			//rotate this GO by 90 degrees
			Tile northTile = world.GetTileAt(furn.Tile.X, furn.Tile.Y + 1);
			Tile southTile = world.GetTileAt(furn.Tile.X, furn.Tile.Y - 1);
			if (northTile != null && southTile != null && northTile.furniture != null && southTile.furniture != null &&
				northTile.furniture.ObjectType == "Wall" && southTile.furniture.ObjectType == "Wall") {
				furn_go.transform.rotation = Quaternion.Euler(0, 0, 90);
				furn_go.transform.Translate(1f, 0,0, Space.World); //Ugly hack to compensate for bottom_left anchor point
			}
		}

		//FIXME: We assume that the object must be a wall, so use the hardcoded reference to the wall sprite.
		furn_go.AddComponent< SpriteRenderer>().sprite = GetSpriteForFurniture(furn); 

		furn_go.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";

		furn.RegisterOnChangedCallback(OnFurnitureChanged);
	}

	public Sprite GetSpriteForFurniture (Furniture furn) {
		int x = furn.Tile.X, y = furn.Tile.Y;
		string spriteName = furn.ObjectType;
		if (furn.LinksToNeighbour == false) {
			//If this is a door, check for openness and update the sprite.
			//FIXME: All this hardcoding needs to be generalized later.
			if (furn.ObjectType == "Door") {

				if (furn.furnParameters["openness"] < 0.1f) {
					//Door is closed
					spriteName = "Door";
				} else if (furn.furnParameters["openness"] < 0.5f) {
					spriteName = "Door_openness_1";
				} else if (furn.furnParameters["openness"] < 0.9f) {
					spriteName = "Door_openness_2";
				} else {
					spriteName = "Door_openness_3";
				}
			} 

			return furnitureSprites[spriteName];
		} else {
			//otherwise the sprite name is more complicated.
			spriteName += '_';


			//Check for neighbours North, East, South, West
			Tile t;
			t = world.GetTileAt(x, y + 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == furn.ObjectType)
				spriteName += "N";
			t = world.GetTileAt(x + 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == furn.ObjectType)
				spriteName += "E";
			t = world.GetTileAt(x, y - 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == furn.ObjectType)
				spriteName += "S";
			t = world.GetTileAt(x - 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == furn.ObjectType)
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

	public Sprite GetSpriteForFurniture(string objectType) {
		if (furnitureSprites.ContainsKey(objectType)) {
			return furnitureSprites[objectType];
		} else if (furnitureSprites.ContainsKey(objectType + "_")){
			return furnitureSprites[objectType + "_"];
		} else {
			Debug.LogError("GetSpriteForFurniture - Sprite: " + objectType + " does not exist!");
			return null;
		}
	}
}

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
	}
		
	void LoadSprites() {
		furnitureSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

		foreach (Sprite s in sprites) {
			furnitureSprites[s.name] = s;
		}
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
			t = world.GetTileAt(x, y + 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "N";
			t = world.GetTileAt(x + 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "E";
			t = world.GetTileAt(x, y - 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				spriteName += "S";
			t = world.GetTileAt(x - 1, y);
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

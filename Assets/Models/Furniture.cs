using UnityEngine;
using System.Collections;
using System;

public class Furniture {

	public Tile Tile {
		get;
		protected set;
	}

	public string ObjectType {
		get;
		protected set;
	}

	int width, height;

	public bool LinksToNeighbour {
		get;
		protected set;
	}

	Action<Furniture> cbOnChanged;

	//This is a multiplier. So a value of 2 here, means you move twice as slowly. (i.e. at half speed)
	float movementCost;

	//TODO: Implement larger objects
	//TODO: Implement object rotation

	protected Furniture (){}

	//This is used by our object factory to create the prototypical object
	//It doesnt ask for a tile.
	static public Furniture CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false) {
		Furniture obj = new Furniture();
		obj.ObjectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;
		obj.LinksToNeighbour = linksToNeighbour;

		return obj;
	}

	static public Furniture PlaceInstance (Furniture proto, Tile tile) {
		Furniture obj = new Furniture();

		obj.ObjectType = proto.ObjectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;
		obj.LinksToNeighbour = proto.LinksToNeighbour;

		obj.Tile = tile;

		//FIXME:This assumes we are 1x1!
		if(!tile.PlaceObject(obj)) {
			//For some reason, we weren't able to place our object in this tile.
			//Probably it was already occupied.

			//Do not return our newly instantiated object, instead it will be garbage collected.
			return null;
		}

		if (obj.LinksToNeighbour) {
			//Notify neighbours that i am here
			Tile t;
			int x = tile.X, y = tile.Y;

			/*for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					Debug.Log("Getting tile at: " + (i + x) + "," + )
					t = tile.world.GetTileAt(i + x, j + y);
					if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType) 
						t.furniture.cbOnChanged(t.furniture);
				}
			}*/
			t = tile.world.GetTileAt(x, y + 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x + 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x, y - 1);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x - 1, y);
			if (t != null && t.furniture != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
		}

		return obj;
	}

	public void RegisterOnChangedCallback(Action<Furniture> callbackFunc ) {
		cbOnChanged += callbackFunc;
	}

	public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc ) {
		cbOnChanged -= callbackFunc;
	}
}

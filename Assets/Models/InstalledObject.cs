using UnityEngine;
using System.Collections;
using System;

public class InstalledObject {

	public Tile Tile {
		get;
		protected set;
	}

	public string ObjectType {
		get;
		protected set;
	}

	int width, height;

	Action<InstalledObject> cbOnChanged;

	//This is a multiplier. So a value of 2 here, means you move twice as slowly. (i.e. at half speed)
	float movementCost;

	//TODO: Implement larger objects
	//TODO: Implement object rotation

	protected InstalledObject (){}

	//This is used by our object factory to create the prototypical object
	//It doesnt ask for a tile.
	static public InstalledObject CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1) {
		InstalledObject obj = new InstalledObject();
		obj.ObjectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	static public InstalledObject PlaceInstance (InstalledObject proto, Tile tile) {
		InstalledObject obj = new InstalledObject();

		obj.ObjectType = proto.ObjectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;

		obj.Tile = tile;

		//FIXME:This assumes we are 1x1!
		if(!tile.PlaceObject(obj)) {
			//For some reason, we weren't able to place our object in this tile.
			//Probably it was already occupied.

			//Do not return our newly instantiated object, instead it will be garbage collected.
			return null;
		}

		return obj;
	}

	public void RegisterOnChengedCallback(Action<InstalledObject> callbackFunc ) {
		cbOnChanged += callbackFunc;
	}

	public void UnregisterOnChengedCallback(Action<InstalledObject> callbackFunc ) {
		cbOnChanged -= callbackFunc;
	}
}

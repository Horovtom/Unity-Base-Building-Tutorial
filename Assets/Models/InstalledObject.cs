using UnityEngine;
using System.Collections;

public class InstalledObject {

	Tile tile;

	string objectType;

	int width, height;

	//This is a multiplier. So a value of 2 here, means you move twice as slowly. (i.e. at half speed)
	float movementCost;

	//TODO: Implement larger objects
	//TODO: Implement object rotation

	protected InstalledObject (){}

	//This is used by our object factory to create the prototypical object
	//It doesnt ask for a tile.
	static public InstalledObject CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1) {
		InstalledObject obj = new InstalledObject();
		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	static public InstalledObject PlaceObject (InstalledObject proto, Tile tile) {
		InstalledObject obj = new InstalledObject();

		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;

		obj.tile = tile;

		//FIXME:This assumes we are 1x1!
		if(!tile.PlaceObject(obj)) {
			//For some reason, we weren't able to place our object in this tile.
			//Probably it was already occupied.

			//Do not return our newly instantiated object, instead it will be garbage collected.
			return null;
		}

		return obj;
	}



}

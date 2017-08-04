using UnityEngine;
using System.Collections;

public class InstalledObject {

	Tile tile;

	string objectType;

	int width, height;

	//This is a multiplier. So a value of 2 here, means you move twice as slowly. (i.e. at half speed)
	float movementCost;

	//This is used by our object factory to create the prototypical object
	//It doesnt ask for a tile.
	public InstalledObject(string objectType, float movementCost = 1f, int width = 1, int height = 1) {
		this.objectType = objectType;
		this.movementCost = movementCost;
		this.width = width;
		this.height = height;
	}

	protected InstalledObject(InstalledObject proto, Tile tile) {
		this.objectType = proto.objectType;
		this.movementCost = proto.movementCost;
		this.width = proto.width;
		this.height = proto.height;

		this.tile = tile;

	}



}

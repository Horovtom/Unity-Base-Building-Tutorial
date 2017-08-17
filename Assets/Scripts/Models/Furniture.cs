using UnityEngine;
using System.Collections.Generic ;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Furniture : IXmlSerializable{

	public Dictionary<string, object> furnParameters;
	public Action<Furniture, float> updateActions;

	public void Update(float deltaTime) {
		if (updateActions != null) {
			updateActions(this, deltaTime);
		}
	}

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

	public Func<Tile, bool> funcPositionValidation {get; protected set;}

	//This is a multiplier. So a value of 2 here, means you move twice as slowly. (i.e. at half speed)
	public float MovementCost {
		get;
		protected set;
	}

	//This is used by our object factory to create the prototypical object
	//It doesnt ask for a tile.
	public Furniture(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false) {
		this.ObjectType = objectType;
		this.MovementCost = movementCost;
		this.width = width;
		this.height = height;
		this.LinksToNeighbour = linksToNeighbour;
		this.funcPositionValidation = this.__IsValidPosition;

		furnParameters = new Dictionary<string, object>();
	}

	virtual public Furniture Clone() {
		return new Furniture(this);
	}

	//Used to place furniture
	protected Furniture(Furniture other) {
		this.ObjectType = other.ObjectType;
		this.MovementCost = other.MovementCost;
		this.width = other.width;
		this.height = other.height;
		this.LinksToNeighbour = other.LinksToNeighbour;
		if (other.updateActions != null)
			this.updateActions = (Action<Furniture, float>)other.updateActions.Clone();
		this.furnParameters = new Dictionary<string, object>(other.furnParameters);
	}

	static public Furniture PlaceInstance (Furniture proto, Tile tile) {
		if (tile == null) {
			Debug.LogError("PlaceInstance -- tile is null");
		}

		if (proto.funcPositionValidation(tile) == false) {
			Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
			return null;
		}

		//We know our placement destination is valid

		Furniture obj = proto.Clone();
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
			if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x + 1, y);
			if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x, y - 1);
			if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.ObjectType == obj.ObjectType)
				t.furniture.cbOnChanged(t.furniture);
			t = tile.world.GetTileAt(x - 1, y);
			if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.ObjectType == obj.ObjectType)
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

	public bool IsValidPosition(Tile t)  {
		return funcPositionValidation(t);
	}

	//FIXME: These functions should never be called directly,
	// so they probably shouldn't be public functions
	public bool __IsValidPosition(Tile t) {
		//Make sure tile is FLOOR
		if (t.Type != TileType.Floor) 
			return false;
		

		//Make sure tile doesn't already have furniture on
		if (t.furniture != null)
			return false;


		return true;
	}

	public bool __IsValidPosition_Door(Tile t) {
		if (__IsValidPosition(t) == false)
			return false;

		//Make sure we have a pair of E/W walls or N/S walls

		return true;
	}



	//////////////////////////////////////////
	/// 		
	/// 			SAVING & LOADING
	/// 
	/////////////////////////////////////////

	#region Saving & Loading

	//TODO: Implement larger objects
	//TODO: Implement object rotation
	[System.Obsolete("Method is deprecated, should be used only by serialization")]
	public Furniture (){
		furnParameters = new Dictionary<string, object>();
	}

	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		//Save info here
		writer.WriteAttributeString("objectType", ObjectType);
		writer.WriteAttributeString("movementCost", MovementCost.ToString());
		writer.WriteAttributeString("X", Tile.X.ToString());
		writer.WriteAttributeString("Y", Tile.Y.ToString());

	}

	public void ReadXml(XmlReader reader) {
		//Load info here
		MovementCost = float.Parse(reader.GetAttribute("movementCost"));
	}

	#endregion
}

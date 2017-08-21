using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Tile : IXmlSerializable {

	Action<Tile> cbTileChanged;

	TileType _type = TileType.Empty;

	const float baseTileMovementCost = 1; //FIXME: This is just hardcoded for now

	public float MovementCost {
		get {
			if (Type == TileType.Empty)
				return 0;
			else if (furniture == null)
				return baseTileMovementCost;
			else
				return baseTileMovementCost * furniture.MovementCost;
		}
	}

	public TileType Type {
		get {
			return _type;
		}
		set {
			if (_type != value) {
				_type = value;
				//Call the callback and let things know we've changed
				if (cbTileChanged != null)
					cbTileChanged(this);
			}
		}
	}

	/// <summary>
	/// Returns true if you can enter this tile right this moment
	/// </summary>
	public Enterability GetEnterability() {
		if (MovementCost == 0)
			return Enterability.Never;

		//Check out furniture to see if it has a special block on enterability
		if (furniture != null && furniture.getEnterability != null) {
			return furniture.getEnterability(furniture);
		}

		return Enterability.Yes;
	}

	Inventory inventory;
	public Furniture furniture {
		get;
		protected set;
	}

	public World world {
		get;
		protected set;
	}
	int x, y;

	public Job pendingFurnitureJob;

	public int X {
		get {
			return x;
		} 

	}

	public int Y {
		get {
			return y;
		}
	
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">A World instance.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile (World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Registers the tile type changed callback.
	/// </summary>
	public void RegisterTileTypeChangedCallback (Action<Tile> callback) {
		//It behaves like an array, you can call this function multiple times
		this.cbTileChanged += callback;
	}

	/// <summary>
	/// Unregisters the tile type changed callback.
	/// </summary>
	public void UnregisterTileTypeChangedCallback (Action<Tile> callback) {
		this.cbTileChanged -= callback;
	}

	public bool PlaceObject (Furniture objInstance) {
		if (objInstance == null) {
			//We are uninstalling whatever was here before.
			furniture = null;
			return true;
		}

		//objInstance isn't null
		if (furniture != null) {
			Debug.LogError("Trying to assign a furniture to a tile that already has one!");
			return false;
		}

		furniture = objInstance;
		return true;
	}
		
	public bool IsNeighbour(Tile tile, bool diagOkay = false) {
		if (diagOkay)
			return Mathf.Abs(this.X - tile.X) <= 1 && Mathf.Abs(this.Y - tile.Y) <= 1 && (tile.X != this.X && tile.Y != this.Y);
		else
			return Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1;
	}

	ReadOnlyCollection<Tile>[] neighbours;

	/// <summary>
	/// Gets the neighbours. 
	/// </summary>
	/// <returns>
	/// The neighbours in an array of size 4 if diagOkay is false (N/E/S/W), 
	/// or size 8 if diagOkay is true (N/E/S/W/NE/SE/SW/NW). 
	/// </returns>
	/// <param name="diagOkay">If set to <c>true</c> diagonals are included okay.</param>
	public ReadOnlyCollection<Tile> GetNeighbours(bool diagOkay = false) {
		if (neighbours == null)
			PopulateNeighbours();
		return neighbours[diagOkay ? 1 : 0];
	}

	void PopulateNeighbours() {
		Tile[] neighbours4 = new Tile[4];
		Tile[] neighbours8 = new Tile[8];
		int[] xarr, yarr;
 
		xarr = new int[8]{ 0, 1, 0, -1, 1, 1, -1, -1 };
		yarr = new int[8]{ 1, 0, -1, 0, 1, -1, -1, 1 };

		Tile n;

		for (int i = 0; i < 8; i++) {
			n = world.GetTileAt(x + xarr[i], y + yarr[i]);
			if (i < 4)
				neighbours4[i] = n;
			neighbours8[i] = n;
		}

		neighbours = new ReadOnlyCollection<Tile>[2];
		neighbours[0] = new ReadOnlyCollection<Tile>(neighbours4);
		neighbours[1] = new ReadOnlyCollection<Tile>(neighbours8);
	}

	//////////////////////////////////////////
	/// 		
	/// 			SAVING & LOADING
	/// 
	/////////////////////////////////////////

	#region Saving & Loading

	public Tile() {}

	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		//Save info here
		writer.WriteAttributeString("X", x.ToString());
		writer.WriteAttributeString("Y", y.ToString());
		writer.WriteAttributeString("Type", ((int)Type).ToString());


	}

	public void ReadXml(XmlReader reader) {
		//Load info here
		/*x = int.Parse(reader.GetAttribute("X"));
		y = int.Parse(reader.GetAttribute("Y"));*/

		Type = (TileType)int.Parse(reader.GetAttribute("Type"));

	}

	#endregion
}

public enum TileType {
	Empty,
	Floor}
;

public enum Enterability {Yes, Never, Soon};

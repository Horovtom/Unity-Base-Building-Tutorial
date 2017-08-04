using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	public static WorldController Instance {
		get;
		protected set;
	}
	public Sprite floorSprite;

	Dictionary<Tile, GameObject> tileGameObjectMap;

	public World World {
		get; 
		protected set;
	}

	// Use this for initialization
	void Start () {

		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		if (Instance != null) {
			Debug.LogError("There should be only one WorldController!!!");
		}
		Instance = this;

		World = new World();


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

				tile_gameobject.AddComponent<SpriteRenderer>();
				//Just save lambda there
				tile_dataobject.RegisterTileTypeChangedCallback(OnTileTypeChanged);

			}
		}

		World.RandomizeTiles();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnTileTypeChanged(Tile tile_data) {
		if (tileGameObjectMap.ContainsKey(tile_data) == false) {
			Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		GameObject tile_go = tileGameObjectMap[tile_data];

		if (tile_go == null) {
			Debug.LogError("tileGameObjectMap returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
			return;
		}

		if (tile_data.Type == TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
		} else if (tile_data.Type == TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer>().sprite = null;
		} else {
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type!");
		}
	}

	public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		return WorldController.Instance.World.GetTileAt(x, y);
	}

}

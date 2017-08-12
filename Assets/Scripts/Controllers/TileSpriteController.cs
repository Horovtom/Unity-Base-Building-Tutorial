using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSpriteController: MonoBehaviour {


	public static WorldController Instance {
		get;
		protected set;
	}

	public Sprite floorSprite;
	public Sprite emptySprite;

	Dictionary<Tile, GameObject> tileGameObjectMap;

	World world {
		get {return WorldController.Instance.world;}
	}

	// Use this for initialization
	void Start () {
		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		//Create a GameObject for each of our tiles, so they show visually
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				Tile tile_dataobject = world.GetTileAt(x, y);
				//Get our tile game object (used in graphics)
				GameObject tile_gameobject = new GameObject();
				tileGameObjectMap.Add(tile_dataobject, tile_gameobject);

				tile_gameobject.name = "Tile(" + x + "," + y + ")";
				tile_gameobject.transform.position = new Vector3(tile_dataobject.X, tile_dataobject.Y, 0);
				tile_gameobject.transform.SetParent(this.transform, true);

				//Add a Sprite Renderer
				//Add a default sprite for empty tiles.
				SpriteRenderer sr = tile_gameobject.AddComponent<SpriteRenderer>();
				sr.sprite = emptySprite;
				sr.sortingLayerName = "Tiles";
			}
		}

		//Registering callback
		world.RegisterTileChanged(OnTileChanged);
	}

	void OnTileChanged (Tile tile_data) {
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
}

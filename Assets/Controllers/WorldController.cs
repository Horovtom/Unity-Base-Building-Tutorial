using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

	public Sprite floorSprite;

	World world;

	// Use this for initialization
	void Start () {

		world = new World();


		//Create a GameObject for each of our tiles, so they show visually
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				Tile tile_dataobject = world.GetTileAt(x, y);
				//Get our tile game object (used in graphics)
				GameObject tile_gameobject = new GameObject();

				tile_gameobject.name = "Tile(" + x + "," + y + ")";
				tile_gameobject.transform.position = new Vector3(tile_dataobject.X, tile_dataobject.Y, 0);

				tile_gameobject.AddComponent<SpriteRenderer>();

			}
		}

		world.RandomizeTiles();
	}

	float randomizeTileTimer = 2f;

	// Update is called once per frame
	void Update () {
		randomizeTileTimer -= Time.deltaTime;

		if (randomizeTileTimer < 0) {
			world.RandomizeTiles();
			randomizeTileTimer = 2f;
		}
	}

	void OnTileTypeChanged(Tile tile_data, GameObject tile_go) {
		if (tile_data.Type == Tile.TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
		} else if (tile_data.Type == Tile.TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer>().sprite = null;
		} else {
			Debug.LogError("OnTileTypeChanged - Unrecognized tile type!");
		}
	}

}

using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	float soundCooldown = 0;

	// Use this for initialization
	void Start () {
		WorldController.Instance.World.RegisterFurnitureCreated(OnFurnitureCreated);
		WorldController.Instance.World.RegisterTileChanged(OnTileChanged);
	}
	
	// Update is called once per frame
	void Update () {
		if (soundCooldown > 0) 
			soundCooldown -= Time.deltaTime;
	}

	void OnTileChanged(Tile tile_data) {
		//FIXME
		if (soundCooldown > 0)
			return;
		AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
		AudioSource.PlayClipAtPoint(ac, new Vector3(tile_data.X, tile_data.Y, Camera.main.transform.position.z));
		soundCooldown = 0.1f;
	}

	public void OnFurnitureCreated(Furniture furn) {
		if (furn == null) {
			Debug.LogError("OnFurnitureCreated -- furn is null");
		}
		if (soundCooldown > 0)
			return;

		AudioClip ac = Resources.Load<AudioClip>("Sounds/" + furn.ObjectType + "_OnCreated");

		if (ac == null) {
			//WTF? What do we do?
			//Since there's no specific sound for whatever Furniture this is, just use default sound
			Debug.Log("There is no sound file for " + furn.ObjectType);
			ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
		}
		AudioSource.PlayClipAtPoint(ac, new Vector3(furn.Tile.X, furn.Tile.Y, Camera.main.transform.position.z));
		soundCooldown = 0.1f;
	}
}

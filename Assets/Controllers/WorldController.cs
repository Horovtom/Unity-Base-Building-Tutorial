using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {


	public static WorldController Instance {
		get;
		protected set;
	}
		
	public World world {
		get; 
		protected set;
	}

	// Use this for initialization
	void OnEnable () {
		if (Instance != null) {
			Debug.LogError("There should be only one WorldController!!!");
		}
		Instance = this;

		world = new World();

		//Center the Camera
		Camera.main.transform.position = new Vector3(world.Width /2, world.Height/2, Camera.main.transform.position.z);
	}

	void Update() {
		//TODO: ADD pause/unpause, speed controls, etc...
		world.Update(Time.deltaTime);
	}

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		return WorldController.Instance.world.GetTileAt(x, y);
	}
}

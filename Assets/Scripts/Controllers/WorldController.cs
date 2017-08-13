using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour {


	public static WorldController Instance {
		get;
		protected set;
	}
		
	public World world {
		get; 
		protected set;
	}

	static bool loadWorld = false;

	// Use this for initialization
	void OnEnable () {
		if (Instance != null) {
			Debug.LogError("There should be only one WorldController!!!");
		}
		Instance = this;

		if (loadWorld) {
			loadWorld = false;
			CreateWorldFromSaveFile();
		} else 
			CreateEmptyWorld();
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

	public void NewWorld() {
		Debug.Log("NewWorld button was clicked.");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void SaveWorld() {
		Debug.Log("SaveWorld button was clicked.");

		XmlSerializer serializer = new XmlSerializer(typeof(World));
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, world);
		writer.Close();

		Debug.Log(writer.ToString());

	}

	public void LoadWorld() {
		Debug.Log("LoadWorld button was clicked.");

		loadWorld = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void CreateWorldFromSaveFile() {
		world = new World(100,100);

		//Center the Camera
		Camera.main.transform.position = new Vector3(world.Width /2, world.Height/2, Camera.main.transform.position.z);
	}

	void CreateEmptyWorld() {
		world = new World(100,100);

		//Center the Camera
		Camera.main.transform.position = new Vector3(world.Width /2, world.Height/2, Camera.main.transform.position.z);
	}
}

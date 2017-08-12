using UnityEngine;
using System.Collections.Generic;
using System;

public class CharacterSpriteController : MonoBehaviour {

	Dictionary<Character, GameObject> characterGameObjectMap;
	Dictionary<string, Sprite> characterSprites;

	World world {
		get {return WorldController.Instance.world;}
	}

	// Use this for initialization
	void Start () {
		LoadSprites();

		characterGameObjectMap = new Dictionary<Character, GameObject>();


		//Registering callback
		world.RegisterCharacterCreated(OnCharacterCreated);

		//DEBUG
		world.CreateCharacter(world.GetTileAt(world.Width / 2, world.Height / 2));
	}

	void LoadSprites() {
		characterSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters");

		foreach (Sprite s in sprites) {
			characterSprites[s.name] = s;
		}
	}

	public void OnCharacterCreated (Character obj) {
		//Create a visual GameObject linked to this data.
		GameObject char_go = new GameObject();


		characterGameObjectMap.Add(obj, char_go);
		char_go.name = "Character";
		char_go.transform.position = new Vector3(obj.currTile.X, obj.currTile.Y, 0);
		char_go.transform.SetParent(this.transform, true);

		//FIXME: We assume that the object must be a wall, so use the hardcoded reference to the wall sprite.
		char_go.AddComponent< SpriteRenderer>().sprite = characterSprites["p1_front"];

		char_go.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";

		//obj.RegisterOnChangedCallback(OnCharacterChanged);
	}

	/*public void OnCharacterChanged (Furniture furn) {
		//Make sure the furniture's graphics are correct.
		if (characterGameObjectMap.ContainsKey(furn) == false) {
			Debug.LogError("OnFurnitureChanged - Trying to change visuals for furniture not in our map.");
			return;
		}

		GameObject furn_go = characterGameObjectMap[furn];
		furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForCharacter(furn);
	}*/
}

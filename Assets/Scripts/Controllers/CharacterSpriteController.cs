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
		Character c = world.CreateCharacter(world.GetTileAt(world.Width / 2, world.Height / 2));
		//c.SetDestination(world.GetTileAt(world.Width / 2 + 5, world.Height / 2));
	}

	void LoadSprites() {
		characterSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters");

		foreach (Sprite s in sprites) {
			characterSprites[s.name] = s;
		}
	}

	public void OnCharacterCreated (Character character) {
		//Create a visual GameObject linked to this data.
		GameObject char_go = new GameObject();


		characterGameObjectMap.Add(character, char_go);
		char_go.name = "Character";
		char_go.transform.position = new Vector3(character.X, character.Y, 0);
		char_go.transform.SetParent(this.transform, true);

		//FIXME: We assume that the object must be a wall, so use the hardcoded reference to the wall sprite.
		char_go.AddComponent< SpriteRenderer>().sprite = characterSprites["p1_front"];

		char_go.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";

		character.RegisterOnChangedCallback(OnCharacterChanged);
	}

	public void OnCharacterChanged (Character character) {
		//Make sure the furniture's graphics are correct.
		if (characterGameObjectMap.ContainsKey(character) == false) {
			Debug.LogError("OnCharacterChanged - Trying to change visuals for character not in our map.");
			return;
		}

		GameObject char_go = characterGameObjectMap[character];
		//furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForCharacter(character);

		char_go.transform.position = new Vector3(character.X, character.Y, 0);
	}
}

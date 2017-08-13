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

		//Check for pre-existing characters, which won't do the callback.
		foreach(Character c in world.characters) {
			OnCharacterCreated(c);
		}
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

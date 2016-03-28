using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCollection : MonoBehaviour {

	[System.Serializable]
	public class Character{
		public string identifierName;
		public string displayName;
		public string characterSpriteSheetName;
		public Sprite characterThumb;
		public Sprite[] presentSprites;
		public AudioClip taunt;
		public bool taken = false;
	}

	public Character[] characters;

	private List<Character> characterCollection;

	// Use this for initialization
	void Awake () {
		characterCollection = new List<Character>();
		foreach(Character character in characters){
			characterCollection.Add(character);
		}
	}
		
	public Character GetFirstOpenCharacter(){
		foreach(Character character in characters){
			if(character.taken == false){
				character.taken = true;
				return character;
			}
		}
		return null;
	}

	public Character GetNextOpenCharacter(Character currentCharacter){
		Character nextCharacter = null;
		int characterPosition = characterCollection.IndexOf(currentCharacter);
		characters[characterPosition].taken = false;

		while(nextCharacter == null){
			if(characterPosition == characterCollection.Count - 1){
				characterPosition = 0;
			}else{
				characterPosition++;
			}

			if(characterCollection[characterPosition].taken == false){
				nextCharacter = characterCollection[characterPosition];
				characters[characterPosition].taken = true;
			}
		}
		return nextCharacter;
	}

	public Character GetPreviousOpenCharacter(Character currentCharacter){
		Character prevCharacter = null;
		int characterPosition = characterCollection.IndexOf(currentCharacter);
		characters[characterPosition].taken = false;

		while(prevCharacter == null){
			if(characterPosition == 0){
				characterPosition = characterCollection.Count-1;
			}else{
				characterPosition--;
			}

			if(characterCollection[characterPosition].taken == false){
				prevCharacter = characterCollection[characterPosition];
				characters[characterPosition].taken = true;
			}
		}
		Debug.Log(prevCharacter.displayName);
		return prevCharacter;
	}

	private List<Character> GetAvailableCharacters(){
		List<Character> availableCharacters = characterCollection;
		foreach(Character character in availableCharacters){
			if(character.taken){
				availableCharacters.Remove(character);
			}
		}
		return availableCharacters;

	}

}

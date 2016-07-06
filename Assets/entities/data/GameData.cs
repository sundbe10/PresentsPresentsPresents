using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameData : Singleton<GameData> {

	private CharacterCollection.Character[] playerCharacters;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		playerCharacters = new CharacterCollection.Character[4];
	}

	void Start(){
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	static public void SetCharacter(int playerNumber, CharacterCollection.Character character){
		Instance.playerCharacters[playerNumber-1] = character;
	}

	static public CharacterCollection.Character GetCharacter(int playerNumber){
		return Instance.playerCharacters[playerNumber-1];
	}

	static public void RemoveCharacter(int playerNumber){
		Instance.playerCharacters[playerNumber-1] = null;
	}

	static public void SetLevel(){
		//Set Level
	}

}

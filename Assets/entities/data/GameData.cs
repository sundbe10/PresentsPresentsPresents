using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameData : Singleton<GameData> {

	private CharacterCollection.Character[] playerCharacters;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		InitializePlayerCharacters();
	}

	void Start(){
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLevelWasLoaded(int level){
		//If navigating to a scene other than the game screen, reset all player choices for character selections
		if(level == 1) InitializePlayerCharacters();
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

	void InitializePlayerCharacters(){
		playerCharacters = new CharacterCollection.Character[4];
	}

}

using UnityEngine;
using System.Collections;

public class GamePlayersController : MonoBehaviour {

	[System.Serializable]
	public class PlayerPosition{
		public float position;
	}

	public GameObject playerObject;
	public int numberOfPlayers = 4;
	public PlayerPosition[] playerPositions;

	int maxPlayers = 4;

	// Use this for initialization
	void Start () {
		SetPlayers();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetPlayers(){
		for(int i = 1; i <= maxPlayers; i++){
			//Add players based on the current number of players
			CharacterCollection.Character character = GameData.GetCharacter(i);
			if(character != null){
				GameObject newPlayer = Instantiate(playerObject, new Vector3(playerPositions[i-1].position,0,0), Quaternion.identity) as GameObject;
				newPlayer.GetComponentInChildren<PlayerController>().InitializePlayer(i, character);
			}
		}
	}
}

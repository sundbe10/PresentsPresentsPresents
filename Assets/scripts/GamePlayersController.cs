using UnityEngine;
using System.Collections;

public class GamePlayersController : MonoBehaviour {

	public int numberOfPlayers = 2;

	float worldWidth = 500;
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

			//Position/Remove players based on the current number of players
			GameObject player = GameObject.Find("Player "+i+" Group");
			GameObject playerScore = GameObject.Find ("Player "+i+" Score");
			CharacterCollection.Character character = GameData.GetCharacter(i);
			if(character != null){
				//float startX = worldWidth*i/(numberOfPlayers+1)-worldWidth/2;
				//player.transform.position = new Vector3(startX, player.transform.position.y, player.transform.position.z);
				player.GetComponentInChildren<PlayerController>().SetCharacter(character);
			}else{
				Destroy(player);
				Destroy (playerScore);
			}

			//Set specific player score positions for 1 or 2 players
			if(numberOfPlayers == 2){
				//TODO center P1/P2 scores around time
			}else if(numberOfPlayers == 1){
				//TODO: Figure out score position for 1 player
			}

			//TODO Set characters based on character selection
		}
	}
}

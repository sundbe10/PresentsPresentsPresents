using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfirmCharacterSelect : MonoBehaviour {

	private GameObject[] players;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool AllPlayersReady(){
		players = GameObject.FindGameObjectsWithTag("Player");
		//If no active player, return false
		if(players.Length == 0) return false;
		//Determine if active players are ready
		bool ready = true;
		foreach(GameObject player in players){
			if(!player.GetComponent<CharacterSelectController>().IsReady()){
				ready = false;
			}
		}
		return ready;
	}
	public List<int> GetActivePlayers(){
		players = GameObject.FindGameObjectsWithTag("Player");
		List<int> playerNumbers = new List<int>();
		//If no active player, return empty
		if(players.Length == 0) return playerNumbers;
		//Determine if active players are ready
		foreach(GameObject player in players){
			playerNumbers.Add(player.GetComponent<CharacterSelectController>().playerNumber);
		}
		return playerNumbers;
	}

	public bool NoActivePlayers(){
		players = GameObject.FindGameObjectsWithTag("Player");
		return players.Length == 0;
	}
		
}

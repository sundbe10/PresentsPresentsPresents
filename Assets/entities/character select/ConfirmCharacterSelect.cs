using UnityEngine;
using System.Collections;

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
		Debug.Log(players.Length);
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

	public bool NoActivePlayers(){
		players = GameObject.FindGameObjectsWithTag("Player");
		return players.Length == 0;
	}
		
}

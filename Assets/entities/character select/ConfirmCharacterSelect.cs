using UnityEngine;
using System.Collections;

public class ConfirmCharacterSelect : MonoBehaviour {

	private GameObject[] players;
	private SceneLoader _sceneLoader = new SceneLoader();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Start")){
			if(AllPlayersReady()){
				GameObject.Find("Fade").GetComponent<FadeOut>().StartFade();
				StartCoroutine(GoToGame());
			}
		}
	}

	bool AllPlayersReady(){
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

	IEnumerator GoToGame(){
		yield return new WaitForSeconds(1.5f);
		_sceneLoader.LoadScene("Game");
	}
}

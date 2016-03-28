using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddPlayer : MonoBehaviour {

	enum State{
		ADD,
		SELECTING,
		READY,
		DEAD
	}

	public string input;
	public GameObject playerObject;
	public int playerNumber;

	private State _state;
	private Text text;

	// Use this for initialization
	void Start () {
		_state = State.ADD;
		text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ADD:
			HandleAddPlayer();
			break;
		case State.SELECTING:
			HandlePlayerSelecting();
			break;
		case State.READY:
			HandlePlayerReady();
			break;
		}
	}

	void HandleAddPlayer(){
		text.text = "PRESS\nA\nTO START";
		if(Input.GetButtonDown(input)){
			GameObject newPlayer = Instantiate(playerObject, new Vector3(transform.position.x, transform.position.y-30,0), Quaternion.identity) as GameObject;
			newPlayer.GetComponent<CharacterSelectController>().SetPlayerNumber(playerNumber);
			_state = State.SELECTING;
		}
	}

	void HandlePlayerSelecting(){
		text.text = "";
		if(Input.GetButtonDown(input)){
			_state = State.READY;
		}
	}

	void HandlePlayerReady(){
		text.text = "READY";
	}
}

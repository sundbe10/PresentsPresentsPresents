using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddPlayer : MonoBehaviour {

	enum State{
		ADD,
		SELECTING,
		READY
	}

	public string startText = "JOIN GAME";
	public GameObject playerObject;
	public int playerNumber;
	public AudioClip selectionSound;
	public AudioClip joinSound;
	public AudioClip backSound;

	CharacterSelectController newPlayerController;
	CharacterCollection.Character currentCharacter;
	AudioSource audioSource;
	State _state;
	Text text;
	bool axisButtonDown = false;

	// Use this for initialization
	void Awake () {
		_state = State.ADD;
		text = gameObject.GetComponent<Text>();
		audioSource = gameObject.GetComponent<AudioSource>();
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
		HandleBack();
	}

	//Reset all players when navigating to charcter select screen
	void OnLevelWasLoaded(int level) {
		if (level == 1){
			GameData.RemoveCharacter(playerNumber);
		}
	}

	void HandleAddPlayer(){
		text.text = startText;
		if(Input.GetButtonDown("Start_P"+playerNumber) || Input.GetButtonDown("Throw_P"+playerNumber)){
			currentCharacter = CharacterCollection.GetFirstOpenCharacter(playerNumber);
			GameObject newPlayer = Instantiate(playerObject, new Vector3(transform.position.x, transform.position.y-40,0), Quaternion.identity) as GameObject;
			newPlayerController = newPlayer.GetComponent<CharacterSelectController>();
			newPlayerController.SetCharacter(currentCharacter);
			newPlayerController.playerNumber = playerNumber;
			_state = State.SELECTING;
			gameObject.GetComponent<Blink>().StopBlink();
			PlaySound(joinSound);
		}
	}

	void HandlePlayerSelecting(){
		//Select Character
		if(Input.GetAxis("Horizontal_P"+playerNumber) != 0.0f && !axisButtonDown){
			var axis = Input.GetAxis("Horizontal_P"+playerNumber);
			if(axis > 0){
				currentCharacter = CharacterCollection.GetNextOpenCharacter(playerNumber);
			}
			else if(axis < 0){
				currentCharacter = CharacterCollection.GetPreviousOpenCharacter(playerNumber);
			}
			newPlayerController.SetCharacter(currentCharacter);
			PlaySound(selectionSound);
			axisButtonDown = true;
		}
		//Select Costume
		if(Input.GetAxis("Vertical_P"+playerNumber) != 0.0f && !axisButtonDown){
			var axis = -Input.GetAxis("Vertical_P"+playerNumber);
			if(axis > 0){
				currentCharacter = CharacterCollection.GetNextOpenCostume(playerNumber);
			}
			else if(axis < 0){
				currentCharacter = CharacterCollection.GetPreviousOpenCostume(playerNumber);
			}

			newPlayerController.SetCharacter(currentCharacter);
			PlaySound(selectionSound);
			axisButtonDown = true;
		}
		//Show character name
		text.text = currentCharacter.locked ? "???" : currentCharacter.displayName.ToUpper();
		//Finalize Selection
		if(Input.GetButtonDown("Throw_P"+playerNumber) && !currentCharacter.locked){
			PlaySound(currentCharacter.taunt);
			newPlayerController.FinalizeSelection();
			GameData.SetCharacter(playerNumber, currentCharacter);
			gameObject.GetComponent<Blink>().StartBlink();
			_state = State.READY;
		}
		//Reset axis button down
		if(Input.GetAxis("Horizontal_P"+playerNumber) == 0 && Input.GetAxis("Vertical_P"+playerNumber) == 0){
			axisButtonDown = false;
		}
	}

	void HandlePlayerReady(){
		text.text = "READY";
	}

	void HandleBack(){
		if(Input.GetButtonDown("Back_P"+playerNumber)){
			switch(_state){
			case State.READY:
				_state = State.SELECTING;
				newPlayerController.RemoveSelection();
				gameObject.GetComponent<Blink>().StopBlink();
				break;
			case State.SELECTING:
				_state = State.ADD;
				CharacterCollection.DeselectCharacter(playerNumber);
				newPlayerController.RemoveCharacter();
				gameObject.GetComponent<Blink>().StartBlink();
				break;
			}
			PlaySound(backSound);
		}
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}

using UnityEngine;
using System.Collections;

public class CharacterSelectController : MonoBehaviour {

	enum State{
		ACTIVE,
		SELECTED
	}

	public int playerNumber;
	public AudioClip entranceSound;
	public AudioClip selectionSound;
	public AudioClip confirmationSound;

	private SpriteSwitch spriteSwitch;
	private CharacterCollection characterCollection;
	private CharacterCollection.Character currentCharacter;
	private AudioSource audioSource;
	private bool characterSelected = false;
	private Animator avatarAnimator;
	private State _state;
	private GameData gameData;

	// Use this for initialization
	void Start () {
		_state = State.ACTIVE;
		spriteSwitch = gameObject.GetComponent<SpriteSwitch>();
		audioSource = gameObject.GetComponent<AudioSource>();
		avatarAnimator = transform.Find("porthole/avatar").gameObject.GetComponent<Animator>();
		characterCollection = GameObject.Find("CharacterCollection").GetComponent<CharacterCollection>();
		gameData = GameObject.FindGameObjectWithTag("Game Data").GetComponent<GameData>();
		SetCharacter(characterCollection.GetFirstOpenCharacter());
		PlaySound(entranceSound);
	}

	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ACTIVE:
			HandleCharacterSelection();
			break;
		case State.SELECTED:
			break;
		}
	}

	public void SetPlayerNumber(int player){
		playerNumber = player;
	}

	public bool IsReady(){
		return _state == State.SELECTED;
	}

	void HandleCharacterSelection(){
		if(Input.GetButtonDown("Horizontal_P"+playerNumber)){
			var axis = Input.GetAxis("Horizontal_P"+playerNumber);
			if(axis > 0){
				SetCharacter(characterCollection.GetNextOpenCharacter(currentCharacter));
			}
			else if(axis < 0){
				SetCharacter(characterCollection.GetPreviousOpenCharacter(currentCharacter));
			}
			PlaySound(selectionSound);
		}
		if(Input.GetButtonDown("Throw_P"+playerNumber)){
			_state = State.SELECTED;
			PlaySound(currentCharacter.taunt);
			avatarAnimator.CrossFade("selected", 0f);
			SelectCharacter();
		}
	}

	void SetCharacter(CharacterCollection.Character character){
		currentCharacter = character;
		spriteSwitch.SetSpriteSheet(currentCharacter.characterSpriteSheetName);
	}

	void SelectCharacter(){
		gameData.SetCharacter(playerNumber, currentCharacter);
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

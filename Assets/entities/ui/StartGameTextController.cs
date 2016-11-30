using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGameTextController : MonoBehaviour {

	public AudioClip playersReady;

	enum State{
		IDLE,
		READY
	}

	State _state = State.IDLE;
	ConfirmCharacterSelect characterManager;
	AudioSource audioSource;
	Text text;

	// Use this for initialization
	void Start () {
		characterManager = GameObject.Find("CharacterManager").GetComponent<ConfirmCharacterSelect>();
		audioSource = gameObject.GetComponent<AudioSource>();
		text = gameObject.GetComponent<Text>();
		text.color = new Color32(255,255,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.IDLE && characterManager.AllPlayersReady()){
			PlaySound(playersReady);
			text.color = new Color32(255,255,0,255);
			_state = State.READY;
		}else if(_state == State.READY && !characterManager.AllPlayersReady()){
			text.color = new Color32(255,255,0,0);
			_state = State.IDLE;
		}
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

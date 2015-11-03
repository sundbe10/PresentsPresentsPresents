using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour {

	//State 
	enum State {
		DEAD,
		COUNTING
	}

	//Public vars
	public AudioClip lowTick;
	public AudioClip highTick;

	//Private vars
	float countdownTimer = 0;
	Text text;
	AudioSource audioSource;
	int countdownNumber = 3;
	State _state; 

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		text = gameObject.GetComponent<Text>();
		text.text = "";
		_state = State.DEAD;
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
			case State.COUNTING:
				text.enabled = true;
				countdownTimer += Time.deltaTime;
				if(countdownTimer >  4 && countdownNumber == -1){
					_state = State.DEAD;
				}else if(countdownTimer > 3 && countdownNumber == 0){
					text.text = "GO";
					PlaySound (highTick);
					countdownNumber--;
				}else if(countdownTimer > 2 && countdownNumber == 1){
					text.text = "1";
					PlaySound (lowTick);
					countdownNumber--;
				}else if(countdownTimer > 1 && countdownNumber == 2){
					text.text = "2";
					PlaySound (lowTick);
					countdownNumber--;
				}else if(countdownTimer > 0 && countdownNumber == 3){
					text.text = "3";
					PlaySound (lowTick);
					countdownNumber--;
				}
				break;
			case State.DEAD:
				text.enabled = false;
				break;
		}

	}


	//Public Functions
	public int StartTimer(){
		_state = State.COUNTING;
		return countdownNumber;
	}


	//Private Functions
	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound,1f);
	}
	
}

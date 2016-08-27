using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreController : MonoBehaviour {

	enum State {
		ENTRY,
		ENDED
	};

	public AudioClip blip;
	public AudioClip confirmLetter;
	public AudioClip negateLetter;

	State _state = State.ENTRY;
	int letterNumber = 1;
	int playerNumber = 1;
	Text currentLetterText;
	string currentLetter;
	AudioSource audioSource;
	string name = "";
	int score;
	string characterSpriteSheet;

	// Use this for initialization
	void Start () {
		SetCurrentLetter();
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.ENTRY) ChooseName();
	}

	public void SetPlayerInfo(int highScore, string spriteSheet){
		score = highScore;
		characterSpriteSheet = spriteSheet;
		gameObject.GetComponentInChildren<SpriteSwitch>().SetSpriteSheet(spriteSheet);
	}

	void ChooseName(){
		if(Input.GetButtonDown("Vertical_P"+playerNumber)){
			var axis = Input.GetAxis("Vertical_P"+playerNumber);
			char c = currentLetter[0];
			if(axis > 0 && currentLetter != "Z"){
				c++;
			}
			else if(axis < 0 && currentLetter != "A"){
				c--;
			}
			//if(c < 65) c = 90;
			//if(c > 90) c = 65;
			currentLetterText.text = currentLetter = c.ToString();
			PlaySound(blip);
		}
		if(Input.GetButtonDown("Throw_P"+playerNumber)){
			if(letterNumber < 3){
				PlaySound(confirmLetter);
				currentLetterText.gameObject.GetComponent<Blink>().StopBlink();
				letterNumber++;
				SetCurrentLetter();
			}else if(letterNumber == 3){
				PlaySound(confirmLetter);
				letterNumber++;
				currentLetterText.gameObject.GetComponent<Blink>().StopBlink();
				name = GetFinalName();
				Leaderboard.SetHighScore(new Leaderboard.Leader(name, score, characterSpriteSheet));
				GameController.CompleteHighScoreEntry();
			}
		}
		if(Input.GetButtonDown("Back_P"+playerNumber)){
			PlaySound(negateLetter);
			if(letterNumber > 1 && letterNumber < 4){
				currentLetterText.gameObject.GetComponent<Blink>().StopBlink();
				currentLetterText.text = "A";
				letterNumber--;
				SetCurrentLetter();
			}
		}
	}


	string GetFinalName(){
		string enteredName = "";
		for(int i = 1; i<4; i++){
			enteredName += GameObject.Find("Name "+i).GetComponent<Text>().text;
		}
		return enteredName;
	}

	void SetCurrentLetter(){
		currentLetterText = GameObject.Find("Name "+letterNumber).GetComponent<Text>();
		currentLetterText.gameObject.GetComponent<Blink>().StartBlink();
		currentLetter = currentLetterText.text;
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

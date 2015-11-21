using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	//State
	enum State {
		PLAYING,
		ENDED
	};

	//Public vars
	public int introTime = 3;
	public int gameTime = 60;


	//Private vars
	GameObject countdownObject;
	GameObject timerObject;
	GameAudioController audioController;
	State _state = State.PLAYING;

	// Use this for initialization
	void Start () {
		audioController = gameObject.GetComponent<GameAudioController>();
		countdownObject = GameObject.Find("Countdown");
		timerObject = GameObject.Find("Timer");
		SetTimerText ();
		//Initiate countdown
		StartCoroutine(StartCountdown());
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.ENDED){
			if(Input.GetButton("Throw_P1")){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}


	//Public Functions


	//Private Functions
	IEnumerator StartCountdown(){
		yield return new WaitForSeconds(introTime);
		int countdownTime = countdownObject.GetComponent<CountdownController>().StartTimer();
		yield return new WaitForSeconds(countdownTime);
		EnablePlayers();
		StartCoroutine(IncrementTimer());
	}

	void EnablePlayers(){
		GameObject[] players = FindObjectsOfType<GameObject>();
		foreach(GameObject player in players){
			if(player.tag == "Player"){
				player.GetComponent<PlayerController>().EnablePlayer();
			}
		}
	}

	IEnumerator IncrementTimer(){
		yield return new WaitForSeconds(1);
		gameTime--;
		SetTimerText();
		if(gameTime == 0){
			DeclareWinner();
		}else{
			if(gameTime <= 5) TimerWarning();
			StartCoroutine(IncrementTimer());
		}
	}
	
	void SetTimerText(){
		timerObject.GetComponent<Text>().text = gameTime.ToString();
	}

	void TimerWarning(){
		audioController.TimerWarning();
		timerObject.GetComponent<Text>().color = Color.red;
		timerObject.GetComponent<Shadow>().effectColor = new Color(0.6f,0f,0f,1f);
	}

	void DeclareWinner(){

		//Stop music and play fall sound effect
		audioController.PlayFallSound();

		//Disable Kids
		GameObject[] kids = GameObject.FindGameObjectsWithTag("Kid");
		foreach(GameObject kid in kids){
			kid.GetComponent<KidController>().DisableKid();
		}

		//Calculate Winner and kill losers
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject highScorer = null;
		int highScore = 0;

		foreach(GameObject player in players){
			int playerScore = player.GetComponent<PlayerController>().GetScore();
			//Player has a higher Score
			if(playerScore > highScore){
				if(highScorer) highScorer.GetComponent<PlayerController>().FallPlayer();
				highScorer = player;
				highScore = highScorer.GetComponent<PlayerController>().GetScore();
			}
			//Player is tied for highest Score
			else if(playerScore == highScore){
				player.GetComponent<PlayerController>().FallPlayer();
				if(highScorer) highScorer.GetComponent<PlayerController>().FallPlayer();
				highScorer = null;
			}
			//Player has a lower score
			else{
				player.GetComponent<PlayerController>().FallPlayer();
			}
		}

		//Set Winner
		StartCoroutine(AnnounceWinner(highScorer));
	}

	IEnumerator AnnounceWinner(GameObject player){
		Text winnerText = GameObject.Find("Winner Text").GetComponent<Text>();
		winnerText.enabled = true;
		if(player){
			player.GetComponent<PlayerController>().DeclareWinner();
			yield return new WaitForSeconds(3);
			winnerText.text = player.GetComponent<PlayerController>().GetName();
			audioController.WinText();
			yield return new WaitForSeconds(1);
			winnerText.text += "\nWINS";
			audioController.WinText();
			_state = State.ENDED;
		}else{
			yield return new WaitForSeconds(3);
			winnerText.text = "DRAW";
			audioController.WinText();
			_state = State.ENDED;
		}
	}

}

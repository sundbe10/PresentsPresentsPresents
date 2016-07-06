using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {

	//State
	enum State {
		PLAYING,
		PAUSED,
		ENDED,
		POSTEND
	};

	public enum Mode{
		CLASSIC,
		BATTLE,
		SURVIVAL
	}

	//Public vars
	public int introTime = 3;
	public int gameTime = 60;
	public int playToScore = 5000;
	public GameObject pauseMenu;
	public GameObject endMenu;
	public Mode _mode;

	//Private vars
	private GameObject countdownObject;
	private GameObject timerObject;
	private GameAudioController audioController;
	private State _state = State.PLAYING;
	private GameObject menu;

	// Use this for initialization
	void Start () {
		audioController = gameObject.GetComponent<GameAudioController>();
		countdownObject = GameObject.Find("Countdown");
		timerObject = GameObject.Find("Timer");
		switch(Instance._mode){
		case Mode.BATTLE:
		case Mode.SURVIVAL:
			SetTimerText(-1);
			break;
		default:
			SetTimerText(gameTime);
			break;
		}
		//Initiate countdown
		StartCoroutine(StartCountdown());
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.PLAYING){
			if(Input.GetButtonDown("Start")){
				_state = State.PAUSED;
				audioController.PlayPauseSound();
				menu = Instantiate(pauseMenu, Vector3.zero, Quaternion.identity) as GameObject;
				menu.transform.parent = GameObject.Find("Game Canvas").transform;
				PauseGame();
			}
		}else if(_state == State.ENDED){
			if(Input.GetButtonDown("Confirm")){
				if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")){
					menu = Instantiate(endMenu, Vector3.zero, Quaternion.identity) as GameObject;
					menu.transform.parent = GameObject.Find("Game Canvas").transform;
					audioController.PlayPauseSound();
					_state = State.POSTEND;
				}
			}
		}
	}

	//Static
	static public int GetMaxGameScore(){
		return Instance.playToScore;
	}
	static public int GetInitialGameScore(){
		switch(Instance._mode){
		case Mode.BATTLE:
			return 0;
		case Mode.SURVIVAL:
			return Instance.playToScore;
		default:
			return 0;
		}
	}
	static public void RecordPlayerScore(int playerNum, int score){
	
	}
	static public bool GameIsActive(){
		return Instance._state == State.PLAYING;
	}
	static public void UnpauseGame(){
		Instance.ResumeGame();
	}


	//Public Functions
	public void PauseGame(){
		audioController.PauseMusic();
		Time.timeScale = 0;
	}
		
	public void ResumeGame(){
		_state = State.PLAYING;
		audioController.ResumeMusic();
		Time.timeScale = 1.0f;
	}

	//Private Functions
	IEnumerator StartCountdown(){
		yield return new WaitForSeconds(introTime);
		int countdownTime = countdownObject.GetComponent<CountdownController>().StartTimer();
		yield return new WaitForSeconds(countdownTime);
		EnablePlayers();
		if(gameTime > 0){
			StartCoroutine(IncrementTimer());
		}
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
		SetTimerText(gameTime);
		if(gameTime == 0){
			DeclareWinner();
		}else{
			if(gameTime <= 5) TimerWarning();
			StartCoroutine(IncrementTimer());
		}
	}
	
	void SetTimerText(int time){
		string timeString = time == -1 ? "" : time.ToString(); 
		timerObject.GetComponent<Text>().text = timeString;
	}

	void TimerWarning(){
		audioController.TimerWarning();
		timerObject.GetComponent<Text>().color = Color.red;
		timerObject.GetComponent<Shadow>().effectColor = new Color(0.6f,0f,0f,1f);
	}

	void DeclareWinner(){

		//Stop music and play fall sound effect
		audioController.PlayFallSound();

		//TODO Change diabling to an event

		//Disable Kids
		GameObject[] kids = GameObject.FindGameObjectsWithTag("Kid");
		foreach(GameObject kid in kids){
			kid.GetComponent<KidController>().DisableKid();
		}

		//Disable Bullies
		GameObject[] bullies = GameObject.FindGameObjectsWithTag("Bully");
		foreach(GameObject bully in bullies){
			bully.GetComponent<BullyController>().DisableBully();
		}

		//End TODO

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

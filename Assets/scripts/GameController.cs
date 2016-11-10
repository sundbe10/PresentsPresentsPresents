using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {

	//State
	enum State {
		PLAYING,
		PAUSED,
		ENDED,
		POSTEND,
		DONE,
		HIGHSCORE
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
	public GameObject highScore;
	public Mode _mode;

	//Private vars
	GameObject countdownObject;
	GameObject timerObject;
	GameObject winner;
	GameAudioController audioController;
	State _state = State.PLAYING;
	GameObject menu;

	//Events
	public delegate void OnGameStateChangeDelegate ();
	public static event OnGameStateChangeDelegate onGameStartEvent;
	public static event OnGameStateChangeDelegate onGameEndEvent;
	public static event OnGameStateChangeDelegate onGamePauseEvent;
	public static event OnGameStateChangeDelegate onGameResumeEvent;

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
				onGamePauseEvent();
				audioController.PlayPauseSound();
				menu = Instantiate(pauseMenu, Vector3.zero, Quaternion.identity) as GameObject;
				menu.transform.parent = GameObject.Find("Game Canvas").transform;
				PauseGame();
			}
		}else if(_state == State.POSTEND){
			if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")){
				if(GameStats.GetUnlockedCharacter() == null){
					menu = Instantiate(endMenu, Vector3.zero, Quaternion.identity) as GameObject;
					menu.transform.parent = GameObject.Find("Game Canvas").transform;
					audioController.PlayPauseSound();
					_state = State.DONE;
				}else{
					SceneLoader.GoToScene("CharacterUnlock",true);
				}
			}
		}else if(_state == State.HIGHSCORE){
			if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")){
				GameObject.Find("Winner Text").GetComponent<Text>().text = "";
				menu = Instantiate(highScore, new Vector3(0,40f,0), Quaternion.identity) as GameObject;
				menu.transform.parent = GameObject.Find("Game Canvas").transform;
				PlayerController winnerController = winner.GetComponent<PlayerController>();
				int winnerScore = winnerController.GetScore();
				string winnerCharacter = winnerController.currentCharacter.characterSpriteSheetName;
				menu.GetComponent<HighScoreController>().SetPlayerInfo(winnerScore, winnerCharacter);
				audioController.PlayPauseSound();
				_state = State.DONE;
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
		//TODO Get player score, save to an array, update score UI appropriately
	}
	static public bool GameIsActive(){
		return Instance._state == State.PLAYING;
	}
	static public void UnpauseGame(){
		Instance.ResumeGame();
	}
	static public void CompleteHighScoreEntry(){
		Instance._state = State.DONE;
		if(GameStats.GetUnlockedCharacter() == null){
			SceneLoader.GoToScene("Leaderboard_game",true);
		}else{
			SceneLoader.GoToScene("CharacterUnlock",true);
		}
	}
	static public int GetWinnerNumber(){
		if(Instance.winner != null){
			return Instance.winner.GetComponent<PlayerController>().playerNum;
		}else{
			return 0;
		}
	}

	//Public Functions
	public void PauseGame(){
		audioController.PauseMusic();
		Time.timeScale = 0;
	}
		
	public void ResumeGame(){
		_state = State.PLAYING;
		onGameResumeEvent();
		audioController.ResumeMusic();
		Time.timeScale = 1.0f;
	}

	//Private Functions
	IEnumerator StartCountdown(){
		yield return new WaitForSeconds(introTime);
		int countdownTime = countdownObject.GetComponent<CountdownController>().StartTimer();
		yield return new WaitForSeconds(countdownTime);
		onGameStartEvent();
		if(gameTime > 0){
			StartCoroutine(IncrementTimer());
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
		winner = null;
		//Stop music and play fall sound effect. Send end game event.
		audioController.PlayFallSound();
		onGameEndEvent();

		//Calculate Winner and kill losers
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject highScorer = null;
		int highScore = 0;

		foreach(GameObject player in players){
			PlayerController playerController = player.GetComponent<PlayerController>();
			int playerScore = playerController.GetScore();
			//Player has a higher Score
			if(playerScore > highScore){
				if(highScorer) highScorer.GetComponent<PlayerController>().FallPlayer();
				highScorer = player;
				highScore = playerScore;
			}
			//Player is tied for highest Score
			else if(playerScore == highScore){
				if(highScorer) highScorer.GetComponent<PlayerController>().FallPlayer();
				playerController.FallPlayer();
				highScorer = null;
			}
			//Player loses
			else{
				playerController.FallPlayer();
			}
		}

		//Set Winner
		winner = highScorer;
		_state = State.ENDED;
		StartCoroutine(AnnounceWinner(winner));

		//Save game stats
		GameStats.IncrementStat(GameStats.Stat.GamesPlayed);
		GameStats.SaveStats();
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
			if(Leaderboard.IsNewHighScore(player.GetComponent<PlayerController>().GetScore())){
				_state = State.HIGHSCORE;
			}else{
				_state = State.POSTEND;
			}
		}else{
			yield return new WaitForSeconds(3);
			winnerText.text = "DRAW";
			audioController.WinText();
		}
	}

}

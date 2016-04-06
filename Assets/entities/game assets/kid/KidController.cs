using UnityEngine;
using System.Collections;

public class KidController : MonoBehaviour {

	//State
	enum State{
		ENTERING,
		WALKING,
		RUNNING,
		DISABLED
	}

	//Public Vars
	public int scoreValue = 100;
	public float walkSpeedMin = 0.5f;
	public float walkSpeedMax = 1f;
	public string[] possibleSprites;
	public GameObject[] powerups;
	public float powerupProbability = 0.1f;
	public AudioClip runSound;
	public GameObject scoreText;

	//Private Vars
	GameObject powerup;
	float walkSpeed;
	float runSpeed = 2.5f;
	AudioSource audioSource;
	State _state = State.ENTERING;
	bool gameEnded = false;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();

		//Set random walk speed
		walkSpeed = Random.Range(walkSpeedMin, walkSpeedMax);

		//Set random walk left or right
		SetRandomDirection();

		//Set Random sprite
		string randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
		SpriteSwitch switcher = gameObject.GetComponent<SpriteSwitch>();
		switcher.character = randomSprite;

		//Give Powerup
		if(Random.value < powerupProbability){
			GivePowerup();
		}

		//Start Entry
		StartCoroutine(EnterKid());
	}
	
	// Update is called once per frame
	void Update () {
		MoveKid();
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "Destroyer"){
			RemoveKid ();
		}
	}


	//Public Functions
	public bool KidHasPresent(){
		return _state != State.WALKING;
	}

	public void PresentCaught( GameObject thrownObject, GameObject thrower){
		//Score
		int multiplier = thrower.GetComponent<PlayerController>().IncrementScore(scoreValue);
		//Show score text
		CreateScoreText(multiplier);
		//Hold new object
		HoldObject (thrownObject);
		//Powerup

		if(powerup){
			powerup.GetComponent<ApplyPowerupController>().ApplyPowerup(thrower);
		}	PlaySound (runSound);
		if(walkSpeed > 0 ){
			walkSpeed = runSpeed;
		}else{
			walkSpeed = -runSpeed;
		}
		_state = State.RUNNING;
	}

	public void DisableKid(){
		gameEnded = true;
		if(_state == State.WALKING){
			_state = State.DISABLED;
		}
	}


	//Private Functions
	void SetRandomDirection(){
		float leftOrRight = Random.value;
		Vector3 worldPosition = Camera.main.WorldToScreenPoint(transform.position);
		float percentAcrossScreen = worldPosition.x / Screen.width;
		
		if(transform.position.x <= -Screen.width/2){
			//Walk Right (default)
			transform.localScale = new Vector3(1,1,1);
			walkSpeed = Mathf.Abs(walkSpeed);
		}
		else if(transform.position.x > Screen.width/2 || leftOrRight < percentAcrossScreen){
			//Walk Left
			transform.localScale = new Vector3(-1,1,1);
			walkSpeed = -Mathf.Abs(walkSpeed);
		}
	}

	void MoveKid(){
		if(_state == State.ENTERING){
			transform.position = new Vector3(transform.position.x, transform.position.y + -0.2f, transform.position.z);
		}else{
			transform.position = new Vector3(transform.position.x+walkSpeed, transform.position.y, transform.position.z);
		}
	}

	void RemoveKid(){
		Destroy(gameObject);
	}

	void GivePowerup(){
		Debug.Log ("Powerup Created...");
		GameObject randomPowerup = powerups[Random.Range(0, powerups.Length)];
		powerup = Instantiate(randomPowerup, transform.position, Quaternion.identity) as GameObject;
		HoldObject(powerup);
	}

	void HoldObject(GameObject heldObject){
		heldObject.transform.parent = transform;
		heldObject.transform.position = transform.position + new Vector3(6*transform.localScale.x,-2,-1);
	}

	void CreateScoreText(int multiplier){
		GameObject newScoreText = Instantiate(scoreText, transform.position, Quaternion.identity) as GameObject;
		newScoreText.GetComponent<ScoringTextController>().SetText(scoreValue, multiplier);
	}

	IEnumerator EnterKid(){
		yield return new WaitForSeconds(0.4f);
		SetRandomDirection();
		_state = gameEnded ? State.DISABLED : State.WALKING;
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

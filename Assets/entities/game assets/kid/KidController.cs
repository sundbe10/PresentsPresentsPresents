using UnityEngine;
using System.Collections;

public class KidController : MonoBehaviour {

	//State
	enum State{
		ENTERING,
		WALKING,
		RUNNING,
		HIT,
		CRYING,
		DISABLED
	}

	//Public Vars
	public int scoreValue = 100;
	public float walkSpeedMin = 0.5f;
	public float walkSpeedMax = 1f;
	public float runSpeed = 100f;
	public string[] possibleSprites;
	public GameObject[] powerups;
	public float powerupProbability = 0.1f;
	public AudioClip runSound;
	public GameObject scoreText;

	//Private Vars
	GameObject powerup;
	float walkSpeed;
	AudioSource audioSource;
	State _state = State.ENTERING;
	bool gameEnded = false;
	Animator animator;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		animator = gameObject.GetComponent<Animator>();

		//Set random walk speed
		walkSpeed = Random.Range(walkSpeedMin, walkSpeedMax);

		//Set random walk left or right
		SetRandomDirection();

		//Set Random sprite
		string randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
		SpriteSwitch switcher = gameObject.GetComponent<SpriteSwitch>();
		switcher.SetSpriteSheet(randomSprite);

		//Give Powerup
		if(Random.value < powerupProbability){
			GivePowerup();
		}

		//Start Entry
		StartCoroutine(EnterKid());
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ENTERING:
		case State.WALKING:
		case State.RUNNING:
		case State.CRYING:
		case State.DISABLED:
			MoveKid();
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.CompareTag("Destroyer")){
			RemoveKid ();
		}else if(collider.CompareTag("Hit Box") && _state == State.WALKING){
			HitKid(collider);
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
		}	

		PlaySound (runSound);
		if(walkSpeed > 0 ){
			walkSpeed = runSpeed;
		}else{
			walkSpeed = -runSpeed;
		}
		_state = State.RUNNING;
	}

	public void CoalCaught(GameObject thrownObject, GameObject thrower){
		//Show score text
		thrower.GetComponent<PlayerController>().IncrementScore(-scoreValue);
		//Show score text
		CreateScoreText(-1);
		//Hold new object
		HoldObject (thrownObject);
		//Powerup
		if(powerup){
			powerup.GetComponent<ApplyPowerupController>().ApplyPowerup(thrower);
		}

		CryKid();
	}

	public void DisableKid(){
		gameEnded = true;
		if(_state == State.WALKING){
			_state = State.DISABLED;
		}
	}

	public void CryKid(){
		gameObject.GetComponent<Animator>().SetBool("isCrying",true);
		_state = State.CRYING;
		PlaySound (runSound);

		//Flip tears if kid is flipped
		Transform[] tears = transform.Find("Body").GetComponentsInChildren<Transform>(true);
		foreach(Transform tear in tears){
			if(tear.gameObject.name != "Body" && transform.localScale.x == -1){
				tear.rotation = new Quaternion(0, 180, 0, 0);
			}
		}

		if(walkSpeed > 0 ){
			walkSpeed = runSpeed;
		}else{
			walkSpeed = -runSpeed;
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
			transform.position = new Vector3(transform.position.x, transform.position.y + -8f*Time.deltaTime, transform.position.z);
		}else{
			transform.position = new Vector3(transform.position.x+walkSpeed*Time.deltaTime, transform.position.y, transform.position.z);
		}
	}

	void RemoveKid(){
		Destroy(gameObject);
	}

	void HitKid(Collider2D collider){
		_state = State.HIT;
		float hitDirection = collider.transform.parent.localScale.x;
		if(hitDirection != transform.localScale.x){
			transform.localScale = new Vector2(hitDirection, 1);
			walkSpeed = -walkSpeed;
		}
		animator.CrossFade("Fall", 0f);

		//Remove powerup
		if(powerup){
			Destroy(powerup);
		}
	}

	void GivePowerup(){
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

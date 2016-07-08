using UnityEngine;
using System.Collections;

public class BullyController : MonoBehaviour {

	//State
	enum State{
		ENTERING,
		IDLE,
		WALKING,
		RUNNING,
		FIGHTING,
		STUNNED
	}

	//Public Vars
	public float aggression = 0.2f;
	public float walkSpeed = 15f;
	public float runSpeed = 25f;
	public string[] possibleSprites;
	public AudioClip runSound;
	public AudioClip gruntSound;
	public AudioClip coalCatchSound;
	public AudioClip presentCatchSound;
	public GameObject scoreText;
	public int scoreValue = 100;
	public GameObject throwingObject;

	//Private Vars
	AudioSource audioSource;
	State _state = State.ENTERING;
	Animator animator;
	float aiTimer = 0;
	bool disabled = false;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		animator = gameObject.GetComponent<Animator>();

		//Set random walk left or right
		SetRandomDirection();

		//Set Random sprite
		string randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
		SpriteSwitch switcher = gameObject.GetComponent<SpriteSwitch>();
		switcher.SetSpriteSheet(randomSprite);

	}

	void Awake(){
		if(!GameController.GameIsActive()){
			DisableBully();
		}
	}

	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ENTERING:
		case State.RUNNING:
			MoveBully();
			break;
		case State.WALKING:
			AIControl();
			MoveBully();
			break;
		case State.IDLE:
			AIControl();
			break;
		case State.FIGHTING:
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.CompareTag("Destroyer")){
			RemoveBully ();
		}else if(collider.CompareTag("Kid") && !disabled){
			switch(_state){
			case State.WALKING:
			case State.IDLE:
				TryToHit();
				break;
			}
		}else if(collider.CompareTag("Present") && !disabled){
			switch(_state){
			case State.WALKING:
			case State.IDLE:
			case State.FIGHTING:
				PresentCaught(collider.gameObject);
				break;
			}
		}else if(collider.CompareTag("Ground") && _state == State.ENTERING){
			SetAsActive();
		}else if(collider.CompareTag("Hit Box")){
			if(!collider.transform.IsChildOf(transform)){
				StartCoroutine("RunBully");
			}
		}
	}

	//Public functions
	public void HitKid(){
		//turn off collider if a kid is hit. This will hopefully keep the number of kids able to be hit by a single punch to 1-2.
		transform.FindChild("Hit Box").GetComponent<BoxCollider2D>().enabled = false;
	}
	public void Grunt(){
		PlaySound(gruntSound);
	}
	public void PuchCooldown(){
		_state = State.IDLE;
	}
	public void DisableBully(){
		disabled = true;
	}


	//Private Functions
	void MoveBully(){
		if(_state == State.ENTERING){
			transform.position = new Vector3(transform.position.x, transform.position.y + -4f*Time.deltaTime, transform.position.z);
		}else{
			transform.position = new Vector3(transform.position.x+walkSpeed*Time.deltaTime, transform.position.y, transform.position.z);
		}
	}

	void PresentCaught( GameObject thrownObject ){

		PresentController presentController = thrownObject.GetComponent<PresentController>();
		PlayerController throwerController = presentController.GetThrower().GetComponent<PlayerController>();
		if(presentController.IsCaught()) return;

		//Score
		int multiplier;
		if(!presentController.IsPresent()){
			multiplier = throwerController.IncrementScore(scoreValue);
			StartCoroutine("RunBully");
		}else{
			multiplier = throwerController.IncrementScore(-scoreValue);
			StartCoroutine("StunBully");
		}

		//Show score text
		CreateScoreText(multiplier);
		//Catch new object
		presentController.SetCaught(false);

	}

	void TryToHit(){
		if(Random.value <= aggression){
			_state = State.FIGHTING;
			transform.Find("Hit Box").transform.localScale = new Vector2(transform.localScale.x, 1);
			animator.CrossFade("Push", 0f);
		}
	}

	void ThrowObject(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject targetPlayer = players[Mathf.FloorToInt(Random.Range(0,players.Length))];
		//Turn bully toward player
		transform.localScale = new Vector2(targetPlayer.transform.position.x < transform.position.x ? -1 : 1, 1);
		//Create and throw projectile
		GameObject thrownObject = Instantiate(throwingObject, transform.position + new Vector3(transform.localScale.x,1,0)*10, Quaternion.identity) as GameObject;
		Vector2 throwVector = targetPlayer.transform.position - thrownObject.transform.position;
		thrownObject.GetComponent<SnowballController>().SetVelocity(new Vector2(throwVector.x*0.7f, throwVector.y).normalized);
	}

	IEnumerator StunBully(){
		PlaySound(coalCatchSound);
		animator.CrossFade("Stunned", 0f);
		_state = State.STUNNED;	

		yield return new WaitForSeconds(1.5f);

		_state = State.IDLE;
	}

	IEnumerator RunBully(){
		PlaySound(presentCatchSound);
		animator.CrossFade("Stunned", 0f);
		_state = State.STUNNED;

		yield return new WaitForSeconds(1f);

		animator.CrossFade("Run", 0f);
		_state = State.RUNNING;

		PlaySound (runSound);
		if(walkSpeed > 0 ){
			walkSpeed = runSpeed;
		}else{
			walkSpeed = -runSpeed;
		}
	}

	void SetRandomDirection(){
		float leftOrRight = Mathf.Round(Random.value);
		Vector3 worldPosition = Camera.main.WorldToScreenPoint(transform.position);
		int direction = 1;
		if(worldPosition.x < Screen.width*1/8){
			//Walk Right
			direction = 1;
		}
		else if(worldPosition.x > Screen.width*7/8 || leftOrRight == 1){
			//Walk Left
			direction = -1;
		}

		transform.localScale = new Vector3(direction,1,1);
		walkSpeed = direction *Mathf.Abs(walkSpeed);
	}

	void RemoveBully(){
		Destroy(gameObject);
	}

	void AIControl(){
		//Set AI timer
		aiTimer += Time.deltaTime;
		if(aiTimer < 2){
			return;
		}else{
			aiTimer = 0;
		}

		float stateNum = Random.value;

		//Set Random State
		SetRandomDirection();
		if(stateNum < 0.5f){
			ThrowObject();
		}
		else if(stateNum < 0.7f){
			_state = State.WALKING;
			animator.CrossFade("Walk", 0f);
		}
		else{
			_state = State.IDLE;
			animator.CrossFade("Idle", 0f);
		}
	}

	void SetAsActive(){
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
		SetRandomDirection();
		_state = State.WALKING;
	}

	void CreateScoreText(int multiplier){
		GameObject newScoreText = Instantiate(scoreText, transform.position, Quaternion.identity) as GameObject;
		newScoreText.GetComponent<ScoringTextController>().SetText(scoreValue, multiplier);
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	//State
	enum State{
		ENTRY,
		MOVE_ONLY,
		THROW_ONLY,
		ACTIVE,
		WINDING,
		FALLING,
		DEAD,
		CELEBRATE
	}

	enum AnimatorState{
		IDLE,
		RUNNING,
		JUMPING,
		LANDING,
		THROWING,
		WINDING,
		HIT
	}

	//Public vars
	public AudioClip throwSound;
	public AudioClip catchSound;
	public AudioClip tauntSound;
	public AudioClip jumpSound;
	public AudioClip multiplier2x;
	public AudioClip multiplier3x;
	public AudioClip multiplier4x;
	public float moveSpeed = 4f;
	public float throwSpeed = 1f;
	public float jumpSpeed = -10f;
	public float throwVelocity = 200f;
	public GameObject present;
	public int playerNum = 1;	
	public Sprite[] presentSprites;
	public string name;
	public Hashtable playerAttrs = new Hashtable();
	public LayerMask groundCheckMask;
	public enum Attributes{
		SPEED,
		THROWSPEED,
		THROWVELOCITY,
		FROZEN
	}

	//Private vars
	Animator animator;
	Animator bodyAnimator;
	AudioSource audioSource;
	Rigidbody2D rigidBody;
	bool canThrow = true;
	bool canJump = true;
	int catches = 0;
	int playerScore = 0;
	int scoreMultiplier = 1;
	State _state = State.ENTRY;

	// Use this for initialization
	void Awake () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();
		bodyAnimator = transform.Find ("Body").gameObject.GetComponent<Animator>();
		bodyAnimator.logWarnings = false;
		//Populate Attrs Hashtable
		playerAttrs.Add (Attributes.SPEED, moveSpeed);
		playerAttrs.Add (Attributes.THROWSPEED, throwSpeed);
		playerAttrs.Add (Attributes.FROZEN, State.DEAD);
		playerAttrs.Add (Attributes.THROWVELOCITY, throwVelocity);
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ENTRY:
			break;
		case State.MOVE_ONLY:
			animator.applyRootMotion = false;
			DetectGround();
			MovePlayer();
			Jump();
			break;
		case State.ACTIVE:
			DetectGround();
			MovePlayer();
			Jump();
			ThrowPresent();
			break;
		case State.WINDING:
			DetectGround();
			Jump();
			ThrowPresent();
			MoveTarget();
			if(canJump){
				PivotPlayer();
			}else{
				MovePlayer();
			}
			if(rigidBody.velocity.x == 0){
				SetBodyAnimationState(AnimatorState.IDLE);
			}
			break;
		case State.FALLING:
			break;
		case State.DEAD:
			break;
		}

		//Print buttons for debugging
//		foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode))){
//			if (Input.GetKeyDown(kcode)) Debug.Log("KeyCode down: " + kcode);
//		}
	}

	void LateUpdate(){
		if(bodyAnimator.GetBool("isThrowing") == true){
			bodyAnimator.SetBool("isThrowing",false);
		}
	}

	//Public Functions
	public void SetCharacter(CharacterCollection.Character character){
		transform.parent.gameObject.GetComponent<SpriteSwitch>().SetSpriteSheet(character.characterSpriteSheetName);
		presentSprites = character.presentSprites;
	}

	public void EnableMovement(){
		_state = State.MOVE_ONLY;
	}
	
	public void EnablePlayer(){
		_state = State.ACTIVE;
	}

	public void FallPlayer(){
		_state = State.FALLING;
		animator.SetBool("FALLING",true);
		transform.Find("Body/Flames").gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void KillPlayer(){
		_state = State.DEAD;
		bodyAnimator.SetBool("DEAD",true);
	}

	public void DeclareWinner(){
		_state = State.CELEBRATE;
	}
	
	public int IncrementScore(int score){
		catches++;
		switch (catches){
		case 4:
			scoreMultiplier = 2;
			PlaySound(multiplier2x);
			break;
		case 6:
			scoreMultiplier = 3;
			PlaySound(multiplier3x);
			break;
		case 8:
			scoreMultiplier = 4;
			PlaySound(multiplier4x);
			break;
		}
		PlaySound(catchSound);

		int scoreIncrement = score*scoreMultiplier;
		playerScore += scoreIncrement;
		return scoreMultiplier;
	}
	
	public void RemoveMultiplier(){
		scoreMultiplier = 1;
		catches = 0;
	}

	public int GetMultipiler(){
		return scoreMultiplier;
	}

	public int GetScore(){
		return playerScore;
	}

	public string GetName(){
		return name;
	}

	public void ApplyPowerup(Attributes attribute, float multiplier, float timeout){
		Debug.Log ("Increase "+name+" "+attribute+" by "+multiplier.ToString());
		StartCoroutine(PowerupTimeout(attribute, multiplier, timeout));
	}

	//Private Functions
	void MovePlayer(){
		float deltaX = 0;
		//TODO: Set player to move with incline of platforms, eliminate jitter
		if(Input.GetAxis("Horizontal_P"+playerNum) > 0 && Camera.main.WorldToScreenPoint(transform.position).x < Screen.width*0.975f){
			deltaX = (float) playerAttrs[Attributes.SPEED];
			transform.localScale = new Vector3(1,1,1);
			SetBodyAnimationState(AnimatorState.RUNNING);
		}else if(Input.GetAxis("Horizontal_P"+playerNum) < 0 && Camera.main.WorldToScreenPoint(transform.position).x > Screen.width*0.025f){
			deltaX = -(float) playerAttrs[Attributes.SPEED];
			transform.localScale = new Vector3(-1,1,1);
			SetBodyAnimationState(AnimatorState.RUNNING);
		}else{
			SetBodyAnimationState(AnimatorState.IDLE);
		}
		if(deltaX != 0){
			rigidBody.velocity = new Vector2(deltaX, rigidBody.velocity.y);
		}
	}

	void MoveTarget(){
		var x = Input.GetAxis("Horizontal_P"+playerNum)*25f + 5 + transform.position.x ;
		var y = -Input.GetAxis("Vertical_P"+playerNum)*25f + transform.position.y;
		Transform target = transform.Find("target");
		target.position = new Vector3(x, y, 0);
	}

	void PivotPlayer(){
		if(Input.GetAxis("Horizontal_P"+playerNum) > 0){
			transform.localScale = new Vector3(1,1,1);
		}else if(Input.GetAxis("Horizontal_P"+playerNum) < 0){
			transform.localScale = new Vector3(-1,1,1);
		}
	}

	void Jump(){
		float deltaX = 0;
		if(Input.GetButtonDown("Jump_P"+playerNum) && canJump){
			SetBodyAnimationState(AnimatorState.JUMPING);
			PlaySound(jumpSound);
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
			canJump = false;
		}
	}

	void ThrowPresent(){
		if(Input.GetButton("Throw_P"+playerNum) && canThrow){
			SetBodyAnimationState(AnimatorState.WINDING);
		}
		if(Input.GetButtonUp("Throw_P"+playerNum) && canThrow && bodyAnimator.GetBool("isWinding")){
			//Play throw sound
			PlaySound(throwSound);
			SetBodyAnimationState(AnimatorState.THROWING);

			//Create Present
			Vector3 initialPosition = transform.position + new Vector3(8*transform.localScale.x,-5,0);
			GameObject newPresent = Instantiate(present, initialPosition, Quaternion.identity) as GameObject;
			PresentController presentController = newPresent.GetComponent<PresentController>();
			presentController.SetThrower(gameObject);
			presentController.SetPresentSprite(presentSprites[UnityEngine.Random.Range (0, presentSprites.Length)]);

			//Set Present velocity
			Vector2 relativeVelocity;
			var xVelocity = rigidBody.velocity.x;
			var yVelocity = rigidBody.velocity.y;
			if(Input.GetAxis("Horizontal_P"+playerNum) > 0){
				relativeVelocity = new Vector2(throwVelocity,0);
			}else if(Input.GetAxis("Horizontal_P"+playerNum) < 0){
				relativeVelocity = new Vector2(-throwVelocity,0);
			}else{
				relativeVelocity = new Vector2(0,-throwVelocity);
			}
			presentController.SetVelocity(relativeVelocity);

			//Prevent player from being able to throw immidiately 
			canThrow = false;
			StartCoroutine(ThrowCooldown());
		}
	}

	void DetectGround(){
		//Test Diagonally from bottom
		RaycastHit2D hitLeft = Physics2D.Raycast(transform.position-new Vector3(9f,12f,0), new Vector2(-1,-1), 5f, groundCheckMask);
		RaycastHit2D hitRight = Physics2D.Raycast(transform.position-new Vector3(-9f,12f,0), new Vector2(1,-1), 5f, groundCheckMask);
		Debug.DrawLine(transform.position-new Vector3(9f,12f,0), transform.position-new Vector3(11.7f,14.7f,0), Color.red);
		Debug.DrawLine(transform.position-new Vector3(-9f,12f,0), transform.position-new Vector3(-11.7f,14.7f,0), Color.red);
		if (hitLeft || hitRight){
			canJump = true;
			SetBodyAnimationState(AnimatorState.LANDING);
		}else{
			canJump = false;
			SetBodyAnimationState(AnimatorState.JUMPING);
		}
			
	}

	void SetBodyAnimationState(AnimatorState state){
		switch(state){
		case AnimatorState.IDLE:
			bodyAnimator.SetBool("isRunning",false);
			break;
		case AnimatorState.RUNNING:
			bodyAnimator.SetBool("isRunning",true);
			break;
		case AnimatorState.JUMPING:
			bodyAnimator.SetBool("isJumping",true);
			break;
		case AnimatorState.LANDING:
			bodyAnimator.SetBool("isJumping",false);
			break;
		case AnimatorState.WINDING:
			_state = State.WINDING;
			bodyAnimator.SetBool("isWinding",true);
			break;
		case AnimatorState.THROWING:
			bodyAnimator.SetBool("isWinding",false);
			bodyAnimator.SetBool("isThrowing",true);
			_state = State.ACTIVE;
			break;
		}
	}


	IEnumerator ThrowCooldown(){
		yield return new WaitForSeconds((float) playerAttrs[Attributes.THROWSPEED]);
		SetBodyAnimationState(AnimatorState.IDLE);
		canThrow = true;
	}

	IEnumerator PowerupTimeout(Attributes attribute, float multiplier, float timeout){
		var attributeType = playerAttrs[attribute].GetType();
		if(attributeType == typeof(float)){
			playerAttrs[attribute] = (float) playerAttrs[attribute] * multiplier;
			yield return new WaitForSeconds(timeout);
			playerAttrs[attribute] = (float) playerAttrs[attribute] /  multiplier;
		}else if(attributeType == typeof(State)){
			_state = (State)playerAttrs[attribute];
			yield return new WaitForSeconds(timeout);
			if(_state == (State)playerAttrs[attribute]) _state = State.ACTIVE;
		}
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}

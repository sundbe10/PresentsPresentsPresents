using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour {

	//State
	enum State{
		ENTRY,
		MOVE_ONLY,
		THROW_ONLY,
		ACTIVE,
		DASHING,
		HIT,
		STUNNED,
		FALLING,
		DEAD,
		CELEBRATE
	}

	//Public vars
	public AudioClip throwSound;
	public AudioClip catchSound;
	public AudioClip badCatchSound;
	public AudioClip dashSound;
	public AudioClip swapSound;
	public AudioClip stunnedSound;
	public AudioClip multiplier2x;
	public AudioClip multiplier3x;
	public AudioClip multiplier4x;
	public float moveSpeed = 4f;
	public float dashForce = 500000f;
	public float dashDelay = 0.2f;
	public float hitForce = 200000f;
	public float throwSpeed = 1f;
	public float presentSpeed = 150f;
	public GameObject present;
	public int playerNum = 1;	
	public CharacterCollection.Character currentCharacter = null;
	public string name;
	public Hashtable playerAttrs = new Hashtable();
	public string playerLayer;
	public string playerDashLayer;
	public enum Attributes{
		SPEED,
		THROWSPEED,
		PRESENTSPEED,
		FROZEN,
		DASHDELAY,
		CONTROLDIRECTION
	}

	//Private vars
	Animator animator;
	Animator bodyAnimator;
	Animator dashAnimator;
	bool disabled = false;
	AudioSource audioSource;
	Rigidbody2D rigidBody;
	bool canThrow = true;
	int catches = 0;
	int maxScore;
	int playerScore;
	int scoreMultiplier = 1;
	State _state = State.ENTRY;
	State _prevState;
	GameObject playerScoreGroup;
	PlayerScoreController playerScoreBar = null;
	Text playerNameText;
	Transform playerTag;

	// Use this for initialization
	void Awake () {
		animator = gameObject.GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();
		bodyAnimator = transform.Find ("Body").gameObject.GetComponent<Animator>();
		dashAnimator = transform.Find ("Body/Dash").gameObject.GetComponent<Animator>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		playerTag = transform.Find("Body/tag").transform;
		bodyAnimator.logWarnings = false;
		//Get correct score component
		maxScore = GameController.GetMaxGameScore();
		playerScore = GameController.GetInitialGameScore();
		playerScoreGroup = transform.parent.FindChild("Canvas/Player Score").gameObject;
		playerScoreBar = playerScoreGroup.GetComponent<PlayerScoreController>();
		playerNameText = playerScoreGroup.transform.FindChild("Player Name").gameObject.GetComponent<Text>();
		//Populate Attrs Hashtable
		playerAttrs.Add (Attributes.SPEED, moveSpeed);
		playerAttrs.Add (Attributes.THROWSPEED, throwSpeed);
		playerAttrs.Add (Attributes.PRESENTSPEED, presentSpeed);
		playerAttrs.Add (Attributes.FROZEN, State.DEAD);
		playerAttrs.Add (Attributes.DASHDELAY, dashDelay);
		playerAttrs.Add (Attributes.CONTROLDIRECTION, 1f); 
	}

	void Start(){
		playerScoreBar.SetInitialScore(playerScore, maxScore, scoreMultiplier);
		GameController.onGameStartEvent += EnablePlayer;
		GameController.onGameEndEvent += DisablePlayer;
		GameController.onGamePauseEvent += PausePlayer;
		GameController.onGameResumeEvent += ResumePlayer;
	}
	
	// Update is called once per frame
	void Update () {
		if(!disabled){
			switch(_state){
			case State.ENTRY:
				break;
			case State.MOVE_ONLY:
				MovePlayer();
				Dash();
				break;
			case State.THROW_ONLY:
				ThrowPresent();
				break;
			case State.ACTIVE:
				MovePlayer();
				Dash();
				ThrowPresent();
				Score();
				break;
			case State.HIT:
				ThrowPresent();
				Score();
				break;
			case State.STUNNED:
				Score();
				break;
			case State.FALLING:
				break;
			case State.DEAD:
				break;
			}
		}
	}

	void LateUpdate(){
		if(bodyAnimator.GetBool("isThrowing") == true){
			bodyAnimator.SetBool("isThrowing",false);
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.CompareTag("Player Hit Box")){
			Hit(collider);
		}
	}

	void OnCollisionEnter2D(Collision2D collision){
		Debug.Log(collision.collider);
		if(collision.collider.CompareTag("Hit Box") && !IsDashing() && _state != State.STUNNED){
			StunPlayer();
		}
	}

	//Public Functions
	public void InitializePlayer(int _playerNum, CharacterCollection.Character _character){
		playerNum = _playerNum;
		name = playerNameText.text = _character.displayName;
		transform.parent.gameObject.GetComponent<SpriteSwitch>().SetSpriteSheet(_character.characterSpriteSheetName);
		currentCharacter = _character;
	}

	public void EnableMovement(){
		ChangeState(State.MOVE_ONLY);
	}
	
	public void EnablePlayer(){
		_prevState = State.ACTIVE;
		if(_state == State.MOVE_ONLY)
			ChangeState(State.ACTIVE);
	}

	public void DisablePlayer(){
		_prevState = State.DEAD;
		ChangeState(State.DEAD);
	}

	public void PausePlayer(){
		disabled = true;
	}

	public void ResumePlayer(){
		disabled = false;
	}

	public void FallPlayer(){
		ChangeState(State.FALLING);
		animator.SetBool("FALLING",true);
		transform.Find("Body/Flames").gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void KillPlayer(){
		ChangeState(State.DEAD);
		bodyAnimator.SetBool("DEAD",true);
	}

	public void DeclareWinner(){
		ChangeState(State.CELEBRATE);
	}
	
	public int IncrementScore(int score){
		int scoreIncrement;
		if(scoreMultiplier == -1) scoreMultiplier = 1;
		if(score > 0){
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
			scoreIncrement = score*scoreMultiplier;
		}else{
			RemoveMultiplier();
			scoreMultiplier = -1;
			scoreIncrement = score;
		}
			
		playerScore += scoreIncrement;
		//Limit score values
		if(playerScore > maxScore){
			playerScore = maxScore;
		}else if(playerScore < 0){
			playerScore = 0;
		}

		return scoreMultiplier;
	}

	public void IncrementScorePassive(int score){
		playerScore += score;
		//Limit score values
		if(playerScore > maxScore){
			playerScore = maxScore;
		}else if(playerScore < 0){
			playerScore = 0;
		}
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

	public float GetDirection(){
		return transform.localScale.x;
	}

	public void ApplyPowerup(Attributes attribute, float multiplier, float timeout){
		Debug.Log ("Increase "+name+" "+attribute+" by "+multiplier.ToString());
		//TODO: Add in switch statement for different object types (float and bool at least)
		StartCoroutine(PowerupTimeout(attribute, multiplier, timeout));
	}

	public bool IsDashing(){
		return _state == State.DASHING;
	}

	//Private Functions
	void Score(){
		if(playerScoreBar != null) playerScoreBar.SetScore(playerScore, scoreMultiplier);
	}

	void ChangeState(State state){
		if(_state == State.MOVE_ONLY || _state == State.ACTIVE || _state == State.DEAD) _prevState = _state;
		_state = state;
	}


	/***** Moving *****/

	void MovePlayer(){
		if(Input.GetAxis("Horizontal_P"+playerNum) != 0.0f){
			float maxSpeed = (float) playerAttrs[Attributes.SPEED];
			int direction = 1;
			float axis = Input.GetAxis("Horizontal_P"+playerNum) * (float)playerAttrs[Attributes.CONTROLDIRECTION];
			if(axis > 0){
				direction = 1;
			}else if(axis < 0){
				direction = -1;
			}
			transform.localScale = new Vector3(direction,1,1);
			playerTag.localScale = new Vector3(direction,1,1);
			if(Mathf.Abs(rigidBody.velocity.x) < maxSpeed){ 
				rigidBody.velocity = new Vector2(direction*maxSpeed*Time.deltaTime, 0);
			}
		}
	}


	/***** Dashing *****/

	void Dash(){
		if(Input.GetButton("Dash_P"+playerNum)){
			float dashDirection =  transform.localScale.x;
			rigidBody.AddForce(Vector2.right * dashDirection * dashForce * Time.deltaTime);
			gameObject.layer = LayerMask.NameToLayer(playerDashLayer);
			dashAnimator.CrossFade("Dash", 0f);
			PlaySound(dashSound);
			StartCoroutine("DashCooldown");
		}
	}
	IEnumerator DashCooldown(){
		ChangeState(State.DASHING);
		yield return new WaitForSeconds((float) playerAttrs[Attributes.DASHDELAY]);
		gameObject.layer = LayerMask.NameToLayer(playerLayer);
		ChangeState(_prevState);
	}


	/***** Throwing *****/

	void ThrowPresent(){
		if(Input.GetButton("Throw_P"+playerNum) && canThrow){
			CreateDropItem(true);
		}else if(Input.GetButton("Back_P"+playerNum) && canThrow){
			CreateDropItem(false);
		}
	}

	IEnumerator ThrowCooldown(){
		yield return new WaitForSeconds((float) playerAttrs[Attributes.THROWSPEED]);
		canThrow = true;
	}

	void CreateDropItem(bool isPresent){
		//Play throw sound
		PlaySound(throwSound);
		bodyAnimator.SetBool("isThrowing",true);

		//Create Present
		Vector3 initialPosition = transform.position + new Vector3(8*transform.localScale.x,-5,0);
		GameObject newPresent = Instantiate(present, initialPosition, Quaternion.identity) as GameObject;
		PresentController presentController = newPresent.GetComponent<PresentController>();
		if(isPresent){
			presentController.SetAsPresent();
		}else{
			presentController.SetAsCoal();
		}
		presentController.SetThrower(gameObject);
		if(currentCharacter != null) presentController.SetPresentSprite(currentCharacter.characterSpriteSheetName);
		presentController.SetSpeed((float)playerAttrs[Attributes.PRESENTSPEED]);
		canThrow = false;

		//Prevent player from being able to throw immidiately 
		StartCoroutine("ThrowCooldown");
	}


	/***** Stun *****/
	void StunPlayer(){
		disabled = true;
		PlaySound(stunnedSound);
		StartCoroutine("StunCooldown");
		bodyAnimator.SetBool("isStunned",true);
	}
	IEnumerator StunCooldown(){
		yield return new WaitForSeconds(2f);
		bodyAnimator.SetBool("isStunned",false);
		disabled = false;
	}


	/***** Dash Hit *****/
	void Hit(Collider2D collider){
		if(_state != State.STUNNED) ChangeState(State.HIT);
		var direction = collider.GetComponentInParent<PlayerController>().GetDirection();
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce(Vector2.right * Time.deltaTime * -direction * hitForce);
		PlaySound(swapSound);
		StartCoroutine("HitCooldown");
	}
	IEnumerator HitCooldown(){
		yield return new WaitForSeconds(0.3f);
		ChangeState(_prevState);
	}


	/***** Punching *****/

	/*void ThrowPunch(){ 
		if(Input.GetButtonDown("Back_P"+playerNum) && canThrow){ 
			canThrow = false; 
			bodyAnimator.SetBool("isHitting",true); 
			_state = State.PUNCHING; 
			StartCoroutine(PunchCooldown()); 
			StartCoroutine(ThrowCooldown()); 
		} 
	} 

	void Punch(){ 
		rigidBody.velocity = new Vector2((float) playerAttrs[Attributes.SPEED] * 3f * transform.localScale.x, 0); 
		bodyAnimator.SetBool("isHitting",false); 
	} 

	IEnumerator PunchCooldown(){ 
		yield return new WaitForSeconds(0.1f); 
		_state = State.DEAD; 
		yield return new WaitForSeconds(0.4f); 
		_state = State.ACTIVE; 
	} */


	/***** Powerup *****/

	IEnumerator PowerupTimeout(Attributes attribute, float multiplier, float timeout){
		var attributeType = playerAttrs[attribute].GetType();
		if(attributeType == typeof(float)){
			playerAttrs[attribute] = (float) playerAttrs[attribute] * multiplier;
			yield return new WaitForSeconds(timeout);
			playerAttrs[attribute] = (float) playerAttrs[attribute] /  multiplier;
		}else if(attributeType == typeof(State)){
			ChangeState((State)playerAttrs[attribute]);
			yield return new WaitForSeconds(timeout);
		//If state was changed by a powerup, return the state to normal
		if(_state == (State)playerAttrs[attribute]) ChangeState(_prevState);
		}
	}

	/***** Sound *****/

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}

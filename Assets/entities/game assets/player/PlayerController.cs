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
	public AudioClip multiplier2x;
	public AudioClip multiplier3x;
	public AudioClip multiplier4x;
	public float moveSpeed = 4f;
	public float dashForce = 500000f;
	public float dashDelay = 0.2f;
	public float hitForce = 200000f;
	public float throwSpeed = 1f;
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
		FROZEN,
		DASHDELAY
	}

	//Private vars
	Animator animator;
	Animator bodyAnimator;
	Animator dashAnimator;
	AudioSource audioSource;
	Rigidbody2D rigidBody;
	BoxCollider2D boxCollider;
	bool canThrow = true;
	int catches = 0;
	int maxScore;
	int playerScore;
	int scoreMultiplier = 1;
	State _state = State.ENTRY;
	GameObject playerScoreGroup;
	PlayerScoreController playerScoreBar = null;
	Text playerNameText;
	Transform playerTag;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();
		bodyAnimator = transform.Find ("Body").gameObject.GetComponent<Animator>();
		dashAnimator = transform.Find ("Body/Dash").gameObject.GetComponent<Animator>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		boxCollider = gameObject.GetComponent<BoxCollider2D>();
		playerTag = transform.Find("Body/tag").transform;
		bodyAnimator.logWarnings = false;
		//Get correct score component
		maxScore = GameController.GetMaxGameScore();
		playerScore = GameController.GetInitialGameScore();
		playerScoreGroup = GameObject.FindGameObjectsWithTag("Player Score").OrderBy(g=>g.transform.GetSiblingIndex()).ToArray()[playerNum-1];
		playerScoreBar = playerScoreGroup.GetComponent<PlayerScoreController>();
		playerScoreBar.SetInitialScore(playerScore, maxScore, scoreMultiplier);
		playerNameText = playerScoreGroup.transform.FindChild("Player Name").gameObject.GetComponent<Text>();
		//Populate Attrs Hashtable
		playerAttrs.Add (Attributes.SPEED, moveSpeed);
		playerAttrs.Add (Attributes.THROWSPEED, throwSpeed);
		playerAttrs.Add (Attributes.FROZEN, State.DEAD);
		playerAttrs.Add (Attributes.DASHDELAY, dashDelay);
	}
	
	// Update is called once per frame
	void Update () {
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
		case State.FALLING:
			break;
		case State.DEAD:
			break;
		}
	}

	void LateUpdate(){
		if(bodyAnimator.GetBool("isThrowing") == true){
			bodyAnimator.SetBool("isThrowing",false);
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.CompareTag("Hit Box")){
			Hit(collider);
		}
	}

	//Public Functions
	public void SetCharacter(CharacterCollection.Character character){
		name = playerNameText.text = character.displayName;
		transform.parent.gameObject.GetComponent<SpriteSwitch>().SetSpriteSheet(character.characterSpriteSheetName);
		Debug.Log(playerScoreGroup);
		playerScoreGroup.GetComponent<SpriteSwitch>().SetSpriteSheet(character.characterSpriteSheetName);
		currentCharacter = character;
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

	//Private Functions
	void Score(){
		if(playerScoreBar != null) playerScoreBar.SetScore(playerScore, scoreMultiplier);
	}


	/***** Moving *****/

	void MovePlayer(){
		if(Input.GetButton("Horizontal_P"+playerNum)){
			float maxSpeed = (float) playerAttrs[Attributes.SPEED];
			int direction = 1;
			if(Input.GetAxis("Horizontal_P"+playerNum) > 0){
				direction = 1;
			}else if(Input.GetAxis("Horizontal_P"+playerNum) < 0){
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
		if(Input.GetButton("X_P"+playerNum)){
			float dashDirection =  transform.localScale.x;
			rigidBody.AddForce(Vector2.right * dashDirection * dashForce * Time.deltaTime);
			gameObject.layer = LayerMask.NameToLayer(playerDashLayer);
			dashAnimator.CrossFade("Dash", 0f);
			PlaySound(dashSound);
			StartCoroutine("DashCooldown");
		}
	}
	IEnumerator DashCooldown(){
		State _prevState = _state;
		_state = State.DASHING;
		yield return new WaitForSeconds((float) playerAttrs[Attributes.DASHDELAY]);
		gameObject.layer = LayerMask.NameToLayer(playerLayer);
		if(_state == State.DASHING){
			_state = _prevState == State.MOVE_ONLY ? State.MOVE_ONLY : State.ACTIVE;
		}
	}


	/***** Throwing *****/

	void ThrowPresent(){
		if(Input.GetButtonDown("Throw_P"+playerNum) && canThrow){
			CreateDropItem(true);
		}else if(Input.GetButtonDown("Back_P"+playerNum) && canThrow){
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
		canThrow = false;

		//Prevent player from being able to throw immidiately 
		StartCoroutine("ThrowCooldown");
	}


	/***** Hit *****/
	void Hit(Collider2D collider){
		_state = State.HIT;
		var direction = collider.GetComponentInParent<PlayerController>().GetDirection();
		rigidBody.velocity = Vector2.zero;
		rigidBody.AddForce(Vector2.right * Time.deltaTime * -direction * hitForce);
		PlaySound(swapSound);
		StartCoroutine("HitCooldown");
	}
	IEnumerator HitCooldown(){
		yield return new WaitForSeconds(0.3f);
		_state = State.ACTIVE;
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
			_state = (State)playerAttrs[attribute];
			yield return new WaitForSeconds(timeout);
			if(_state == (State)playerAttrs[attribute]) _state = State.ACTIVE;
		}
	}


	/***** Sound *****/

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}

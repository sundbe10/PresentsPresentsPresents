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
		DISABLED
	}

	//Public Vars
	public float aggression = 1;
	public float walkSpeed = 15f;
	public float runSpeed = 25f;
	public string[] possibleSprites;
	public AudioClip runSound;

	//Private Vars
	AudioSource audioSource;
	State _state = State.ENTERING;
	Animator animator;

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

		//Start Entry
		StartCoroutine(EnterBully());
		StartCoroutine(AIControl());
	}

	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.ENTERING:
		case State.WALKING:
		case State.RUNNING:
			MoveBully();
			break;
		case State.IDLE:
			break;
		case State.FIGHTING:
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "Destroyer"){
			RemoveBully ();
		}
		if(collider.gameObject.tag == "Kid"){
			TryToHit();
		}
	}

	//Private Functions
	void MoveBully(){
		if(_state == State.ENTERING){
			transform.position = new Vector3(transform.position.x, transform.position.y + -8f*Time.deltaTime, transform.position.z);
		}else{
			transform.position = new Vector3(transform.position.x+walkSpeed*Time.deltaTime, transform.position.y, transform.position.z);
		}
	}

	void TryToHit(){
		if(_state != State.FIGHTING && Random.value > 0.5f){
			StopCoroutine("AIControl");
			_state = State.FIGHTING;
			animator.CrossFade("Push", 0f);
			StartCoroutine("AIControl");
		}
	}

	void SetRandomDirection(){
		float leftOrRight = Mathf.Round(Random.value);
		Debug.Log(-Screen.width*3/8);
		Vector3 worldPosition = Camera.main.WorldToScreenPoint(transform.position);
		//float percentAcrossScreen = worldPosition.x / Screen.width;

		if(transform.position.x < -Screen.width*3/8 || leftOrRight == 0){
			//Walk Right
			transform.localScale = new Vector3(1,1,1);
			walkSpeed = Mathf.Abs(walkSpeed);
		}
		else if(transform.position.x > Screen.width*3/8 || leftOrRight == 1){
			//Walk Left
			transform.localScale = new Vector3(-1,1,1);
			walkSpeed = -Mathf.Abs(walkSpeed);
		}
	}

	void RemoveBully(){
		Destroy(gameObject);
	}

	IEnumerator AIControl(){
		yield return new WaitForSeconds(2f);
		float stateNum = Random.value;
		float directionNum = Random.value;

		//Set Random State
		if(stateNum < 0.7f){
			_state = State.WALKING;
			animator.CrossFade("Walk", 0f);
		}else{
			_state = State.IDLE;
			animator.CrossFade("Idle", 0f);
		}

		//Set Random Direction
		if(directionNum > 0.5f){
			SetRandomDirection();
		}

		Debug.Log(_state);
		StartCoroutine("AIControl");
	}

	IEnumerator EnterBully(){
		yield return new WaitForSeconds(0.6f);
		SetRandomDirection();
		_state = State.WALKING;
		Debug.Log(_state);
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}
}

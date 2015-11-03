using UnityEngine;
using System.Collections;

public class KidController : MonoBehaviour {

	//State
	enum State{
		WALKING,
		RUNNING,
		DISABLED
	}

	//Public Vars
	public float walkSpeedMin = 0.5f;
	public float walkSpeedMax = 1f;
	public string[] possibleSprites;
	public GameObject[] powerups;
	public float powerupProbability = 0.1f;

	//Private Vars
	GameObject powerup;
	float walkSpeed;
	float runSpeed = 2.5f;
	AudioSource audioSource;
	State _state = State.WALKING;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();

		//Set random walk speed
		walkSpeed = Random.Range(walkSpeedMin, walkSpeedMax);

		//Set random walk left or right
		float leftOrRight = Random.value;
		Vector3 worldPosition = Camera.main.WorldToScreenPoint(transform.position);
		float percentAcrossScreen = worldPosition.x / Screen.width;
		
		if(transform.position.x <= -Screen.width/2){
			//Walk Right (default)
		}
		else if(transform.position.x > Screen.width/2 || leftOrRight < percentAcrossScreen){
			//Walk Left
			transform.localScale = new Vector3(-1,1,1);
			walkSpeed = -walkSpeed;
		}

		//Set Random sprite
		string randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
		SpriteSwitch switcher = gameObject.GetComponent<SpriteSwitch>();
		switcher.character = randomSprite;

		//Give Powerup
		if(Random.value < powerupProbability){
			GivePowerup();
		}
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
		//Powerup
		if(powerup){
			powerup.GetComponent<PowerupController>().ApplyPowerup(thrower);
		}
		HoldObject (thrownObject);

		audioSource.Play();
		if(walkSpeed > 0 ){
			walkSpeed = runSpeed;
		}else{
			walkSpeed = -runSpeed;
		}
		_state = State.RUNNING;
	}

	public void DisableKid(){
		_state = State.DISABLED;
	}


	//Private Functions
	void MoveKid(){
		transform.position = new Vector3(transform.position.x+walkSpeed, transform.position.y, transform.position.z);
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
}

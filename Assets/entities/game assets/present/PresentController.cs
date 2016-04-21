using UnityEngine;
using System.Collections;

public class PresentController : MonoBehaviour {

	//State
	enum State{
		FALLING,
		CAUGHT
	}

	//Private Vars
	GameObject thrower;
	State _state = State.FALLING;
	Rigidbody2D rigidBody;

	// Use this for initialization
	void Awake () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		StartCoroutine(PresentFall());
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.FALLING:
			break;
		case State.CAUGHT:
			break;
		}

	}

	void OnTriggerEnter2D(Collider2D collider){
		switch(_state){
		case State.FALLING:
			if(collider.gameObject.tag == "Kid" && !collider.GetComponent<KidController>().KidHasPresent()){
				PresentCaught(collider.gameObject);
			}else if(collider.gameObject.tag == "Destroyer"){
				thrower.GetComponent<PlayerController>().RemoveMultiplier();
				RemovePresent();
			}
			break;
		case State.CAUGHT:
			if(collider.gameObject.tag == "Destroyer"){
				RemovePresent();
			}
			break;
		}
	}


	//Public Functions
	public void SetThrower(GameObject throwerObject){
		thrower = throwerObject;
	}

	public void SetPresentSprite(Sprite presentSprite){
		SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
		spriteRender.sprite = presentSprite;
	}

	public void SetVelocity(Vector2 velocity){
		rigidBody.velocity = velocity;
	}

	//Private Functions
	void RemovePresent(){
		Destroy(gameObject);
	}
	
	void PresentCaught(GameObject catcher){
		rigidBody.isKinematic = true;
		catcher.GetComponent<KidController>().PresentCaught(gameObject, thrower);
		_state = State.CAUGHT;
	}

	IEnumerator PresentFall(){
		Debug.Log("waiting");
		yield return new WaitForSeconds(0.5f);
		Debug.Log("fall");
		rigidBody.gravityScale = 3;
		rigidBody.drag = 0.5f;
	}
}

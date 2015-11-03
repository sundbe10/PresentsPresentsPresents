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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.FALLING:
			transform.position += new Vector3(0,-2.5f,0);
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

	//Private Functions
	void RemovePresent(){
		Destroy(gameObject);
	}
	
	void PresentCaught(GameObject catcher){
		thrower.GetComponent<PlayerController>().IncrementScore(100);
		catcher.GetComponent<KidController>().PresentCaught(gameObject, thrower);
		_state = State.CAUGHT;
	}
}

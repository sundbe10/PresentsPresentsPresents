using UnityEngine;
using System.Collections;

public class PresentController : MonoBehaviour {

	//State
	enum State{
		FALLING,
		CAUGHT
	}

	//Public Vars
	public Sprite presentSprite;
	public Sprite coalSprite;
	public GameObject splash;

	//Private Vars
	bool isPresent;
	GameObject thrower;
	State _state = State.FALLING;
	SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		switch(_state){
		case State.FALLING:
			transform.position += new Vector3(0,-150f*Time.deltaTime,0);
			break;
		case State.CAUGHT:
			break;
		}

	}

	void OnTriggerEnter2D(Collider2D collider){
		switch(_state){
		case State.FALLING:
			if(collider.CompareTag("Destroyer")){
				thrower.GetComponent<PlayerController>().RemoveMultiplier();
				RemovePresent();
				MakeSplash();
			}
			break;
		case State.CAUGHT:
			if(collider.CompareTag("Destroyer")){
				RemovePresent();
			}
			break;
		}
	}


	//Public Functions
	public void SetThrower(GameObject throwerObject){
		thrower = throwerObject;
	}

	public GameObject GetThrower(){
		return thrower;
	}

	public void SetCaught(bool isKid){
		_state = State.CAUGHT;
		if(!isKid) RemovePresent();
	}

	public void SetAsPresent(){
		spriteRenderer.sprite = presentSprite;
		isPresent = true;
	}

	public void SetAsCoal(){
		spriteRenderer.sprite = coalSprite;
		isPresent = false;
	}

	public bool IsPresent(){
		return isPresent;
	}

	public bool IsCaught(){
		return _state == State.CAUGHT;
	}

	public void SetPresentSprite(string spriteSheetName){
		SpriteSwitch switcher = gameObject.GetComponent<SpriteSwitch>();
		switcher.SetSpriteSheet(spriteSheetName);
	}

	//Private Functions
	void RemovePresent(){
		Destroy(gameObject);
	}

	void MakeSplash(){
		Instantiate(splash, transform.position, Quaternion.identity);
	}

}

using UnityEngine;
using System.Collections;

public class SnowballController : MonoBehaviour {

	public float throwSpeed;
	public GameObject splash;

	Rigidbody2D rigidBody;

	// Use this for initialization
	void Awake () {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector2(rigidBody.velocity.x > 0 ? 1 : -1, 1);
	}

	void OnTriggerEnter2D(Collider2D collider ){
		if(collider.CompareTag("Destroyer")){
			Instantiate(splash, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}

	public void SetVelocity(Vector2 direction){
		Debug.Log(direction);
		rigidBody.velocity = throwSpeed * direction * Time.deltaTime;	
	}
}

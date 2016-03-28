using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour {

	public GameObject splash;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "Present"){
			Instantiate(splash, collider.transform.position, Quaternion.identity);
		}
	}
}

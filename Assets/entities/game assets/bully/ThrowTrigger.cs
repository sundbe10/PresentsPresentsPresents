using UnityEngine;
using System.Collections;

public class ThrowTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D collider){
		gameObject.GetComponentInParent<BullyController>().OnTriggerStayChild2D(collider);
	}
}

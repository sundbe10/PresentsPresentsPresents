using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	void Awake() {
		float screenScaler = Mathf.Round (Screen.height / 350f);
		if(Screen.height/(2*screenScaler) < 640){
			
		}
		//gameObject.GetComponent<Camera> ().orthographicSize = Screen.height / (2 * 2.5f); //4 works well for a height of 1080
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

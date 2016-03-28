using UnityEngine;
using System.Collections;

public class InitializerController : MonoBehaviour {

	public GameObject[] initialzeGameobjects;

	// Use this for initialization
	void Awake () {
		foreach(GameObject iObject in initialzeGameobjects){
			if(!GameObject.Find(iObject.name+"(Clone)") && !GameObject.Find(iObject.name)){
				Instantiate(iObject, transform.position,Quaternion.identity); 
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

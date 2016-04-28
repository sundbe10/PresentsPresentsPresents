using UnityEngine;
using System.Collections;

public class InitializerController : MonoBehaviour {

	public GameObject[] initializeGameObjects;

	// Use this for initialization
	void Awake () {
		foreach(GameObject iObject in initializeGameObjects){
			if(!GameObject.Find(iObject.name+"(Clone)") && !GameObject.Find(iObject.name)){
				Instantiate(iObject, transform.position,Quaternion.identity); 
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

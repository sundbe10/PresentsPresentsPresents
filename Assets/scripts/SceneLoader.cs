using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	void Start () {
		
	}

	public void LoadScene(string name){
		Debug.Log ("New Level load: " + name);
		Application.LoadLevel (name);
	}

	public void QuitRequest(){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}
		

}
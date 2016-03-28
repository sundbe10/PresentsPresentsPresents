using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour {


	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(SceneManager.GetActiveScene().name == "Game"){
			Destroy(gameObject);
		}
	}
}

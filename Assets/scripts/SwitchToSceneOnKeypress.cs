using UnityEngine;
using System.Collections;

public class SwitchToSceneOnKeypress : MonoBehaviour {

	public string input;
	public string scene;

	private SceneLoader _sceneLoader;

	// Use this for initialization
	void Start () {
		_sceneLoader = new SceneLoader();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown(input)){
			_sceneLoader.LoadScene(scene);
		}
	}
}

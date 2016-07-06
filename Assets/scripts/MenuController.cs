using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	void Start(){
		GameObject firstMenuItem = transform.Find("MenuItem").gameObject;
		gameObject.GetComponent<UnityEngine.EventSystems.EventSystem>().firstSelectedGameObject = firstMenuItem;
	}

	public void LoadSceneForward(string scene){
		SceneLoader.GoToScene(scene, true);
	}

	public void LoadSceneBackward(string scene){
		SceneLoader.GoToScene(scene, false);
	}

	public void RemoveMenu(){
		Destroy(gameObject);
	}

	public void UnpauseGame(){
		GameController.UnpauseGame();
	}

	public void QuitGame(){
		Application.Quit();
	}

}

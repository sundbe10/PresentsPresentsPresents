using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	public AudioClip backSound;
	public AudioClip forwardSound;
	public AudioClip windSound;

	AudioSource audioSource;
	Animator animator;
	bool allowButtonPresses = true;

	void Start () {
		DontDestroyOnLoad(gameObject);
		animator = gameObject.GetComponent<Animator>();
		audioSource = gameObject.GetComponent<AudioSource>();
	}

	void Update (){
		if(allowButtonPresses){
			switch(Application.loadedLevelName){
			case "Start":
				HandleStart();
				break;
			case "CharacterSelect":
				HandleCharacterSelection();
				break;
			case "Game":
				HandleGame();
				break;
			}
		}
	}
	public void FadeIn(){
		transform.Find("Fade").GetComponent<FadeOut>().StartFadeIn();
	}

	public void FadeOut(){
		transform.Find("Fade").GetComponent<FadeOut>().StartFadeOut();
	}

	public void AllowButtonPresses(){
		allowButtonPresses = true;
	}

	public void QuitRequest(){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}

	public void GoToScene(string scene, bool forward){
		StartCoroutine(LoadScene(scene));
		if(forward){
			SceneForward();
		}else{
			SceneBackward();
		}
	}

	void HandleStart(){
		if(Input.GetButtonDown("Start")){
			StartCoroutine(LoadScene("CharacterSelect"));
			SceneForward();
		}
	}

	void HandleCharacterSelection(){
		if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")){
			if(GameObject.Find("CharacterCollection").GetComponent<ConfirmCharacterSelect>().AllPlayersReady()){
				StartCoroutine(LoadScene("Game"));
				SceneForward();
			}
		}
		if(Input.GetButtonDown("Cancel")){
			if(GameObject.Find("CharacterCollection").GetComponent<ConfirmCharacterSelect>().NoActivePlayers()){
				StartCoroutine(LoadScene("Start"));
				SceneBackward();
			}
		}
	}

	void SceneForward(){
		allowButtonPresses = false;
		PlaySound(windSound);
		PlaySound(forwardSound);
		animator.CrossFade("SceneForward", 0f);
	}

	void SceneBackward(){
		allowButtonPresses = false;
		PlaySound(windSound);
		PlaySound(backSound);
		animator.CrossFade("SceneBackward", 0f);
	}

	IEnumerator LoadScene(string name){
		yield return new WaitForSeconds(1f);
		Debug.Log ("New Level load: " + name);
		Application.LoadLevel (name);
	}

	void HandleGame(){
	
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}
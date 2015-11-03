using UnityEngine;
using System.Collections;

public class GameAudioController : MonoBehaviour {

	//Public Vars
	public AudioClip entranceMusic;
	public AudioClip gameMusic;
	public AudioClip spawnSound;
	public AudioClip fallSound;
	public AudioClip timerWarningSound;
	public AudioClip winTextSound;

	//Private Vars
	AudioSource gameAudio;

	// Use this for initialization
	void Start () {
		gameAudio = gameObject.GetComponent<AudioSource>();
		PlaySound(entranceMusic);
		PlaySound(spawnSound);
		StartCoroutine(PlayGameAudio());
	}
	
	// Update is called once per frame
	void Update () {

	}


	//Public Functions
	public void PlayFallSound(){
		gameAudio.Stop();
		PlaySound(fallSound);
	}

	public void TimerWarning(){
		PlaySound(timerWarningSound);
	}

	public void WinText(){
		PlaySound(winTextSound);
	}

	//Private Functions
	IEnumerator PlayGameAudio(){
		yield return new WaitForSeconds(6);
		gameAudio.clip = gameMusic;
		gameAudio.loop = true;
		gameAudio.Play();

	}

	void PlaySound(AudioClip sound){
		gameAudio.PlayOneShot(sound,1f);
	}
}

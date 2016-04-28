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
	public AudioClip pauseSound;
	public AudioClip confirmSound;
	public AudioClip backSound;

	//Private Vars
	AudioSource gameAudio;
	AudioSource effectsAudio;
	bool fadeOutMusic = false;

	// Use this for initialization
	void Start () {
		gameAudio = gameObject.GetComponents<AudioSource>()[0];
		effectsAudio = gameObject.GetComponents<AudioSource>()[1];
		gameAudio.clip = entranceMusic;
		gameAudio.PlayDelayed(1.0f);
		PlayMusic(spawnSound);
		StartCoroutine(PlayGameAudio());
	}
	
	// Update is called once per frame
	void Update () {
		if(fadeOutMusic && gameAudio.volume > 0){
			gameAudio.volume =- 0.01f;
		}
	}


	//Public Functions
	public void PauseMusic(){
		gameAudio.Pause();
	}

	public void ResumeMusic(){
		gameAudio.UnPause();
	}

	public void FadeOutMusic(){
		fadeOutMusic = true;
	}

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

	public void PlayConfirmSound(){
		PlaySound(confirmSound);
	}

	public void PlayBackSound(){
		PlaySound(backSound);
	}

	public void PlayPauseSound(){
		PlaySound(pauseSound);
	}

	public void PlaySound(AudioClip sound){
		effectsAudio.PlayOneShot(sound,1f);
	}
	public void PlayMusic(AudioClip sound){
		gameAudio.PlayOneShot(sound,1f);
	}

	//Private Functions
	IEnumerator PlayGameAudio(){
		yield return new WaitForSeconds(7);
		gameAudio.clip = gameMusic;
		gameAudio.loop = true;
		gameAudio.Play();

	}


}

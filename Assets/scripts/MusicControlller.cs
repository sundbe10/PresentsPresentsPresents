using UnityEngine;
using System.Collections;

public class MusicControlller : MonoBehaviour {

	public AudioClip[] sceneMusic;

	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		PlaySceneMusic();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound,1f);
	}

	void PlaySceneMusic(){
		audioSource.clip = sceneMusic[0];
		audioSource.loop = true;
		audioSource.Play();
	}
}

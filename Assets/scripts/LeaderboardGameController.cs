using UnityEngine;
using System.Collections;

public class LeaderboardGameController : MonoBehaviour {

	enum State{
		ACTIVE,
		END
	}
		
	public GameObject menu;
	public AudioClip pauseSound;

	State _state = State.ACTIVE;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.ACTIVE){
			if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")){
				menu = Instantiate(menu, Vector3.zero, Quaternion.identity) as GameObject;
				menu.transform.SetParent(GameObject.Find("Canvas").transform);
				audioSource.PlayOneShot(pauseSound,1f);
				_state = State.END;
			}
		}
	}
}

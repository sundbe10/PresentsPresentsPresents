using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class CharacterUnlockController : MonoBehaviour {

	public AudioClip crashSound;
	public AudioClip whiteLightSound;
	public AudioClip unlockSound;
	public AudioClip discoverySound;
	public GameObject menu;
	public AudioClip pauseSound;

	enum State{
		IDLE,
		ACTIVE,
		END
	}

	State _state = State.IDLE;
	AudioSource audioSource;
	Image panel;
	Text characterText;
	byte fadeAlpha = 240;
	CharacterCollection.CharacterModel character;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		panel = transform.GetComponentInChildren<Image>();
		characterText = transform.Find("Character Text").GetComponent<Text>();
		character = GameStats.GetUnlockedCharacter();
		if(character != null){
			foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
				int counter = int.Parse(player.name.Replace("Character ",""));
				player.GetComponentInChildren<SpriteSwitch>().SetSpriteSheet(character.costumes[counter].characterSpriteSheetName);
			}
		}
		CharacterCollection.UnlockCharacter(character);
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == State.ACTIVE){
			if(Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm") || Input.GetButtonDown("Cancel")){
				menu = Instantiate(menu, Vector3.zero, Quaternion.identity) as GameObject;
				menu.transform.SetParent(GameObject.Find("Canvas").transform);
				audioSource.PlayOneShot(pauseSound,1f);
				_state = State.END;
			}
		}
	}

	public void EnableSceneNavigation(){
		_state = State.ACTIVE;
	}

	public void ShowCharacters(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			player.GetComponentInChildren<SpriteRenderer>().color = new Color32(255,255,255,255);
		}
	}

	public void ShowWhite(){
		panel.color = new Color32(255,255,255,255);
		PlaySound(whiteLightSound);
	}

	public void FadeWhite(){
		fadeAlpha -= 20;
		panel.color = new Color32(255,255,255,fadeAlpha);
		if(fadeAlpha != 0){
			Invoke("FadeWhite",0.2f);
		}else{
			panel.color = new Color32(255,255,255,0);
		}
	}
		
	public void PlayTaunt(){
		PlaySound(character.taunt);
	}

	public void PlayUnlcokMusic(){
		PlaySound(unlockSound);
	}

	public void PlayDiscoverySound(){
		PlaySound(discoverySound);
	}

	public void ShowCharacterText(){
		characterText.text = character.displayName.ToUpper();;
		PlaySound(crashSound);
		PlayTaunt();
	}



	void ShowUnlockedText(){
		characterText.text += "\nUNLOCKED";
		PlaySound(crashSound);
	}

	void PlaySound(AudioClip sound){
		audioSource.PlayOneShot(sound);
	}

}

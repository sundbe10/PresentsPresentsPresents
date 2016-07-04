using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSelectController : MonoBehaviour {

	private SpriteSwitch spriteSwitch;
	private Animator avatarAnimator;
	private bool isReady = false;


	// Use this for initialization
	void Awake () {
		spriteSwitch = gameObject.GetComponent<SpriteSwitch>();
		avatarAnimator = transform.Find("Canvas/Avatar Mask/Avatar").gameObject.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

	}
		
	public void SetCharacter(CharacterCollection.Character character){
		spriteSwitch.SetSpriteSheet(character.characterSpriteSheetName);
		//Hide if locked
		var color = character.locked ? new Color32(0,0,0,255) : new Color32(255,255,255,255);
		gameObject.GetComponentInChildren<SpriteRenderer>().color = color;
		transform.Find("Canvas/Avatar Mask/Avatar").GetComponent<Image>().color = color;
	}

	public void RemoveCharacter(){
		Destroy(gameObject);
	}

	public bool IsReady(){
		return isReady;
	}

	public void FinalizeSelection(){
		avatarAnimator.CrossFade("selected", 0f);
		isReady = true;
	}

	public void RemoveSelection(){
		avatarAnimator.CrossFade("idle", 0f);
		isReady = false;
	}
		
}

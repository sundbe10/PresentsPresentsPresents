using UnityEngine;
using System.Collections;

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

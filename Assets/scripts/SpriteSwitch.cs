using UnityEngine;
using System.Collections;
using System;

public class SpriteSwitch : MonoBehaviour {

	public string character;

	SpriteRenderer[] spriteRenderer;
	Sprite[] newSprites;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>();
		newSprites = Resources.LoadAll<Sprite>(character);
	}

	// Update is called once per frame
	void LateUpdate () {
		if(character != ""){
			foreach(SpriteRenderer renderer in spriteRenderer){
				if(renderer){
					//Debug.Log(renderer);
					string spriteName = renderer.sprite.name;
					Sprite newSprite = Array.Find(newSprites, item => item.name == spriteName);
					if(newSprite) renderer.sprite = newSprite;
				}
			}
		}
	}
}

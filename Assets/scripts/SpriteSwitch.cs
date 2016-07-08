using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SpriteSwitch : MonoBehaviour {

	public string character;

	SpriteRenderer[] spriteRenderer;
	Image[] uiImages;
	Sprite[] newSprites;

	// Use this for initialization
	void Awake () {
		spriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>();
		uiImages = gameObject.GetComponentsInChildren<Image>();
		newSprites = Resources.LoadAll<Sprite>(character);
	}

	// Update is called once per frame
	void LateUpdate () {
		if(character != ""){
			//For sprites
			foreach(SpriteRenderer renderer in spriteRenderer){
				if(renderer && renderer.sprite){
					string spriteName = renderer.sprite.name;
					Sprite newSprite = Array.Find(newSprites, item => item.name == spriteName);
					if(newSprite) renderer.sprite = newSprite;
				}
			}
			//For UI Images
			foreach(Image uiImage in uiImages){
				if(uiImage && uiImage.sprite){
					string spriteName = uiImage.sprite.name;
					Sprite newSprite = Array.Find(newSprites, item => item.name == spriteName);
					if(newSprite) uiImage.sprite = newSprite;
				}
			}
		}
	}

	public void SetSpriteSheet(string spriteSheet){
		character = spriteSheet;
		newSprites = Resources.LoadAll<Sprite>(character);
	}
}

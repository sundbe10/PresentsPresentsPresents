using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		Fade();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Fade(){
		if(spriteRenderer.color.a > 0){
			spriteRenderer.color -= new Color(0,0,0,0.1f);
		}
		Invoke("Fade", 5f*Time.deltaTime);
	}
}

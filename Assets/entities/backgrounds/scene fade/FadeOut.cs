using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = new Color(0,0,0,0);
	}

	// Update is called once per frame
	void Update () {

	}

	public void StartFadeOut(){
		CancelInvoke("StartFadeIn");
		if(spriteRenderer.color.a < 1.0f){
			spriteRenderer.color += new Color(0,0,0,0.1f);
			Invoke("StartFadeOut", 4f*Time.deltaTime);
		}else{
			CancelInvoke("StartFadeOut");
		}
	}

	public void StartFadeIn(){
		CancelInvoke("StartFadeOut");
		if(spriteRenderer.color.a > 0){
			spriteRenderer.color -= new Color(0,0,0,0.1f);
			Invoke("StartFadeIn", 4f*Time.deltaTime);
		}else{
			CancelInvoke("StartFadeIn");
		}
	}
}

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

	public void StartFade(){
		if(spriteRenderer.color.a < 1.0f){
			spriteRenderer.color += new Color(0,0,0,0.1f);
		}
		Invoke("StartFade", 5f*Time.deltaTime);
	}
}

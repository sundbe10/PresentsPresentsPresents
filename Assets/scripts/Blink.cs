using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Blink : MonoBehaviour {

	public float blinkSpeed = 30f;
	public bool blinkAtStart = true;

	CanvasRenderer canvasRenderer;
	bool canFade;
	float opacity = 1f;

	// Use this for initialization
	void Start () {
		canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
		canFade = blinkAtStart;
		Fade();
	}

	// Update is called once per frame
	void Update () {

	}

	public void StartBlink(){
		canFade = true;
	}
		

	public void StopBlink(){
		canFade = false;
		canvasRenderer.SetAlpha(1);
	}

	void Fade(){
		opacity = opacity == 1f ? 0 : 1f;
		if(canFade) canvasRenderer.SetAlpha(opacity);
		Invoke("Fade", blinkSpeed);
	}
		
}
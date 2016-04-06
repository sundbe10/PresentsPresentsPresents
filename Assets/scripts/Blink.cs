using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Blink : MonoBehaviour {

	public float blinkSpeed = 30f;

	private CanvasRenderer canvasRenderer;
	private int direction = 1;

	// Use this for initialization
	void Start () {
		canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
		Fade();
	}

	// Update is called once per frame
	void Update () {

	}

	void Fade(){
		/*if(text.color.a <= 0.1){
			direction = 1;
		}else if(text.color.a >= 1){
			direction = -1;
		}
		text.color += new Color(0,0,0,0.1f*direction);*/
		Debug.Log(canvasRenderer.GetAlpha());
		canvasRenderer.SetAlpha(canvasRenderer.GetAlpha() == 1f ? 0 : 1f);
		Invoke("Fade", blinkSpeed*Time.deltaTime);
	}
}
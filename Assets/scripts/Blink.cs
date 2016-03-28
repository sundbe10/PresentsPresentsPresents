using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Blink : MonoBehaviour {

	private Text text;
	private int direction = 1;

	// Use this for initialization
	void Start () {
		text = gameObject.GetComponent<Text>();
		Fade();
	}

	// Update is called once per frame
	void Update () {

	}

	void Fade(){
		if(text.color.a <= 0.1){
			direction = 1;
		}else if(text.color.a >= 1){
			direction = -1;
		}
		text.color += new Color(0,0,0,0.1f*direction);
		Invoke("Fade", 4f*Time.deltaTime);
	}
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoringTextController : MonoBehaviour {

	Text text;
	GameObject canvas;

	// Use this for initialization
	void Awake () {
		text = gameObject.GetComponent<Text>();
		Debug.Log (text);
		canvas = GameObject.Find("Game Canvas");
		text.color = new Color(255f, 255f, 255f, 0f);
		transform.parent = canvas.transform;
		StartCoroutine(DestroyScoreText());
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(0, 50 * Time.deltaTime, 0);
		if(text.color.a < 1f){
			text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a+0.1f);
		}
	}

	//Public Functions
	public void SetText(int scoreIncrement, int multiplier){
		int score = scoreIncrement*multiplier;
		text.text = score.ToString();
		switch(multiplier){
		case -1:
			text.color = new Color32(255,0,0,255);
			break;
		case 2:
			text.color = new Color32(255,150,0,255);
			break;
		case 3:
			text.color = new Color32(255,200,0,255);
			break;
		case 4:
			text.color = new Color32(255,255,0,255);
			break;
		default:
			text.color = new Color32(255,255,255,255);
			break;
		}
		if(multiplier > 0){
			text.fontSize = 90 + 100*score/1000;
		}else{
			text.fontSize = 100;
		}
	}

	//Private Functions
	IEnumerator DestroyScoreText(){
		yield return new WaitForSeconds(0.8f);
		Destroy (gameObject);
	}

}

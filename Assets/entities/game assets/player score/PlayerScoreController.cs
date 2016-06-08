using UnityEngine;
using System.Collections;

public class PlayerScoreController : MonoBehaviour {

	GameObject scoreBar;
	float maxWidth;

	// Use this for initialization
	void Start () {
		scoreBar = transform.FindChild("Score Bar").gameObject;
		maxWidth = scoreBar.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetScore(int score){
		
	}
}

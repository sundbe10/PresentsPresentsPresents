using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScoreController : MonoBehaviour {

	[System.Serializable]
	public class BarColor{
		public Color32 barColor;
		public Color32 stripeColor;
	}

	//Public vars
	public BarColor[] barColors;

	//Private vars
	RectTransform scoreBar;
	float maxWidth;
	int initialScore;
	int maxScore;
	Image scoreBarImage;
	Image scoreStripesImage;
	Animator animator;
	Text scoreText;

	// Use this for initialization
	void Awake () {
		animator = transform.Find("Score Mask/Score Bar/Score Stripes").GetComponent<Animator>();
		scoreBarImage = transform.Find("Score Mask/Score Bar").GetComponent<Image>();
		scoreStripesImage = transform.Find("Score Mask/Score Bar/Score Stripes").GetComponent<Image>();
		scoreBar = transform.Find("Score Mask/Score Bar").GetComponent<RectTransform>();
		scoreText = transform.Find("Score").GetComponent<Text>();
		maxWidth = scoreBar.rect.width;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Public
	public void SetScore(int score, int multiplier){
		initialScore = score;
		//Ignore -1 multiplier, treat as 1
		if(multiplier == -1) multiplier = 1;
		animator.speed = multiplier * 2;
		scoreBarImage.color = barColors[multiplier-1].barColor;
		scoreStripesImage.color = barColors[multiplier-1].stripeColor;
		scoreText.text = score.ToString();
	}
		
	public void SetInitialScore(int _initialScore, int _maxScore, int _multiplier){
		initialScore = _initialScore;
		maxScore = _maxScore;
		SetScore(initialScore, _multiplier);
	}

}

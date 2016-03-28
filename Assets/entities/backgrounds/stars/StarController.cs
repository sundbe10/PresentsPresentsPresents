using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarController : MonoBehaviour {
	
	public enum StarAnimation{
		Large,
		Medium,
		Small 
	}	
	public StarAnimation starAnimation = StarAnimation.Large;

	private Animator animator;
	private Dictionary<StarAnimation, string> animationDictionary = new Dictionary<StarAnimation, string>();

	// Use this for initialization
	void Start () {
		animationDictionary.Add(StarAnimation.Large, "flicker-large");
		animationDictionary.Add(StarAnimation.Medium, "flicker");
		animationDictionary.Add(StarAnimation.Small, "flicker-small");

		animator = GetComponent<Animator>();
		animator.Play(animationDictionary[starAnimation], -1, Random.Range(0.0f, 1.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

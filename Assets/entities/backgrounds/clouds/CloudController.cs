using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour {

	public float scrollSpeed = 10f;
	public float tileSize = 600f;
	public float initialOffset = 0f;

	private Vector3 starPosition;

	// Use this for initialization
	void Start () {
		starPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float newPosition = Mathf.Repeat(Time.time * scrollSpeed + initialOffset, tileSize);
		transform.position = starPosition + Vector3.left * newPosition;
	}
}

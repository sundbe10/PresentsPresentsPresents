using UnityEngine;
using System.Collections;

public class WallPositioner : MonoBehaviour {

	public enum Wall{
		TOP,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public Wall wallPosition;

	// Use this for initialization
	void Start () {
		Vector2 objectPosition = transform.position;
		Vector2 screenDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1f));
		switch(wallPosition){
		case Wall.TOP:
			break;
		case Wall.BOTTOM:
			break;
		case Wall.LEFT:
			objectPosition.x = -screenDimensions.x;
			break;
		case Wall.RIGHT:
			objectPosition.x = screenDimensions.x;
			break;
		}
		transform.position = objectPosition;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

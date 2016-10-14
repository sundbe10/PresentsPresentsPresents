using UnityEngine;
using System.Collections;

public class LaserPowerController : PowerController {

	Transform playerBody;
	Transform laserTop;
	Transform laserBottom;

	// Use this for initialization
	void Start () {
		base.Start();
		transform.parent = transform.parent.parent;
		playerBody = transform.parent.Find("Body");
		laserTop = transform.Find("laser top");
		laserBottom = transform.Find("laser bottom");
		//transform.position = new Vector3(0,100);
	}
	
	// Update is called once per frame
	void Update () {
		var playerDirection = transform.parent.localScale.x;
		laserTop.position = playerBody.position + new Vector3(7f*playerDirection, -30f);
		laserBottom.position = transform.parent.position + new Vector3(7f*playerDirection, -95);
	}
}

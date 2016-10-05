using UnityEngine;
using System.Collections;

public class FastThrowPowerup : PowerController {

	public GameObject smoke;

	SpriteRenderer playerRenderer;
	int bodyColorRed = 150;
	int sinCounter = 0;

	// Use this for initialization
	void Start () {
		ApplyPower();
		playerRenderer = gameObject.GetComponentInParent<SpriteRenderer>();
		InvokeRepeating("MakeSmoke", 0, 0.4f); // replace 0.5f with needed repeatRate
	}

	// Update is called once per frame
	void Update () {
		byte c = (byte)Mathf.RoundToInt(Mathf.Sin(sinCounter*Mathf.PI/180)*70+100);
		playerRenderer.color = new Color32(255,c,c,255);
		sinCounter += 5;
	}

	public override IEnumerator DestroyPower ()
	{
		yield return new WaitForSeconds(timeout);
		playerRenderer.color = new Color32(255,255,255,255);
		Debug.Log("Destroy");
		Destroy(gameObject);
	}

	void MakeSmoke(){
		GameObject newSmoke = Instantiate(smoke,transform.parent.position + new Vector3(Random.Range(-10f,10f),8+Random.Range(-7f,7f),0), Quaternion.identity) as GameObject;
		newSmoke.transform.SetParent(transform.parent);
	}
}

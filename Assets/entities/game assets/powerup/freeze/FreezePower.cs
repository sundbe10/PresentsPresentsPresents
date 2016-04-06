using UnityEngine;
using System.Collections;

public class FreezePower : PowerController {

	public GameObject iceBreak;

	// Use this for initialization
	void Start () {
		ApplyPower();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate(){
		//Freeze body position
		transform.parent.position = new Vector3(transform.parent.position.x,0,1);
	}

	public override IEnumerator DestroyPower(){
		yield return new WaitForSeconds(timeout);
		Debug.Log("Destroy");
		Instantiate(iceBreak, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class PowerController : MonoBehaviour {

	public PlayerController.Attributes attribute;
	public float multiplier;
	public float timeout;

	// Use this for initialization
	void Start () {
		ApplyPower();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void ApplyPower(){
		gameObject.GetComponentInParent<PlayerController>().ApplyPowerup(attribute, multiplier, timeout);
		StartCoroutine(DestroyPower());
	}

	public virtual IEnumerator DestroyPower(){
		yield return new WaitForSeconds(timeout);
		Debug.Log("Destroy");
		Destroy(gameObject);
	}
		
}

using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour {

	public string attribute;
	public float multiplier;
	public float timeout;
	public bool applyToPlayer;
	public AudioClip powerupSound;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Public Functions
	public void ApplyPowerup(GameObject player){
		AudioSource.PlayClipAtPoint(powerupSound,transform.position);
		player.GetComponent<PlayerController>().ApplyPowerup(attribute, multiplier, timeout);
		Destroy(gameObject);
	}
}

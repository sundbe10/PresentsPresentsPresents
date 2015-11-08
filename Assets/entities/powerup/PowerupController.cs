using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour {

	public string attribute;
	public float multiplier;
	public float timeout;
	public bool applyToPlayer;
	public bool applyToOtherPlayers;
	public AudioClip powerupSound;

	GameAudioController gameAudio;

	// Use this for initialization
	void Start () {
		gameAudio = GameObject.Find("GameController").GetComponent<GameAudioController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Public Functions
	public void ApplyPowerup(GameObject player){
		gameAudio.PlaySound(powerupSound);
		if(applyToPlayer == true){
			player.GetComponent<PlayerController>().ApplyPowerup(attribute, multiplier, timeout);
		}
		if(applyToOtherPlayers == true){
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject somePlayer in players){
				if(somePlayer != player){
					somePlayer.GetComponent<PlayerController>().ApplyPowerup(attribute, multiplier, timeout);
				}
			}
		}
		Destroy(gameObject);
	}
}

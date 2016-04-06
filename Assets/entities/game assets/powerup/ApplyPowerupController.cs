using UnityEngine;
using System.Collections;

public class ApplyPowerupController : MonoBehaviour {

	public GameObject power;
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

	public void ApplyPowerup(GameObject player){
		gameAudio.PlaySound(powerupSound);
		if(applyToPlayer == true){
			CreatePower(player);
		}
		if(applyToOtherPlayers == true){
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject somePlayer in players){
				if(somePlayer != player){
					CreatePower(somePlayer);
				}
			}
		}
		Destroy(gameObject);
	}

	private void CreatePower(GameObject player){
		Transform playerBody = player.transform.Find("Body").transform;
		GameObject _power = Instantiate(power, playerBody.position, Quaternion.identity) as GameObject;
		_power.transform.SetParent(playerBody);
	}
}

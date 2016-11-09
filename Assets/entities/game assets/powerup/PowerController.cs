using UnityEngine;
using System.Collections;

public class PowerController : MonoBehaviour {

	public PlayerController.Attributes attribute;
	public float multiplier;
	public float timeout;
	public GameObject powerSpriteObject;
	public float powerSpriteSpawnInterval = 0.5f;
	public bool powerHasPulseColor;
	public Color32 pulseColor;

	SpriteRenderer playerRenderer;
	int sinCounter = 0;

	// Use this for initialization
	protected void Start () {
		GameController.onGameEndEvent += EndPower;

		playerRenderer = gameObject.GetComponentInParent<SpriteRenderer>();
		ApplyPower();
		if(powerSpriteObject != null){
			InvokeRepeating("MakeSprites", 0, powerSpriteSpawnInterval);
		}
		if(powerHasPulseColor && playerRenderer.color == Color.white){
			InvokeRepeating("PulseColor", 0 , 0.1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void ApplyPower(){
		gameObject.GetComponentInParent<PlayerController>().ApplyPowerup(attribute, multiplier);
		StartCoroutine("DestroyPower");
	}

	protected void EndPower(){
		//Only used to end power when game ends
		StopCoroutine("DestroyPower");
		timeout = 0;
		StartCoroutine("DestroyPower");
	}

	public virtual IEnumerator DestroyPower(){
		yield return new WaitForSeconds(timeout);
		playerRenderer.color = new Color32(255,255,255,255);
		gameObject.GetComponentInParent<PlayerController>().RemovePowerup(attribute, multiplier);
		GameController.onGameEndEvent -= EndPower;
		Destroy(gameObject);
	}

	public void MakeSprites(){
		GameObject newSmoke = Instantiate(powerSpriteObject,transform.parent.position + new Vector3(Random.Range(-10f,10f),8+Random.Range(-7f,7f),0), Quaternion.identity) as GameObject;
		newSmoke.transform.SetParent(transform.parent);
	}

	public void PulseColor(){
		playerRenderer.color = Color.Lerp(Color.white, pulseColor, Mathf.PingPong(Time.time, 1));
	}
}

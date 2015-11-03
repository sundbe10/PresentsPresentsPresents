using UnityEngine;
using System.Collections;

public class KidSpawnController : MonoBehaviour {

	public GameObject kid;
	//public float spawnTimeMin;
	//public float spawnTimeMax;
	public float spawnProbability = 1f; 

	SpriteRenderer bodySpriteRenderer;

	// Use this for initialization
	void Start () {
		if(gameObject.GetComponentsInChildren<SpriteRenderer>().Length > 0){
			bodySpriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
		}
	}
	
	// Update is called once per frame
	void Update () {
		float probability = Time.deltaTime * spawnProbability;
		if(Random.value < probability){
			StartCoroutine(SpawnKid());
		}
	}

	IEnumerator SpawnKid(){
		Instantiate(kid, transform.position, Quaternion.identity);
		if(bodySpriteRenderer){
			bodySpriteRenderer.enabled = true;
			yield return new WaitForSeconds(1);
			bodySpriteRenderer.enabled = false;
		}
	}
}

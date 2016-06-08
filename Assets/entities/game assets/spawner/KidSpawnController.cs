using UnityEngine;
using System.Collections;

public class KidSpawnController : MonoBehaviour {

	[System.Serializable]
	public class SpawnObject{
		public GameObject prefab;
		public float spawnProbability;
		public int maxOnScreen;
	}

	public SpawnObject[] spawnObjects;
	public float spawnFrequency = 1f; 

	SpriteRenderer bodySpriteRenderer;

	// Use this for initialization
	void Start () {
		if(gameObject.GetComponentsInChildren<SpriteRenderer>().Length > 0){
			bodySpriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
		}
	}
	
	// Update is called once per frame
	void Update () {
		float probability = Time.deltaTime * spawnFrequency;
		if(Random.value < probability){
			GameObject newSpawn = GetWeightedObject();
			if(newSpawn != null){
				StartCoroutine(SpawnGameObject(newSpawn));
			}
		}
	}

	IEnumerator SpawnGameObject(GameObject spawnObject){
		Instantiate(spawnObject, transform.position, Quaternion.identity);
		if(bodySpriteRenderer){
			bodySpriteRenderer.enabled = true;
			yield return new WaitForSeconds(1);
			bodySpriteRenderer.enabled = false;
		}
	}

	GameObject GetWeightedObject(){
		//Total weighted items
		float totalProb = 0;
		for(int i=0; i < spawnObjects.Length; i++) totalProb += spawnObjects[i].spawnProbability;
		//Choose random number for weighted choice based on total available
		float randomProb = Random.Range(0, totalProb);
		float probCounter = 0;

		foreach(SpawnObject spawnObject in spawnObjects){
			int numOnScreen = GameObject.FindGameObjectsWithTag(spawnObject.prefab.tag).Length;
			probCounter += spawnObject.spawnProbability;
			if(randomProb < probCounter && numOnScreen < spawnObject.maxOnScreen){
				return spawnObject.prefab;
			}
		}
		return null;
	}
	
}

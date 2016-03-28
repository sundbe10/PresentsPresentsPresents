using UnityEngine;
using System.Collections;

public class IntervalObjectSpawner : MonoBehaviour {

	public GameObject[] objectsToSpawn;
	public float intervalMin = 5.0f;
	public float intervalMax = 6.0f;

	// Use this for initialization
	void Start () {
		SpawnObject();
	}

	// Update is called once per frame
	void Update () {
	}

	void SpawnObject(){
		GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
		Instantiate(objectToSpawn, transform.position, Quaternion.identity);
		Invoke("SpawnObject", Random.Range(intervalMin, intervalMax));
	}
}

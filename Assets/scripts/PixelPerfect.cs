using UnityEngine;
using System.Collections;

public class PixelPerfect : MonoBehaviour {

	// Set object transform to nearest whole pixel
	void LateUpdate(){
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		foreach(Transform transform in transforms){
			transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y), 0);
		}
	}
}

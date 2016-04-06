using UnityEngine;
using System.Collections;

public class AvatarController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Animator anim = GetComponent<Animator>();
		AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);//could replace 0 by any other animation layer index
		anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

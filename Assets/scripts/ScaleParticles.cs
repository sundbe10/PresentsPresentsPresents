using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles dynamically scaling of particles attached to objects.
// Goes through all the sub particle systems storing off initial values.
// Function to call when you update the scale.
public class ScaleParticles : MonoBehaviour {
	ParticleSystem ps;
	ParticleSystem.EmissionModule psemit;
	void Start () {
		ps = GetComponent<ParticleSystem>();
		psemit = ps.emission;
	}
	void Update () {
		if (!ps.isPlaying)
		{
			ps.Simulate(0.0f,true,true);
			psemit.enabled = true;
			ps.Play ();
		}
	}
}
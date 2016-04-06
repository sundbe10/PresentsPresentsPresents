using UnityEngine;
using System.Collections;

public class SuperSpeedPowerup : PowerController {

	// Use this for initialization
	void Start()
	{
		ApplyPower();
		InvokeRepeating("SpawnTrail", 0, 0.05f); // replace 0.2f with needed repeatRate
	}

	void SpawnTrail()
	{
		GameObject trailPart = new GameObject();
		SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
		var bodyRenderer = GetComponentInParent<SpriteRenderer>();
		var playerTransform = transform.parent.parent;
		trailPartRenderer.sprite = bodyRenderer.sprite;
		trailPartRenderer.sortingLayerName = bodyRenderer.sortingLayerName;
		trailPart.transform.position = transform.position;
		trailPart.transform.localScale = playerTransform.localScale;
		Destroy(trailPart, 0.2f); // replace 0.5f with needed lifeTime

		StartCoroutine("FadeTrailPart", trailPartRenderer);
	}

	IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
	{
		Color color = trailPartRenderer.color;
		color.a -= 0.3f; // replace 0.5f with needed alpha decrement
		trailPartRenderer.color = color;

		yield return new WaitForSeconds(0.05f);
		if(color.a > 0) StartCoroutine("FadeTrailPart", trailPartRenderer);

	}
}

using UnityEngine;
using System.Collections;

public class PeriodicShake : CameraShake {
	
	public float speed = 3.0f;
	
	// -------------------------------------------------------------------------
	void Update() {
		if (test) {
			test = false;
			PlayShake();
		}
	}
	
	// -------------------------------------------------------------------------
	public override IEnumerator Shake() {
		
		float elapsed = 0.0f;
		float randomStartX = Random.Range(-1000.0f, 1000.0f);
		float randomStartY = Random.Range(-1000.0f, 1000.0f);
		
		Vector3 originalCamPos = Camera.main.transform.position;
		
		while (elapsed < duration) {
			
			elapsed += Time.deltaTime;			
			
			float percentComplete = elapsed / duration;			
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
			
			// map noise to [-1, 1]
			float x = Mathf.Sin(randomStartX + percentComplete * speed);
			float y = Mathf.Cos(randomStartY + percentComplete * speed);
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);
				
			yield return null;
		}
		
		Camera.main.transform.position = originalCamPos;
	}
}

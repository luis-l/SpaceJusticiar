using UnityEngine;
using System.Collections;

public class RandomShake : CameraShake {

	// -------------------------------------------------------------------------
	void Update() {
		if (test) {
			test = false;
		}
	}

	// -------------------------------------------------------------------------
	public override IEnumerator Shake() {

        _bIsShaking = true;
		float elapsed = 0.0f;

		Vector3 originalCamPos = Camera.main.transform.position;
		
		while (elapsed < duration) {
			
			elapsed += Time.deltaTime;			
			
			float percentComplete = elapsed / duration;			
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
			
			// map noise to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

            if (targetTrans != null) {

                Vector3 newCamPos = new Vector3(x, y, originalCamPos.z);
                newCamPos.x += targetTrans.position.x;
                newCamPos.y += targetTrans.position.y;

                Camera.main.transform.position = newCamPos;
                originalCamPos = newCamPos;
            }

            //else {
              //  Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);
            //}

			yield return null;
		}

        if (elapsed > duration) {
            _bIsShaking = false;
        }

        if (targetTrans != null) {

            originalCamPos.x = targetTrans.position.x;
            originalCamPos.y = targetTrans.position.y;
            Camera.main.transform.position = originalCamPos;
        }

        else {
            Camera.main.transform.position = originalCamPos;
        }
	}
}

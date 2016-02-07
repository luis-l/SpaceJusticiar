using UnityEngine;
using System.Collections;

public class RandomShake : CameraShake {
	
	public float duration = 0.5f;
	public float magnitude = 0.1f;
	
	public bool test = false;

    /// <summary>
    /// The target to shake around from.
    /// </summary>
    public Transform targetTrans = null;

    private bool _bIsShaking = false;

	// -------------------------------------------------------------------------
	void Update() {
		if (test) {
			test = false;
		}
	}

    public bool IsShaking()
    {
        return _bIsShaking;
    }
	
	// -------------------------------------------------------------------------
	private IEnumerator Shake() {

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
                //newCamPos.x += originalCamPos.x;
                //newCamPos.y += originalCamPos.y;

                newCamPos.x += targetTrans.position.x;
                newCamPos.y += targetTrans.position.y;

                Camera.main.transform.position = newCamPos;
            }

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

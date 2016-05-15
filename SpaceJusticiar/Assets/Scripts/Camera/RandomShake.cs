using UnityEngine;
using System.Collections;

public class RandomShake : CameraShake
{

    // -------------------------------------------------------------------------
    void Update()
    {
        if (test) {
            test = false;
        }
    }

    // -------------------------------------------------------------------------
    public override IEnumerator Shake()
    {

        _bIsShaking = true;
        float elapsed = 0.0f;

        while (elapsed < duration) {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map noise to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;

            x *= magnitude * damper;
            y *= magnitude * damper;

            // Move the camera for the shake effect.
            targetCamera.transform.localPosition = new Vector2(x, y);

            yield return null;
        }

        if (elapsed > duration) {
            _bIsShaking = false;
        }

        // Center back the camera.
        targetCamera.transform.localPosition = Vector3.zero;
    }
}

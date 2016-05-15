using UnityEngine;
using System.Collections;

public class PeriodicShake : CameraShake
{

    // -------------------------------------------------------------------------
    void Update()
    {
        if (test) {
            test = false;
            PlayShake();
        }
    }

    // -------------------------------------------------------------------------
    public override IEnumerator Shake()
    {

        float elapsed = 0.0f;
        float randomStartX = Random.Range(-1000.0f, 1000.0f);
        float randomStartY = Random.Range(-1000.0f, 1000.0f);

        while (elapsed < duration) {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map noise to [-1, 1]
            float x = Mathf.Sin(randomStartX + percentComplete * speed);
            float y = Mathf.Cos(randomStartY + percentComplete * speed);
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

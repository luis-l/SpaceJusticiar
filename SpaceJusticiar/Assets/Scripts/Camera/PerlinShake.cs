using UnityEngine;
using System.Collections;

public class PerlinShake : CameraShake
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
        _bIsShaking = true;

        float randomStart = Random.Range(-1000.0f, 1000.0f);

        while (elapsed < duration) {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;

            // We want to reduce the shake from full power to 0 starting half way through
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

            // Calculate the noise parameter starting randomly and going as fast as speed allows
            float alpha = randomStart + speed * percentComplete;

            // map noise to [-1, 1]
            float x = Util.Noise.GetNoise(alpha, 0.0f, 0.0f) * 2.0f - 1.0f;
            float y = Util.Noise.GetNoise(0.0f, alpha, 0.0f) * 2.0f - 1.0f;

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

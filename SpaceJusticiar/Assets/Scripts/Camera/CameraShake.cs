
using UnityEngine;
using System.Collections;

public abstract class CameraShake : MonoBehaviour  {

    public void PlayShake()
    {
        StopAllCoroutines();
        StartCoroutine("Shake");
    }

    private IEnumerator Shake()
    {
        return null;
    }


    public bool IsShaking()
    {
        return false;
    }
}

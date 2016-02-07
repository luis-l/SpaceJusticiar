
using UnityEngine;
using System.Collections;

public abstract class CameraShake : MonoBehaviour  {

    public float duration = 0.5f;
    public float magnitude = 0.1f;

    protected bool _bIsShaking = false;

    public bool test = false;

    /// <summary>
    /// The target to shake around from.
    /// </summary>
    public Transform targetTrans = null;

    public void PlayShake()
    {
        StopAllCoroutines();
        StartCoroutine("Shake");
    }

    public abstract IEnumerator Shake();


    public bool IsShaking()
    {
        return _bIsShaking;
    }
}


/* *** IMPORTANT !!! ***
 * 
 * Since the game uses camera damping and camera shaking, in order
 * for the both effects to work well together, the game uses a dummy object that
 * that is the parent of the main camera. 
 * 
 * The smooth damping moves the dummy parent and the CameraShake moves the main camera
 * in local space.
 * */

using UnityEngine;
using System.Collections;

public abstract class CameraShake : MonoBehaviour  {

    public float duration = 0.5f;
    public float magnitude = 0.1f;
    public float speed = 3f;

    protected bool _bIsShaking = false;

    public bool test = false;

    // The camera to shake in local space. This camera should be a child of object being damped.
    // At least for this game.
    public Camera targetCamera;

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


/* *** IMPORTANT !!! ***
 * 
 * Since the game uses camera damping and camera shaking, in order
 * for the both effects to work well together, the game uses two cameras.
 * 
 * Unity's main camera is used for damping and a secondary one is used for shaking.
 * The secondary camera is a child of the main one, that way the shaking algorithm
 * simply offsets the camera in local space. The final view of the player will be through
 * the secondary camera.
 * */

using UnityEngine;
using System.Collections;

public abstract class CameraShake : MonoBehaviour  {

    public float duration = 0.5f;
    public float magnitude = 0.1f;
    public float speed = 3f;

    protected bool _bIsShaking = false;

    public bool test = false;

    // The camera to shake in local space. This camera should be a child of the main one.
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

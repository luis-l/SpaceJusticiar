using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private CameraFollow _camFollow;
    private CameraShake _camShake;

    public Transform transformToFollow = null;

    public CameraShake CameraShake { get { return _camShake; } }
    public CameraFollow CameraFollow { get { return _camFollow; } }

	// Use this for initialization
	void Start () {
        _camFollow = gameObject.AddComponent<CameraFollow>();
        _camShake = gameObject.AddComponent<PerlinShake>();

        if (transformToFollow == null) {
            transformToFollow = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _camShake.targetTrans = transformToFollow;
        _camFollow.targetTrans = transformToFollow;


	}
	
	// Update is called once per frame
	void Update () {

        bool camIsShaking = _camShake.IsShaking();

        if (!_camFollow.bFollow && !camIsShaking) {
            _camFollow.bFollow = true;
        }

        else if (camIsShaking) {
            _camFollow.bFollow = false;
        }

	}

    public void PlayShake()
    {
        _camShake.PlayShake();
    }

    /// <summary>
    /// Fill the camera background with a color for a certain duration in seconds.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public void FillScreen(Color color, float duration)
    {
        StopCoroutine(Fill(color, duration));
        StartCoroutine(Fill(color, duration));
    } 

    private IEnumerator Fill(Color color, float duration)
    {
        float elapsed = 0f;

        Camera.main.backgroundColor = color;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.backgroundColor = Color.black;
    }
}

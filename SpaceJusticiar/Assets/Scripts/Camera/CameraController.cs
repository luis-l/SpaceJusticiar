using UnityEngine;
using System.Collections;

// Class to access camera behaviors.
public class CameraController : MonoBehaviour {

    [SerializeField]
    private CameraFollow _camFollow;

    [SerializeField]
    private CameraShake _camShake;

    public CameraShake CameraShake { get { return _camShake; } }
    public CameraFollow CameraFollow { get { return _camFollow; } }

    public Transform transformToFollow = null;
    private Color _originalColor;

	// Use this for initialization
	void Awake () {

        if (transformToFollow == null) {
            transformToFollow = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Set camera follow parameters.
        _camFollow.followingTransform = transformToFollow;
        _camFollow.aheadFactor = 6f;
        _camFollow.damping = 0.7f;

        _originalColor = Camera.main.backgroundColor;
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
        StopCoroutine("Fill");
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

        Camera.main.backgroundColor = _originalColor;
    }

    public float CameraSize
    {
        get { return Camera.main.orthographicSize; }
        set
        {
            Camera.main.orthographicSize = value;
            _camShake.targetCamera.orthographicSize = value;
        }
    }
}

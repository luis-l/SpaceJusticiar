using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

    private float _camZoomDelta = 1f;
    private float _minCamSize = 8f;
    private float _maxCamSize = 20f;

    private StarSystem _starSys;

    void Awake()
    {
        ResourceManager.Init();

        _starSys = new StarSystem();
        _starSys.Init();
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Change camera zoom.
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftControl) && wheelDelta > 0) {
            if (Camera.main.orthographicSize > _minCamSize) {
                Camera.main.orthographicSize -= _camZoomDelta;
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl) && wheelDelta < 0) {
            if (Camera.main.orthographicSize < _maxCamSize) {
                Camera.main.orthographicSize += _camZoomDelta;
            }
        }
	}

    public StarSystem StarSystem { get { return _starSys; } }
}

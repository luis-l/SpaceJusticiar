using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

    private float _camZoomDelta = 2f;
    public const float MIN_CAM_SIZE = 2f;
    public const float MAX_CAM_SIZE = 50f;

    private StarSystem _starSys;
    private SpaceEngine _spaceEngine;

    void Awake()
    {
        _spaceEngine = GetComponent<SpaceEngine>();

        ResourceManager.Init();

        _starSys = new StarSystem();
        _starSys.Init();

        _spaceEngine.currentStarSys = _starSys;
    }

	// Use this for initialization
	void Start () {

	}

    private SurfaceDetail _currentDetail = SurfaceDetail.NONE;

	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Change camera zoom.
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftControl) && wheelDelta > 0) {
            if (Camera.main.orthographicSize > MIN_CAM_SIZE) {
                Camera.main.orthographicSize -= _camZoomDelta;
            }
            else {
                Camera.main.orthographicSize = MIN_CAM_SIZE;
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl) && wheelDelta < 0) {
            if (Camera.main.orthographicSize < MAX_CAM_SIZE) {
                Camera.main.orthographicSize += _camZoomDelta;
            }
            else {
                Camera.main.orthographicSize = MAX_CAM_SIZE;
            }
        }
        
        SurfaceDetail nextDetail;
        if (Camera.main.orthographicSize < 5) {
            nextDetail = SurfaceDetail.ULTRA;
        }
        else if (Camera.main.orthographicSize < 10) {
            nextDetail = SurfaceDetail.HIGH;
        }
        else if (Camera.main.orthographicSize < 18) {
            nextDetail = SurfaceDetail.MED;
        }
        else if (Camera.main.orthographicSize < 30) {
            nextDetail = SurfaceDetail.LOW;
        }
        else {
            nextDetail = SurfaceDetail.VERY_LOW;
        }

        if (nextDetail != _currentDetail) {
            _currentDetail = nextDetail;
            CelestialBody first = _starSys.GetPlanet(0);
            StarSystem.GenerateSurface(_currentDetail, first);
        } 
    }

    public StarSystem StarSystem { get { return _starSys; } }
}

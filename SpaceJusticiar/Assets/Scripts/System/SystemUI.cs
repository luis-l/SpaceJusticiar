using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SystemUI : SystemBase {

    private float _camZoomDelta = 2f;
    public const float MIN_CAM_SIZE = 2f;
    public const float MAX_CAM_SIZE = 50f;

    private CameraController _cameraController;

    private SurfaceDetail _currentDetail;

    public SystemUI()
    {
        _cameraController = Camera.main.gameObject.GetComponent<CameraController>();
    }

	// Update is called once per frame
	public override void Update () {

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Change camera zoom.
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftControl) && wheelDelta > 0) {
            if (_cameraController.CameraSize > MIN_CAM_SIZE) {
                _cameraController.CameraSize -= _camZoomDelta;
            }
            else {
                _cameraController.CameraSize = MIN_CAM_SIZE;
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl) && wheelDelta < 0) {
            if (_cameraController.CameraSize < MAX_CAM_SIZE) {
                _cameraController.CameraSize += _camZoomDelta;
            }
            else {
                _cameraController.CameraSize = MAX_CAM_SIZE;
            }
        }

        SurfaceDetail nextDetail;
        if (_cameraController.CameraSize < 8) {
            nextDetail = SurfaceDetail.ULTRA;
        }
        else if (_cameraController.CameraSize < 15) {
            nextDetail = SurfaceDetail.HIGH;
        }
        else if (_cameraController.CameraSize < 25) {
            nextDetail = SurfaceDetail.MED;
        }
        else if (_cameraController.CameraSize < 35) {
            nextDetail = SurfaceDetail.LOW;
        }
        else {
            nextDetail = SurfaceDetail.VERY_LOW;
        }

        if (nextDetail != _currentDetail) {
            _currentDetail = nextDetail;
            CelestialBody first = Systems.Instance.SpaceEngine.ActiveStarSystem.GetPlanet(0);
            StarSystem.GenerateSurface(_currentDetail, first);
        } 
	}

    public CameraController CameraController { get { return _cameraController; } }
}

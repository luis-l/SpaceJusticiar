﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

    private float _camZoomDelta = 2f;
    private float _minCamSize = 2f;
    private float _maxCamSize = 50f;

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
            if (Camera.main.orthographicSize > _minCamSize) {
                Camera.main.orthographicSize -= _camZoomDelta;
            }
            else {
                Camera.main.orthographicSize = _minCamSize;
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl) && wheelDelta < 0) {
            if (Camera.main.orthographicSize < _maxCamSize) {
                Camera.main.orthographicSize += _camZoomDelta;
            }
            else {
                Camera.main.orthographicSize = _maxCamSize;
            }
        }

        SurfaceDetail nextDetail;
        if (Camera.main.orthographicSize < 3) {
            nextDetail = SurfaceDetail.ULTRA;
        }
        else if (Camera.main.orthographicSize < 8) {
            nextDetail = SurfaceDetail.HIGH;
        }
        else if (Camera.main.orthographicSize < 15) {
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

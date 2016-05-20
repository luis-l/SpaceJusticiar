using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SystemUI : SystemBase
{

    private float _camZoomDelta = 2f;
    public const float MIN_CAM_SIZE = 2f;
    public const float MAX_CAM_SIZE = 50f;

    private CameraController _cameraController;

    private SurfaceDetail _currentDetail;

    public Text healthText = null;
    public Text energyText = null;
    public Text firingRateText = null;

    // The OC to focus on
    private ObjectController _focusOC;
    private LaserCannon _focusGun;

    public Transform selectedTarget;

    private Transform _reticleTrans;

    private ObjectController FocusOC
    {
        get { return _focusOC; }
        set
        {
            _focusOC = value;
            _focusGun = _focusOC.gameObject.GetComponentInChildren<LaserCannon>();
        }
    }

    public SystemUI()
    {
        Cursor.visible = false;

        _cameraController = Camera.main.gameObject.GetComponent<CameraController>();

        healthText = GameObject.Find("Canvas/Health/HealthValueText").GetComponent<Text>();
        energyText = GameObject.Find("Canvas/Energy/EnergyValueText").GetComponent<Text>();
        firingRateText = GameObject.Find("Canvas/FiringRate/FiringRateValue").GetComponent<Text>();

        FocusOC = GameObject.Find("Player").GetComponent<ObjectController>();

        _focusGun.gameObject.GetComponent<PlayerShooting>().OnChangeFiringRateEvent += UpdateFiringRateText;

        _reticleTrans = GameObject.Find("Canvas/Reticle").transform;
    }

    // Update is called once per frame
    public override void Update()
    {
        _reticleTrans.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Energy regen.
        if (_focusOC.EnergyCell != null && _focusOC.EnergyCell.Charge < EnergyCell.MAX_ENERGY) {
            energyText.text = _focusOC.EnergyCell.GetPercentage().ToString();
        }

        // Health regen.
        if (_focusOC.Health.GetHealth() < HealthComponent.MAX_HEALTH) {
            healthText.text = _focusOC.Health.GetPercentage().ToString();
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

    private void UpdateFiringRateText()
    {
        firingRateText.text = _focusGun.FiringDelay.ToString("0.##");
    }

    public void DisplayDamageFeedback(ObjectController damagedObject, float damage)
    {
        GameObject textDamage = UIPools.Instance.Fetch("DamageText");
        TextBehavior textBehavior = textDamage.GetComponent<TextBehavior>();
        textBehavior.life = 1.5f;
        textBehavior.lerpSpeed = Random.Range(0.15f, 0.5f);
        textBehavior.Text.text = (damage * 100).ToString("0.##");

        Vector2 screenCoord = Camera.main.WorldToScreenPoint(damagedObject.transform.position);
        textBehavior.RectTransform.position = screenCoord;
        textBehavior.endPosition = screenCoord + Random.insideUnitCircle * 300;
    }

    public CameraController CameraController { get { return _cameraController; } }
}

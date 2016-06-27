using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

using Vectrosity;

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
    public Transform ReticleTransform { get { return _reticleTrans; } }

    // A line used to associate which gameobject is being targeted.
    private VectorLine _targetingLine;

    private Dictionary<ObjectController, HealthBarData> _displayedHealthBars = new Dictionary<ObjectController, HealthBarData>();

    private ObjectController FocusOC
    {
        get { return _focusOC; }
        set
        {
            _focusOC = value;
            _focusGun = _focusOC.gameObject.GetComponentInChildren<LaserCannon>();
        }
    }

    private TargetingSystem _focusTargetSys;

    public SystemUI()
    {

        //Cursor.visible = false;

        _cameraController = Camera.main.gameObject.GetComponent<CameraController>();

        healthText = GameObject.Find("Canvas/Health/HealthValueText").GetComponent<Text>();
        energyText = GameObject.Find("Canvas/Energy/EnergyValueText").GetComponent<Text>();
        firingRateText = GameObject.Find("Canvas/FiringRate/FiringRateValue").GetComponent<Text>();

        _reticleTrans = GameObject.Find("Canvas/Reticle").transform;

        _targetingLine = new VectorLine("TargetingLine", new Vector2[2], null, 0.7f);
        _targetingLine.SetColor(Color.cyan);
    }

    public void SetFocusObject(ObjectController oc)
    {
        FocusOC = oc;

        FocusOC.OnDeathEvent += OnFocusDeath;

        _focusGun.GetComponent<PlayerShooting>().OnChangeFiringRateEvent += UpdateFiringRateText;

        _focusTargetSys = _focusGun.GetComponent<TargetingSystem>();
    }

    private void OnFocusDeath()
    {
        _reticleTrans.gameObject.SetActive(false);
        _targetingLine.active = false;
    }

    public override void Update()
    {
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

        RenderTargetingLine();
        HandleReticle();
        UpdateHealthBars();
    }

    private void RenderTargetingLine()
    {
        if (selectedTarget != null) {

            _targetingLine.active = true;

            _targetingLine.points2[0] = Camera.main.WorldToScreenPoint(selectedTarget.transform.position);
            _targetingLine.points2[1] = _reticleTrans.position;

            _targetingLine.Draw();
        }

        else if (_targetingLine.active) {
            _targetingLine.active = false;
        }
    }

    private void HandleReticle()
    {
        if (_focusTargetSys == null) return;

        _focusTargetSys.targetTrans = selectedTarget;

        if (selectedTarget == null) {
            Systems.Instance.SystemUI.ReticleTransform.position = Input.mousePosition;
        }

        else {
            Systems.Instance.SystemUI.ReticleTransform.position = Camera.main.WorldToScreenPoint(_focusTargetSys.LeadingPosition);
        }
    }

    private void UpdateFiringRateText()
    {
        firingRateText.text = _focusGun.FiringDelay.ToString("0.##");
    }

    public void DisplayDamageFeedback(ObjectController damagedObject, float damage)
    {
        GameObject textDamage = UIPools.Instance.Fetch("DamageText");

        UIFader textFader = textDamage.GetComponent<UIFader>();
        textFader.life = 1.7f;
        textFader.lerpSpeed = Random.Range(0.15f, 0.5f);
        textFader.alphaDecayRate = 1.2f;

        Text text = (Text)textFader.Graphics[0];
        text.text = (damage * 100).ToString("0");

        Vector2 screenCoord = Camera.main.WorldToScreenPoint(damagedObject.transform.position);
        textFader.RectTransform.position = screenCoord;
        textFader.endPosition = screenCoord + Random.insideUnitCircle * 300;

        BindHealthBar(damagedObject);
    }

    private void BindHealthBar(ObjectController damagedObject)
    {
        if (_displayedHealthBars.ContainsKey(damagedObject)) {
            HealthBarData data = _displayedHealthBars[damagedObject];
            data.Fader.SetAlpha(1f);
            data.Fader.ResetLife();
            return;
        }

        GameObject healthBar = UIPools.Instance.Fetch("HealthBar");

        UIFader healthFader = healthBar.GetComponent<UIFader>();
        healthFader.associatedOC = damagedObject;
        healthFader.life = 8f;
        healthFader.lerpSpeed = 0;
        healthFader.alphaDecayRate = 0.2f;

        Slider slider = healthFader.GetComponent<Slider>();
        slider.value = damagedObject.Health.GetHealth();

        Vector2 screenCoord = Camera.main.WorldToScreenPoint(damagedObject.transform.position);
        float width = healthFader.RectTransform.rect.width;
        float height = healthFader.RectTransform.rect.height;
        
        healthFader.RectTransform.position = screenCoord + new Vector2(width / 2f, -height);

        _displayedHealthBars.Add(damagedObject, new HealthBarData(damagedObject, slider, healthFader));
    }

    public void UpdateHealthBars()
    {
        if (_displayedHealthBars.Count == 0) return;

        foreach (KeyValuePair<ObjectController, HealthBarData> pair in _displayedHealthBars) {

            ObjectController oc = pair.Key;
            Slider healthSlider = pair.Value.Slider;
            UIFader fader = pair.Value.Fader;

            healthSlider.value = oc.Health.GetHealth();

            Vector2 screenCoord = Camera.main.WorldToScreenPoint(oc.transform.position);

            float width = fader.RectTransform.rect.width;
            float height = fader.RectTransform.rect.height;

            fader.RectTransform.position = screenCoord + new Vector2(width / 2f, -height);
        }
    }

    public void OnOcDeath(ObjectController oc)
    {
        if (_displayedHealthBars.ContainsKey(oc)) {
            HealthBarData data = _displayedHealthBars[oc];
            data.Fader.CleanUp();
            _displayedHealthBars.Remove(oc);
        }
    }

    public void OnUI_FaderEnd(UIFader fader)
    {
        ObjectController oc = fader.associatedOC;
        if (oc != null) {
            _displayedHealthBars.Remove(oc);
        }
    }

    public CameraController CameraController { get { return _cameraController; } }

    private class HealthBarData
    {
        public HealthBarData(ObjectController oc, Slider slider, UIFader fader)
        {
            _oc = oc;
            _slider = slider;
            _fader = fader;
        }

        private ObjectController _oc;
        private Slider _slider;
        private UIFader _fader;

        public ObjectController OC { get { return _oc; } }
        public Slider Slider { get { return _slider; } }
        public UIFader Fader { get { return _fader; } }
    }
}

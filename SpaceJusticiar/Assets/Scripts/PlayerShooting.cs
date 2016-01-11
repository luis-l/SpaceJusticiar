using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{

    private Vector3 prevPos = new Vector3();
    private Vector3 prevMousePos = new Vector3();

    private GameObject _projectile;

    private float _firingTimer = 0f;
    private float _firingRate = 0.15f;
    private float _firingSpeed = 100f;

    // The bounds of the firing rate.
    private float _lowestFiringDelay = 0.015f;
    private float _highestFiringDelay = 0.3f;
    private float _firingDelayChange = 0.05f;

    private float _energyConsumption = 0.1f;

    private AudioSource _laserShotSfx;

    public PlayerController playerController = null;

    public Text firingRateText = null;

    // Use this for initialization
    void Start()
    {
        prevPos = transform.position;
        prevMousePos = Input.mousePosition;

        _projectile = Resources.Load("Prefabs/EnergyProjectile") as GameObject;
        _laserShotSfx = GetComponent<AudioSource>();

        firingRateText.text = _firingRate.ToString();
    }

    void Update()
    {
        if (playerController.EnergyCell.HasEnergy() && _firingTimer >= _firingRate && Input.GetMouseButton(0)) {
            playerController.EnergyCell.UseEnergy(_energyConsumption);

            GameObject proj = Pools.Instance.Fetch(_projectile.name);

            Vector2 toMouse = new Vector2();
            if (Camera.main.orthographic) {
                toMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            }
            else {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    
                }
            }
            toMouse.Normalize();

            proj.transform.position = transform.position;
            proj.transform.rotation = transform.rotation;
            proj.GetComponent<Rigidbody2D>().velocity = toMouse * _firingSpeed;
            proj.GetComponent<ProjectileBehavior>().targetTag = "Enemy";

            _firingTimer = 0f;

            _laserShotSfx.Play();

        }
        if (_firingTimer < _firingRate)
            _firingTimer += Time.deltaTime;


        // Manage firing rate
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (wheelDelta > 0) {
            _firingRate += _firingDelayChange;
        }
        else if (wheelDelta < 0) {
            _firingRate -= _firingDelayChange;
        }

        // Cap firing rate.
        if (_firingRate > _highestFiringDelay)
            _firingRate = _highestFiringDelay;
        else if (_firingRate < _lowestFiringDelay)
            _firingRate = _lowestFiringDelay;

        // Update firing rate text.
        if (wheelDelta != 0) {
            firingRateText.text = _firingRate.ToString("#.##");
        }
    }

    void LateUpdate()
    {
        if (prevPos != transform.position || prevMousePos != Input.mousePosition) {

            Vector3 toMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            toMouse.Normalize();

            float rotationZ = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            prevPos = transform.position;
            prevPos = Input.mousePosition;
        }
    }
}
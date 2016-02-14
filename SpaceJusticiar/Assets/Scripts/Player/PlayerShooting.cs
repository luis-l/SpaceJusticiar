using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    private Vector3 prevPos = new Vector3();
    private Vector3 prevMousePos = new Vector3();

    // Manual changing of firing delay by the player.
    private float _firingDelayChange = 0.05f;

    public PlayerController playerController = null;

    public Text firingRateText = null;

    [SerializeField]
    private LaserCannon _mainGun = null;

    public GameObject[] projectileTypes;

    // Use this for initialization
    void Start()
    {
        _mainGun.ProjectileType = projectileTypes[0];

        _mainGun.firingForce = 5000f;
        _mainGun.FiringDelay = 0.15f;

        prevPos = transform.position;
        prevMousePos = Input.mousePosition;
        firingRateText.text = _mainGun.FiringDelay.ToString("#.##");
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {

            Vector2 initialProjVelocity = new Vector2(0, 0);

            if (Input.GetMouseButton(0) && _mainGun.ProjectileType.name != projectileTypes[0].name) {
                _mainGun.ProjectileType = projectileTypes[0];
                _mainGun.firingForce = 6000f;
            }

            else if (Input.GetMouseButton(1) && _mainGun.ProjectileType.name != projectileTypes[1].name) {
                _mainGun.ProjectileType = projectileTypes[1];
                _mainGun.firingForce = 850f;
                
            }

            // Give bullet initial velocity of the player to start with.
            if (_mainGun.ProjectileType.name == projectileTypes[1].name) {
                initialProjVelocity = playerController.rigidBody.velocity * 0.75f;
            }


            Vector2 mousePos = new Vector2();
            if (Camera.main.orthographic) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else {
                // Still thinking if perspective should be used.
            }

            _mainGun.Fire(mousePos, "Enemy", playerController.EnergyCell, initialProjVelocity);
        }

        // Manage chaning firing rate
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftShift) && wheelDelta > 0) {
            _mainGun.FiringDelay += _firingDelayChange;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && wheelDelta < 0) {
            _mainGun.FiringDelay -= _firingDelayChange;
        }

        // Update firing rate text.
        if (wheelDelta != 0) {
            firingRateText.text = _mainGun.FiringDelay.ToString("#.##");
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
using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    private Vector3 prevPos = new Vector3();
    private Vector3 prevMousePos = new Vector3();

    // Manual changing of firing delay by the player.
    private float _firingDelayChange = 0.05f;

    public PlayerController playerController = null;

    [SerializeField]
    private LaserCannon _mainGun = null;

    public GameObject[] projectileTypes;

    private TargetingSystem _targetSys;

    public delegate void OnChangeFiringRateDelgate();
    public event OnChangeFiringRateDelgate OnChangeFiringRateEvent = delegate { };

    // Use this for initialization
    void Start()
    {
        _mainGun.ProjectileType = projectileTypes[0];

        _mainGun.firingForce = 1050f;
        _mainGun.FiringDelay = 0.15f;

        prevPos = transform.position;
        prevMousePos = Input.mousePosition;

        _targetSys = gameObject.AddComponent<TargetingSystem>();
        _targetSys.mainGun = _mainGun;
        _targetSys.bAutoFire = false;
        _targetSys.Range = 100;
        _targetSys.EnergyCell = gameObject.transform.parent.gameObject.GetComponent<ObjectController>().EnergyCell;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {

            Vector2 initialProjVelocity = playerController.rigidBody.velocity;

            if (Input.GetMouseButton(0) && _mainGun.ProjectileType.name != projectileTypes[0].name) {
                _mainGun.ProjectileType = projectileTypes[0];
                _mainGun.firingForce = 1000f;
            }

            else if (Input.GetMouseButton(1) && _mainGun.ProjectileType.name != projectileTypes[1].name) {
                _mainGun.ProjectileType = projectileTypes[1];
                _mainGun.firingForce = 480f;
            }

            Vector2 mousePos = new Vector2();
            if (Camera.main.orthographic) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            _mainGun.Fire(mousePos, "Enemy", playerController.OC.EnergyCell, initialProjVelocity);
        }

        // Manage chaning firing rate
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftShift) && wheelDelta > 0) {
            _mainGun.FiringDelay += _firingDelayChange;
            OnChangeFiringRateEvent();
        }
        else if (Input.GetKey(KeyCode.LeftShift) && wheelDelta < 0) {
            _mainGun.FiringDelay -= _firingDelayChange;
            OnChangeFiringRateEvent();
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
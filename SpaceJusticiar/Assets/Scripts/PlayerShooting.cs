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

    // Use this for initialization
    void Start()
    {
        _mainGun.firingSpeed = 100f;
        _mainGun.FiringDelay = 0.15f;

        prevPos = transform.position;
        prevMousePos = Input.mousePosition;


        firingRateText.text = _mainGun.FiringDelay.ToString("#.##");
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) {
            
            Vector2 mousePos = new Vector2();
            if (Camera.main.orthographic) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else {
                // Still thinking if perspective should be used.
            }

            _mainGun.Fire(mousePos, "Enemy", playerController.EnergyCell);
        }

        // Manage chaning firing rate
        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (wheelDelta > 0) {
            _mainGun.FiringDelay += _firingDelayChange;
        }
        else if (wheelDelta < 0) {
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
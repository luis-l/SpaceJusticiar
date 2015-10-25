using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{

    private Vector3 prevPos = new Vector3();
    private Vector3 prevMousePos = new Vector3();

    private GameObject _projectile;

    private float _firingTimer = 0f;
    private float _firingRate = 0.1f;
    private float _firingSpeed = 30f;

    private AudioSource _laserShotSfx;

    // Use this for initialization
    void Start()
    {
        prevPos = transform.position;
        prevMousePos = Input.mousePosition;

        _projectile = Resources.Load("Prefabs/EnergyProjectile") as GameObject;
        _laserShotSfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_firingTimer >= _firingRate && Input.GetMouseButton(0)) {
            GameObject proj = Pools.Instance.Fetch(_projectile.name);

            proj.SetActive(true);

            Vector2 toMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            toMouse.Normalize();

            proj.transform.position = transform.position;
            proj.GetComponent<Rigidbody2D>().velocity = toMouse * _firingSpeed;

            _firingTimer = 0f;

            _laserShotSfx.Play();
        }
        if (_firingTimer < _firingRate)
            _firingTimer += Time.deltaTime;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (prevPos != transform.position || prevMousePos != Input.mousePosition) {

            Vector3 toMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            toMouse.Normalize();

            toMouse.x = Mathf.Abs(toMouse.x) * toMouse.x;
            toMouse.y = Mathf.Abs(toMouse.y) * toMouse.y;

            float rotationZ = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            prevPos = transform.position;
            prevPos = Input.mousePosition;
        }
    }
}
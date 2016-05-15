using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour
{

    public float life = 2f;
    private float _lifeTimer;

    // What the projectile can hit.
    public string targetTag = "None";

    public float energyCost = 0.03f;

    public float gravityScale = 0f;
    private GameObject _planet = null;

    private Rigidbody2D _rigid = null;

    public float damage = 0.2f;

    [SerializeField]
    private string _explosionTypeName = "GreenEnergyExplosion";

    // Use this for initialization
    void Start()
    {
        _lifeTimer = life;
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Tick life, once life runs out, recycle the projectile.
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0) {
            Pools.Instance.Recycle(gameObject);
        }
    }

    void FixedUpdate()
    {
        // Apply gravity to projectile.
        if (gravityScale != 0 && _planet != null) {
            Vector2 up = (transform.position - _planet.transform.position).normalized;
            _rigid.AddForce(-up * gravityScale);
        }
    }

    // Reset projectile timer, so it is ready to be reused.
    void OnEnable()
    {
        _lifeTimer = life;
    }

    void OnDisable()
    {
        _planet = null;
    }

    // Sets the planet the influences the projectile.
    public void SetPlanet(GameObject planet)
    {
        _planet = planet;
    }

    // Need to fix this tag stuff to something more generic
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlanetInfluence") {
            _planet = other.gameObject;
            transform.parent = _planet.transform;
        }

        if (other.tag == "Collidable" || other.tag == targetTag){

            // Make bullet impacts responsive near the player by shaking the camera a bit.
            if (other.tag != "Player") {
                Vector2 distToCam = Camera.main.transform.position - transform.position;
                float mag = distToCam.magnitude;
                if (mag < 15) {

                    // The closer the impact, the stronger the camera shake.
                    float shakeScalar = 5f / (mag * mag);

                    CameraShake camShake = Camera.main.gameObject.GetComponent<CameraController>().CameraShake;
                    camShake.duration = 0.2f;
                    camShake.magnitude = shakeScalar;
                    camShake.speed = 3f;
                    camShake.PlayShake();

                }
            }

            GameObject explosion = Pools.Instance.Fetch(_explosionTypeName);
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Pools.Instance.Recycle(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "PlanetInfluence") {
            _planet = null;
            transform.parent = null;
        }
    }

    public Rigidbody2D RigidBody { get { return _rigid; } }
}

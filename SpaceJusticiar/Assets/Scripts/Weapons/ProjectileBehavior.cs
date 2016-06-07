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
    private CelestialBody _planet= null;

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
            Vector2 up = CelestialBody.GetUp(_planet, transform);
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
    public void SetPlanet(CelestialBody planet)
    {
        _planet = planet;
    }

    // Need to fix this tag stuff to something more generic
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlanetInfluence") {
            _planet = other.transform.parent.gameObject.GetComponent<CelestialBody>();
            transform.parent = _planet.transform;
        }

        else if (other.tag == "Reflector") {
            _rigid.velocity = -_rigid.velocity * 0.8f;
            other.GetComponent<AudioSource>().Play();
        }

        else if (other.tag == "Collidable" || other.tag == targetTag) {

            ObjectController oc = other.GetComponent<ObjectController>();
            if (oc != null) {
                Vector2 velDiff = oc.RigidBody.velocity - _rigid.velocity;
                oc.ApplyDamage(damage * velDiff.magnitude * 0.06f);
            }

            // Make bullet impacts responsive near the player by shaking the camera a bit.
            if (other.tag != "Player") {
                Vector2 distToCam = Camera.main.transform.position - transform.position;
                float mag = distToCam.magnitude;
                if (mag < 20) {

                    // The closer the impact, the stronger the camera shake.
                    float shakeScalar = 1f / (mag * mag);
                    if (shakeScalar > 0.4f) {
                        shakeScalar = 0.4f;
                    }

                    CameraShake camShake = Systems.Instance.SystemUI.CameraController.CameraShake;
                    camShake.duration = 0.3f;
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

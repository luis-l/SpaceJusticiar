using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBehavior : MonoBehaviour
{

    public float life = 2f;
    private float _lifeTimer;

    // What the projectile can hit.
    public string targetTag = "None";

    public float energyCost = 0.03f;

    public float gravityScale = 0f;
    private CelestialBody _planet = null;

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

            Reflect(other.transform);

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
                NearImpactShake();
            }

            // Player is hit.
            else {
                OnHitShake();
            }

            GameObject explosion = Pools.Instance.Fetch(_explosionTypeName);
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Pools.Instance.Recycle(gameObject);
        }
    }

    private void Reflect(Transform other)
    {
        Vector2 toReflector = transform.position - other.position;

        // Normal between reflector and projectile
        Vector2 normal = toReflector.normalized;

        // The tangent of the normal
        Vector2 tangent = new Vector2(normal.y, -normal.x);

        // Project the velocity of the projectile to the normal;
        float velNormalProjection = Vector2.Dot(_rigid.velocity, normal);

        // Project the velocity of the projectile to the tanget.
        float velTangentProjection = Vector2.Dot(_rigid.velocity, tangent);

        // Convert projections to vectors.
        Vector2 reflectedNormal = normal * velNormalProjection;
        Vector2 reflectedTangent = tangent * velTangentProjection;

        Vector2 reflection = -reflectedNormal + reflectedTangent;

        _rigid.velocity = reflection * 0.7f;
    }

    private void NearImpactShake()
    {
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

    private void OnHitShake()
    {
        CameraShake camShake = Systems.Instance.SystemUI.CameraController.CameraShake;
        camShake.duration = 0.75f;
        camShake.magnitude = Mathf.Clamp(damage * 15, 0f, 1.5f);
        camShake.speed = 5f;
        camShake.PlayShake();

        Systems.Instance.SystemUI.CameraController.FillScreen(new Color(1, 1, 1, damage * 10), 0.1f);
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

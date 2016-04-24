using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour
{

    public float life = 2f;
    private float lifeTimer;

    // What the projectile can hit.
    public string targetTag = "None";

    public Text scoreValueText = null;
    private static int _playerScore = 0;

    public float energyCost = 0.03f;

    public float gravityScale = 0f;
    private GameObject _planet = null;

    private Rigidbody2D _rigid = null;

    public float damage = 0.2f;

    public string explosionName = "GreenEnergyExplosion";

    // Use this for initialization
    void Start()
    {
        lifeTimer = life;
        scoreValueText = GameObject.Find("Canvas/Score/ScoreValue").GetComponent<Text>();
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Tick life, once life runs out, recycle the projectile.
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0) {
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
        lifeTimer = life;
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
        }

        if (other.tag == "Collidable" || other.tag == targetTag){

            if (other.tag == "Enemy") {
                _playerScore += 100;
                scoreValueText.text = _playerScore.ToString(); 
            }

            // Make bullet impacts responsive near the player by shaking the camera a bit.
            if (other.tag != "Player") {
                Vector2 distToCam = Camera.main.transform.position - transform.position;
                float mag = distToCam.magnitude;
                if (mag < 25) {

                    // The closer the impact, the stronger the camera shake.
                    float shakeScalar = 2 / mag;

                    CameraShake camShake = Camera.main.gameObject.GetComponent<CameraController>().CameraShake;
                    camShake.duration = 0.1f;
                    camShake.magnitude = shakeScalar;
                    camShake.speed = 1f;
                    camShake.PlayShake();

                }
            }

            GameObject explosion = Pools.Instance.Fetch(explosionName);
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
        }
    }

    public Rigidbody2D RigidBody { get { return _rigid; } }
}

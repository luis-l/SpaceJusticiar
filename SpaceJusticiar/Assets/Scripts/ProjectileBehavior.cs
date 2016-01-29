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

    public static float energyConsumption = 0.03f;

    public float gravityScale = 0f;
    public GameObject planet = null;

    private Rigidbody2D _rigid = null;

    public float damage = 0.2f;

    // Use this for initialization
    void Start()
    {
        lifeTimer = life;
        scoreValueText = GameObject.Find("Canvas/Score/ScoreValue").GetComponent<Text>();
        _rigid = GetComponent<Rigidbody2D>();

        planet = PlanetController.planet;
    }

    // Update is called once per frame
    void Update()
    {

        // Tick life, once life runs out, recycle the projectile.
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0) {
            Pools.Instance.Recycle(gameObject);
        }

        // Apply gravity to projectile.
        if (gravityScale != 0 && planet != null) {
            Vector2 up = transform.position - planet.transform.position;
            _rigid.AddForce(-up * gravityScale);
        }
    }

    // Reset projectile timer, so it is ready to be reused.
    void OnEnable()
    {
        lifeTimer = life;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Collidable" || other.tag == targetTag){

            if (other.tag == "Enemy") {
                _playerScore += 100;
                scoreValueText.text = _playerScore.ToString(); 
            }

            GameObject explosion = Pools.Instance.Fetch("EnergyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Pools.Instance.Recycle(gameObject);
        }
    }
}

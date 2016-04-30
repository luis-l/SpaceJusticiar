using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
    private GameObject _targetPlanet = null;

    public EnemySpawner enemySpawner = null;
    public LaserCannon mainGun = null;
    public TargetingSystem targetingSystem = null;

    private HealthComponent _health = null;
    private EnergyCell _energyCell = null;

    public GameObject initialProjectileType = null;

    public Sprite secondFormSprite = null;
    public GameObject secondFormProjectileType = null;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);

        _health = new HealthComponent();

        mainGun.FiringDelay = 0.2f;
        mainGun.firingForce = 2500f;
        mainGun.ProjectileType = initialProjectileType;

        initialProjectileType.GetComponent<ProjectileBehavior>().explosionName = "RedEnergyExplosion";
    }

    // Update is called once per frame
    void Update()
    {
        _energyCell.Update();
        _health.Update();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant to hit enemy
            ProjectileBehavior proj = other.gameObject.GetComponent<ProjectileBehavior>();
            if (proj.targetTag == gameObject.tag) {

                _health.DealDamage(proj.damage);

                if (_health.GetHealth() == 0) {
                    enemySpawner.fighterCount--;
                    Destroy(gameObject, 0.1f);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Convert fighter to second form
        if (other.gameObject.name == _targetPlanet.name) {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity.Set(0, 0);
            rigid.isKinematic = true;

            GetComponent<SpriteRenderer>().sprite = secondFormSprite;
            mainGun.ProjectileType = secondFormProjectileType;
            mainGun.FiringDelay = 0.05f;
            mainGun.firingForce = 1000;
            mainGun.spread = 2f;

            AudioSource gunSound = mainGun.GetComponent<AudioSource>();
            gunSound.pitch = 1.4f;
            gunSound.volume = 0.14f;
        }
    }

    public void SetPlanetTarget(GameObject planet){
        transform.parent = planet.transform;
        _targetPlanet = planet;
    }
}

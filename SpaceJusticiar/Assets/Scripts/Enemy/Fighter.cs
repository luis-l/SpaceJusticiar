using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
    float _rangeSq = 30f * 30f;

    public Transform targetTrans;
    public Rigidbody2D targetRigid = null;
    
    public GameObject planet = null;
    public EnemySpawner enemySpawner = null;
    public LaserCannon mainGun = null;

    private HealthComponent _health = null;
    private EnergyCell _energyCell = null;

    private bool _bTargetInRange = false;
    private bool _bTargetInSight = false;

    public GameObject initialProjectileType = null;

    public Sprite secondFormSprite = null;
    public GameObject secondFormProjectileType = null;
    private bool _bInSecondForm = false;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);

        _health = new HealthComponent();

        mainGun.FiringDelay = 0.2f;
        mainGun.firingForce = 5000f;
        mainGun.ProjectileType = initialProjectileType;

        planet = GameObject.Find("Planet");

        initialProjectileType.GetComponent<ProjectileBehavior>().explosionName = "RedEnergyExplosion";
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTrans != null) {

            Vector2 distToTarget = targetTrans.position - transform.position;

            if (distToTarget.sqrMagnitude <= _rangeSq) {

                _bTargetInRange = true;
                Vector2 toTarget = distToTarget.normalized;

                // Fire at the player
                if (_bTargetInSight) {
                    float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                    mainGun.transform.rotation = Quaternion.Euler(0, 0, rotZ);

                    Vector2 fireAtPos;

                    // Make it fire around the target (random positions close to target) - simulate machine gun spread.
                    if (_bInSecondForm) {

                        // Make it aim at the front of the player. Temporary approach, will use something better to lead target.
                        Vector2 targetForward = targetRigid.velocity.normalized;

                        if (targetRigid.velocity.sqrMagnitude > 50f) {
                            Vector2 spawnDir = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
                            fireAtPos = (Vector2)targetTrans.position + 4 * targetForward + spawnDir * Random.Range(0.3f, 1.4f);
                        }
                        else {
                            Vector2 spawnDir = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
                            fireAtPos = (Vector2)targetTrans.position + spawnDir * Random.Range(0.1f, 0.5f);
                        }
                    }
                    else {
                        fireAtPos = targetTrans.position;
                    }

                    mainGun.Fire(fireAtPos, "Player", _energyCell, new Vector2(0, 0));
                }
            }

            else {
                _bTargetInRange = false;
            }
        }

        _energyCell.Update();
        _health.Update();
    }

    void FixedUpdate()
    {
        if (_bTargetInRange && targetTrans != null) {

            Vector2 toTarget = (targetTrans.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget);

            // Did not hit player.
            if (hit.collider != null && hit.collider.gameObject == targetTrans.gameObject) {
                _bTargetInSight = true;
            }
            else {
                _bTargetInSight = false;
            }
        }
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
        if (other.gameObject.name == planet.name) {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity.Set(0, 0);
            rigid.isKinematic = true;

            GetComponent<SpriteRenderer>().sprite = secondFormSprite;
            mainGun.ProjectileType = secondFormProjectileType;
            mainGun.FiringDelay = 0.05f;
            mainGun.firingForce = 2000;

            AudioSource gunSound = GetComponent<AudioSource>();
            gunSound.pitch = 1.4f;
            gunSound.volume = 0.14f;
            _bInSecondForm = true;
        }
    }
}

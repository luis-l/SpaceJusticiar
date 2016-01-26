using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
    float _rangeSq = 20f * 20f;

    public Transform targetTrans;
    public GameObject planet = null;
    public EnemySpawner enemySpawner = null;
    public LaserCannon mainGun = null;

    private HealthComponent _health = null;
    private EnergyCell _energyCell = null;

    private bool _bTargetInRange = false;
    private bool _bTargetInSight = false;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);

        _health = new HealthComponent();

        mainGun.FiringDelay = 0.2f;
        mainGun.firingForce = 5000f;

        planet = GameObject.Find("Planet");
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
                    mainGun.Fire(targetTrans.position, "Player", _energyCell);
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
            if (other.gameObject.GetComponent<ProjectileBehavior>().targetTag == gameObject.tag) {

                _health.DealDamage(0.4f);

                if (_health.GetHealth() == 0) {
                    enemySpawner.fighterCount--;
                    Destroy(gameObject, 0.1f);
                }
            }
        }

        else if (other.gameObject.name == planet.name) {
            GetComponent<SpriteRenderer>().material.color = Color.blue;
        }
    }
}

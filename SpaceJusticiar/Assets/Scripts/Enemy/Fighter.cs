using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{

    public Transform target;
    float _rangeSq = 20f * 20f;

    public EnemySpawner enemySpawner = null;

    public LaserCannon mainGun = null;
    private HealthComponent _health = null;

    private EnergyCell _energyCell = null;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);

        _health = new HealthComponent();

        mainGun.FiringDelay = 0.2f;
        mainGun.firingForce = 5000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) {

            Vector2 distToTarget = target.position - transform.position;

            if (distToTarget.sqrMagnitude <= _rangeSq) {

                Vector2 toTarget = distToTarget.normalized;
                float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                mainGun.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                mainGun.Fire(target.position, "Player", _energyCell);
            }
        }

        _energyCell.Update();
        _health.Update();
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
    }
}

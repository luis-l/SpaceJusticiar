using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{

    public Transform target;
    private GameObject _projectile;
    private AudioSource _laserShotSfx;

    float _rangeSq = 20f * 20f;

    float _firingInterval = .2f;
    float _firingTimer = 0f;

    float _health = 1f;

    public EnemySpawner enemySpawner = null;

    // Use this for initialization
    void Start()
    {
        _projectile = Resources.Load("Prefabs/RedEnergyProjectile") as GameObject;
        _laserShotSfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target != null) {

            Vector2 distToTarget = target.position - transform.position;

            if (distToTarget.sqrMagnitude <= _rangeSq) {

                if (_firingTimer >= _firingInterval) {
                    GameObject proj = Pools.Instance.Fetch(_projectile.name);
                    proj.SetActive(true);

                    proj.transform.position = transform.position;
                    Vector2 toTarget = distToTarget.normalized;

                    float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                    proj.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                    proj.GetComponent<Rigidbody2D>().AddForce(toTarget * 5000);
                    proj.GetComponent<ProjectileBehavior>().targetTag = "Player";
                    _firingTimer = 0f;
                    _laserShotSfx.Play();
                }

                _firingTimer += Time.deltaTime;

            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant to hit enemy
            if (other.gameObject.GetComponent<ProjectileBehavior>().targetTag == gameObject.tag) {

                _health -= .4f;

                if (_health <= 0) {
                    enemySpawner.fighterCount--;
                    Destroy(gameObject, 0.1f);
                }
            }
        }
    }
}

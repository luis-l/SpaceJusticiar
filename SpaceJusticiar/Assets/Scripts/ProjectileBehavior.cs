using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour
{

    public float life = 2f;
    private float lifeTimer;

    // Use this for initialization
    void Start()
    {
        lifeTimer = life;
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

    // Reset projectile timer, so it is ready to be reused.
    void OnEnable()
    {
        lifeTimer = life;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Collidable") {

            GameObject explosion = Pools.Instance.Fetch("EnergyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Pools.Instance.Recycle(gameObject);
        }
    }
}

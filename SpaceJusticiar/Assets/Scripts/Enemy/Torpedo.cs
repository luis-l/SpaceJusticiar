using UnityEngine;
using System.Collections;

public class Torpedo : MonoBehaviour {

    [SerializeField]
    private ObjectController _oc;

    // Life timer so torpedoes do not move forever if they miss target.
    private float _lifeTimer = 60f;

    bool _bDead = false;

    public int particleEmissions = 2;

    public float damage;

	// Use this for initialization
    void Start()
    {
        damage = 0.02f;

        Vector2 down = -CelestialBody.GetUp(_oc.PlanetTarget, transform);
        GetComponent<Rigidbody2D>().velocity = down * 5f;

        float angle = Mathf.Atan2(down.y, down.x) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, angle);

        _oc.Health.ReInit(0, 0.1f);
    }
	
	// Update is called once per frame
	void Update () {
        _lifeTimer -= Time.deltaTime;

        if (_lifeTimer <= 0) {
            _oc.Destroy();
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_bDead) return;

        if (other.tag == "Collidable") {

            _bDead = true;

            for (int i = 0; i < particleEmissions; i++) {
                GameObject explosion = Pools.Instance.Fetch("GreenEnergyExplosion");
                explosion.transform.position = transform.position;

                ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
                effect.Play();
            }

            _oc.Destroy();
        }
    }
}

using UnityEngine;
using System.Collections;

public class Torpedo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Collidable" || other.tag == "Projectile") {

            GameObject explosion = Pools.Instance.Fetch("EnergyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Destroy(gameObject, 0.01f);
        }
    }
}

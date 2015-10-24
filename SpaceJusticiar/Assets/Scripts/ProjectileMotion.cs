using UnityEngine;
using System.Collections;

public class ProjectileMotion : MonoBehaviour {

    public Vector3 velocity = new Vector2();

    public float lifeTimer = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position += velocity * Time.deltaTime;

        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0) {
            GameObject.Destroy(gameObject);
        }
	}
}

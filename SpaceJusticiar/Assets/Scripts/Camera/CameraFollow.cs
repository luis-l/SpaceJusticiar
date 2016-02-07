using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
    void Update()
    {
        if (target != null && !GetComponent<CameraShake>().IsShaking()) {
            Vector3 newPos = target.position;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }
}

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform targetTrans;

    public bool bFollow = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
    void Update()
    {
        if (targetTrans != null && bFollow) {
            Vector3 newPos = targetTrans.position;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }
}

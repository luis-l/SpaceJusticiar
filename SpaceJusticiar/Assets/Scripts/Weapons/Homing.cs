using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ProjectileBehavior))]
public class Homing : MonoBehaviour {

    /// <summary>
    /// The target to home into.
    /// </summary>
    public Transform target;

    private Rigidbody2D _rigid;

    public float turningForce = 100f;

	// Use this for initialization
	void Start () {

        _rigid = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        if (target != null) {

            Vector2 toTarget = (target.position - transform.position).normalized;

            _rigid.AddForce(toTarget * turningForce);

            // Cap velocity
            if (_rigid.velocity.magnitude > 30f) {
                _rigid.velocity = _rigid.velocity.normalized * 30f;
            }
        }
    }
}

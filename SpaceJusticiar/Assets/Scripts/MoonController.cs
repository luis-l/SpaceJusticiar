using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour {

    public float rotationSpeed = 1f;
    public float orbitRadius = 200f;
    public float orbitSpeed = 0.01f;
    private float _currentOrbitAngle = 0f;

    public GameObject planet = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

        // Move planet along orbit path.
        _currentOrbitAngle += Time.deltaTime * orbitSpeed;

        // Clamp angle between 0 and 2 pi
        if (_currentOrbitAngle > Mathf.PI * 2) {
            _currentOrbitAngle -= Mathf.PI * 2;
        }

        // Orbit around the sun.
        float x = orbitRadius * Mathf.Cos(_currentOrbitAngle) + planet.transform.position.x;
        float y = orbitRadius * Mathf.Sin(_currentOrbitAngle) + planet.transform.position.y;
        transform.position = new Vector3(x, y, transform.position.z);
	}
}

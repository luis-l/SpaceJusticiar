using UnityEngine;
using System.Collections;
using Vectrosity;

public class CelestialBody : MonoBehaviour {

    // This collider marks the area in which the planet influences a gameobject.
    // In other words, the planet becomes the parent transform of the gameobject.
    private CircleCollider2D _areaOfInfluence = null;

    private float _rotationSpeed = 0f;
    private float _orbitRadius = 200f;
    private float _orbitSpeed = 0.01f;
    private float _currentOrbitAngle = 0f;

    // Do not parent directly.
    // A celestial parent is just for orbital purposes.
    // We do not want moons to rotate if the parent is rotating.
    // We just want the moons to orbit relative to the parent.
    private GameObject _celestialParent = null;

    private Material _material;

    public bool bRotateBody = false;

	// Use this for initialization
    void Start()
    {

        // Create the influence graphic boundry.
        int segments = 45;
        Material mat = Resources.Load<Material>("Materials/Line");
        VectorLine areaInfluenceBorder = new VectorLine("circle", new Vector3[segments * 2], mat, 4);

        areaInfluenceBorder.MakeCircle(transform.position, _areaOfInfluence.radius, segments);
        areaInfluenceBorder.textureScale = 5f;

        VectorManager.ObjectSetup(gameObject, areaInfluenceBorder, Visibility.Dynamic, Brightness.None);
        areaInfluenceBorder.Draw3DAuto();

        GameObject graphic = transform.FindChild("Graphic").gameObject;
        _material = graphic.GetComponent<MeshRenderer>().material;

        // Modify the mesh bounds so the atmosphere does not disappear when the planet mesh goes out of camera view.
        if (_material.shader.name == "PlanetShader" || _material.shader.name == "StarShader") {
            float atmosphereSize = _material.GetFloat("_AtmoSize");

            Mesh planetMesh = graphic.GetComponent<MeshFilter>().mesh;
            Bounds expandedPlanetBounds = new Bounds(planetMesh.bounds.center, planetMesh.bounds.size * atmosphereSize);

            planetMesh.bounds = expandedPlanetBounds;
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (bRotateBody) {
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        }

        // Orbit around the parent
        if (_celestialParent != null) {

            // Move planet along orbit path.
            _currentOrbitAngle += Time.deltaTime * _orbitSpeed;

            // Clamp angle between 0 and 2 pi
            if (_currentOrbitAngle > Mathf.PI * 2) {
                _currentOrbitAngle -= Mathf.PI * 2;
            }

            float x = _orbitRadius * Mathf.Cos(_currentOrbitAngle) + _celestialParent.transform.position.x;
            float y = _orbitRadius * Mathf.Sin(_currentOrbitAngle) + _celestialParent.transform.position.y;
            transform.position = new Vector3(x, y, transform.position.z);
        }
	}

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        _material.SetFloat("_TransformScale", scale);
    }

    /// <summary>
    /// Set the light position of the sun.
    /// </summary>
    /// <param name="pos"></param>
    public void SetSunPos(Vector2 pos)
    {
        _material.SetVector("_SunPos", pos);
    }
}

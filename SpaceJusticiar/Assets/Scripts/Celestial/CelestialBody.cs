using UnityEngine;
using System.Collections;
using Vectrosity;

public class CelestialBody : MonoBehaviour {

    // This collider marks the area in which the planet influences a gameobject.
    // In other words, the planet becomes the parent transform of the gameobject.
    private CircleCollider2D _areaOfInfluence = null;

    public float rotationSpeed = 0f;
    public float orbitRadius = 200f;
    public float orbitSpeed = 0.01f;
    public float currentOrbitAngle = 0f;

    private int _seed;
    public int Seed { get { return _seed; } }

    // Do not parent directly.
    // A celestial parent is just for orbital purposes.
    // We do not want moons to rotate if the parent is rotating.
    // We just want the moons to orbit relative to the parent.
    public GameObject celestialParent = null;
    private GameObject _graphic = null;
    private MeshRenderer _graphicMeshRenderer = null;

    public bool bRotateBody = false;

    void Awake()
    {
        _seed = StarSystem.GetRandomInt();

        Transform aoiTrans = transform.FindChild("AreaOfInfluence");
        if (aoiTrans != null) {
            _areaOfInfluence = aoiTrans.gameObject.GetComponent<CircleCollider2D>();
        }

        // Graphic is the game object that contains the mesh renderer.
        Transform graphicTrans = transform.FindChild("Graphic");
        if (graphicTrans == null) {
            _graphic = gameObject;
        }
        else {
            _graphic = graphicTrans.gameObject;
        }

        _graphicMeshRenderer = _graphic.GetComponent<MeshRenderer>();
    }

	// Use this for initialization
    void Start()
    {

        // Create the influence graphic boundry.
        if (_areaOfInfluence != null) {
            int segments = 45;
            Material mat = Resources.Load<Material>("Materials/Line");
            VectorLine areaInfluenceBorder = new VectorLine("AreaOfInfluence", new Vector3[segments * 2], mat, 4);

            areaInfluenceBorder.MakeCircle(Vector3.zero, _areaOfInfluence.radius, segments);
            areaInfluenceBorder.textureScale = 5f;

            VectorManager.ObjectSetup(gameObject, areaInfluenceBorder, Visibility.Dynamic, Brightness.None);
            areaInfluenceBorder.Draw3DAuto();
        }

        // For planets it is the atmosphere and for the sun it is the light range.
        float glowSize = -1;
        if (_graphicMeshRenderer.material.shader.name == "Custom/Planet Shader") {
            glowSize = _graphicMeshRenderer.material.GetFloat("_AtmoSize");
        }
        else if(_graphicMeshRenderer.material.shader.name == "Custom/Star Shader"){
            glowSize = _graphicMeshRenderer.material.GetFloat("_SunLightSize");
        }

        // Modify the mesh bounds so the atmosphere does not disappear when the planet mesh goes out of camera view.
        if (glowSize != -1) {
            Mesh planetMesh = _graphic.GetComponent<MeshFilter>().mesh;
            Bounds expandedPlanetBounds = new Bounds(planetMesh.bounds.center, planetMesh.bounds.size *  glowSize);
            planetMesh.bounds = expandedPlanetBounds;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if (bRotateBody) {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }

        // Orbit around the parent
        if (celestialParent != null) {

            // Move planet along orbit path.
            currentOrbitAngle += Time.deltaTime * orbitSpeed;

            // Clamp angle between 0 and 2 pi
            if (currentOrbitAngle > Mathf.PI * 2) {
                currentOrbitAngle -= Mathf.PI * 2;
            }

            float x = orbitRadius * Mathf.Cos(currentOrbitAngle) + celestialParent.transform.position.x;
            float y = orbitRadius * Mathf.Sin(currentOrbitAngle) + celestialParent.transform.position.y;
            transform.position = new Vector3(x, y, transform.position.z);
        }
	}

    /// <summary>
    /// Should only be done once when constructing the planet.
    /// </summary>
    /// <param name="scale"></param>
    public void SetScale(float scale)
    {
        _graphic.transform.localScale = new Vector3(scale, scale, scale);
        _graphicMeshRenderer.material.SetFloat("_TransformScale", scale);

        if (_areaOfInfluence != null) {
            _areaOfInfluence.radius = scale * 2.5f;
        }
    }

    /// <summary>
    /// Set the light position of the sun.
    /// </summary>
    /// <param name="pos"></param>
    public void SetSunPos(Vector2 pos)
    {
        _graphicMeshRenderer.material.SetVector("_SunPos", pos);
    }

    public GameObject Graphic
    {
        get { return _graphic; }
    }

    public CircleCollider2D AreaOfInfluence
    {
        get { return _areaOfInfluence; }
    }

    public static Vector2 GetUp(CelestialBody body, Transform t)
    {
        return (t.position - body.transform.position).normalized;
    }
}

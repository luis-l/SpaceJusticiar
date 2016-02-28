using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Vectrosity;

public class PlanetController : MonoBehaviour
{

    public Canvas canvas = null;
    public Text planetIntegrityText = null;
    private float _planetIntegrity = 1f;

    public static GameObject planet = null;

    /// <summary>
    /// This collider marks the area in which the planet influences a gameobject.
    /// In other words, the planet becomes the parent transform of the gameobject.
    /// </summary>
    public CircleCollider2D areaOfInfluence = null;

    public GameObject sun = null;

    private float _rotationSpeed = 0f;
    private float _orbitRadius = 200f;
    private float _orbitSpeed = 0.01f;
    private float _currentOrbitAngle = 0f;

    // Use this for initialization
    void Start()
    {
        planet = gameObject;

        int segments = 45;
        Material mat = Resources.Load<Material>("Materials/Line");
        VectorLine areaInfluenceBorder = new VectorLine("circle", new Vector3[segments * 2], mat, 4);

        areaInfluenceBorder.MakeCircle(transform.position, areaOfInfluence.radius, segments);
        areaInfluenceBorder.textureScale = 5f;

        VectorManager.ObjectSetup(gameObject, areaInfluenceBorder, Visibility.Dynamic, Brightness.None);
        areaInfluenceBorder.Draw3DAuto();

        
        // Modify the mesh bounds so the atmosphere does not disappear when the planet mesh goes out of camera view.
        GameObject graphic = transform.FindChild("Graphic").gameObject;
        Material planetMaterial = graphic.GetComponent<MeshRenderer>().material;

        float atmosphereSize = planetMaterial.GetFloat("_AtmoSize");

        Mesh planetMesh = graphic.GetComponent<MeshFilter>().mesh;
        Bounds expandedPlanetBounds = new Bounds(planetMesh.bounds.center, planetMesh.bounds.size * atmosphereSize);
        planetMesh.bounds = expandedPlanetBounds;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));

        // Move planet along orbit path.
        _currentOrbitAngle += Time.deltaTime * _orbitSpeed;

        // Clamp angle between 0 and 2 pi
        if (_currentOrbitAngle > Mathf.PI * 2) {
            _currentOrbitAngle -= Mathf.PI * 2;
        }

        // Orbit around the sun.
        float x = _orbitRadius * Mathf.Cos(_currentOrbitAngle) + sun.transform.position.x;
        float y = _orbitRadius * Mathf.Sin(_currentOrbitAngle) + sun.transform.position.y;
        transform.position = new Vector3(x, y, transform.position.z);
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        float damage = 0f;

        if (other.tag == "Projectile") {
            damage = 0.001f;
        }
        else if (other.name == "Torpedo(Clone)") {
            damage = 0.01f;
        }

        _planetIntegrity -= damage;

        if (_planetIntegrity <= 0) {
            Time.timeScale = 0;

            GameObject gameOverObject = new GameObject("GameOver Text");
            gameOverObject.transform.parent = canvas.transform;

            RectTransform rectTrans = gameOverObject.AddComponent<RectTransform>();

            Text text = gameOverObject.AddComponent<Text>();
            text.font = (Font)Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 40;
            text.text = "PLANET DESTROYED! GAME OVER!";
            text.color = new Color(1, 0, 0);
            text.fontStyle = FontStyle.Bold;

            rectTrans.sizeDelta = new Vector2(800, 100);
            rectTrans.localPosition = new Vector3(0, 0, 0);
        }

        if (damage != 0) {
            int integrityPercent = Mathf.CeilToInt(_planetIntegrity * 100);
            planetIntegrityText.text = integrityPercent.ToString();
        }
    }
}

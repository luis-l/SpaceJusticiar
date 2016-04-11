using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates and manages a star system which may contain multiple planets, suns, moons,
/// and other celestial bodies.
/// The star system is seeded so it always generates the same celestial bodies.
/// </summary>
public class StarSystem : MonoBehaviour {

    // The center of mass of the solar system. Stars and planets orbit around this.
    // A single star will be treated as the bary center for simplicity.
    private Vector2 _baryCenter = new Vector2(0, 0);

    private List<CelestialBody> _celestialBodies;

	// Use this for initialization
	void Start () {
        _celestialBodies = new List<CelestialBody>();
        CreateSystem();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CreateSystem()
    {
        CreateStars();
        CreatePlanets();
        CreateAsteroids();
    }

    private void CreateStars()
    {
        // Single star
        if (Random.value < SpaceEngine.chanceSingleStar) {
            CreateStar(_baryCenter);
        }

        // Binary star
        else {
            //CreateStar(_baryCenter);
            //CreateStar(_baryCenter);
        }
    }

    private void CreateStar(Vector2 pos)
    {
        GameObject star = GameObject.Instantiate(ResourceManager.CelestialResources.StarPrefab);

        MeshRenderer meshRenderer = star.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(ResourceManager.CelestialResources.StarShader);


        float lightSize = Random.Range(1.5f, 2.5f);
        float lightGradient = Random.Range(0.4f, 1f);
        float brightness = Random.Range(1.5f, 2.5f);

        //Color color = Color.HSVToRGB();
        
        CelestialBody body = star.GetComponent<CelestialBody>();

        

        float bodySize = Random.Range(20f, 60);
        body.SetScale(bodySize);

    }

    private void CreatePlanets()
    {

    }

    private void CreateMoons(CelestialBody parentPlanet)
    {

    }

    private void CreateAsteroids()
    {

    }
}

using UnityEngine;
using System.Collections.Generic;

public enum SurfaceDetail {NONE, VERY_LOW, LOW, MED, HIGH, ULTRA };

/// <summary>
/// Creates and manages a star system which may contain multiple planets, suns, moons,
/// and other celestial bodies.
/// The star system is seeded so it always generates the same celestial bodies.
/// </summary>
public class StarSystem
{
    // The center of mass of the solar system. Stars and planets orbit around this.
    // A single star will be treated as the bary center for simplicity.
    GameObject _barycenter = new GameObject();

    private List<CelestialBody> _suns;
    private List<CelestialBody> _planets;
    //private List<CelestialBody> _moons;
    //private List<CelestialBody> _asteroids;

    private int _seed = 0;

    public void Init()
    {
        _seed = GetRandomInt();

        _barycenter.transform.position = Vector3.zero;
        _barycenter.name = "Barycenter";

        _suns = new List<CelestialBody>();
        _planets = new List<CelestialBody>();
        //_moons = new List<CelestialBody>();
        //_asteroids = new List<CelestialBody>();
        CreateSystem();
    }

    private void CreateSystem()
    {
        Random.seed = _seed;

        CreateStars();
        CreatePlanets();
        CreateAsteroids();
    }

    private void CreateStars()
    {
        // Single star
        if (Random.value < SpaceEngine.chanceSingleStar) {
            CreateStar(_barycenter.transform.position);
        }

        // Binary star
        else {
            CreateStar(_barycenter.transform.position, 2.0f);
            CreateStar(_barycenter.transform.position, -2.0f);
        }
    }

    private void CreateStar(Vector2 pos, float offset = 0)
    {
        GameObject star = GameObject.Instantiate(ResourceManager.CelestialResources.StarPrefab);
        CelestialBody body = star.GetComponent<CelestialBody>();
        _suns.Add(body);

        MeshRenderer renderer = body.Graphic.GetComponent<MeshRenderer>();
        renderer.material = new Material(ResourceManager.CelestialResources.StarShader);

        float size = Random.Range(35f, 60f);
        body.SetScale(size);

        Color color = Color.HSVToRGB(Random.value, Random.Range(0.2f, 0.5f), 1.0f);
        float lightSize = Random.Range(1.5f, 2.5f);
        float lightGradient = Random.Range(0.4f, 1f);
        float brightness = Random.Range(1.5f, 2.5f);

        // Set the shader values
        renderer.material.SetColor("_SunLightColor", color);
        renderer.material.SetFloat("_SunLightSize", lightSize);
        renderer.material.SetFloat("_SunLightGradientExp", lightGradient);
        renderer.material.SetFloat("_SunLightBrightness", brightness);

        float totalOffset = offset * size;
        star.transform.position = pos + new Vector2(totalOffset, 0); ;

        /* Make the stars orbit the barycenter
        if (offset != 0) {
            body.celestialParent = _barycenter;
            body.orbitRadius = totalOffset;
            body.orbitSpeed = 0.01f;
        }*/
    }

    private void CreatePlanets()
    {
        float currentPlanetOrbitRadius = 400f + Random.Range(0, 50);

        int planetCount = Random.Range(SpaceEngine.MIN_PLANETS_PER_SYSTEM, SpaceEngine.MAX_PLANETS_PER_SYSTEM);
        //int planetCount = 1;

        for (int i = 0; i < planetCount; i++) {

            CelestialBody planet = CreatePlanet(currentPlanetOrbitRadius);

            if (currentPlanetOrbitRadius < 1500f) {
                currentPlanetOrbitRadius *= Random.Range(1.5f, 1.8f);
            }
            else {
                currentPlanetOrbitRadius += Random.Range(300f, 500f);
            }

            planet.name += i;
        }
    }

    private CelestialBody CreatePlanet(float orbitRadius)
    {
        GameObject planet = GameObject.Instantiate(ResourceManager.CelestialResources.PlanetPrefab);
        planet.transform.position = _barycenter.transform.position + new Vector3(orbitRadius, 0, 0);

        CelestialBody body = planet.GetComponent<CelestialBody>();
        _planets.Add(body);

        body.currentOrbitAngle = Random.Range(0, 2 * Mathf.PI);

        GenerateSurface(SurfaceDetail.LOW, body);

        MeshRenderer renderer = body.Graphic.GetComponent<MeshRenderer>();
        renderer.material = new Material(ResourceManager.CelestialResources.PlanetShader);

        MeshRenderer backgroundRenderer = body.Background.GetComponent<MeshRenderer>();
        backgroundRenderer.material = new Material(ResourceManager.CelestialResources.PlanetBackgroundShader);

        float bodySize = Random.Range(50, 60);
        body.SetScale(bodySize);
        body.SetSunPos(_barycenter.transform.position);

        // Set planet color
        float hue = Random.value;
        float value = Random.Range(0.8f, 1.0f);
        float sat = Random.Range(0.7f, 1.0f);
        Color surfaceColor = Color.HSVToRGB(hue, sat, value);

        float lightAttenA = 0.0018f;
        float lightAttenB = 0.0f;
        float albedo = Random.Range(3.8f, 5.0f);
        float emission = Random.Range(0.02f, 0.025f);

        renderer.material.color = surfaceColor;
        renderer.material.SetFloat("_LightAttenA", lightAttenA);
        renderer.material.SetFloat("_LightAttenB", lightAttenB);
        renderer.material.SetFloat("_Albedo", albedo);
        renderer.material.SetFloat("_Emission", emission);

        backgroundRenderer.material.color = surfaceColor * 0.8f;
        backgroundRenderer.material.SetFloat("_LightAttenA", lightAttenA);
        backgroundRenderer.material.SetFloat("_LightAttenB", lightAttenB);
        backgroundRenderer.material.SetFloat("_Albedo", albedo + 0.2f);
        backgroundRenderer.material.SetFloat("_Emission", emission);

        // Random atmosphere hue/
        //Color atmoColor = Color.HSVToRGB(Random.value, 1.0f, 1.0f);

        // Atmosphere color somewhat matches its hue to the planet
        float atmoHue = Mathf.Abs(hue + Random.Range(-0.15f, 0.15f));
        Color atmoColor = Color.HSVToRGB(atmoHue, 1.0f, 1.0f);

        renderer.material.SetColor("_AtmoColor", atmoColor);
        renderer.material.SetFloat("_AtmoSize", Random.Range(1.25f, 1.4f));
        renderer.material.SetFloat("_AtmoGradientExp", 0.5f);
        renderer.material.SetFloat("_AtmoBrightness", Random.Range(3.5f, 4.4f));
        renderer.material.SetFloat("_AtmoEmission", 0.16f);

        // Set planet rotations and orbit speeds
        //
        //
        //

        body.celestialParent = _barycenter;
        body.orbitRadius = orbitRadius;
        body.orbitSpeed = 2.0f / orbitRadius;

        return body;
    }

    public static void GenerateSurface(SurfaceDetail detail, CelestialBody body)
    {
        int vertexCount = 200 * (int)detail;

        MeshFilter surfaceFilter = body.Graphic.GetComponent<MeshFilter>();
        //surfaceFilter.mesh = MeshMaker.MakePlanetSurface(100, body.Seed);
        surfaceFilter.mesh = MeshMaker.MakeCircle(150);

        MeshFilter backFilter = body.Background.GetComponent<MeshFilter>();
        backFilter.mesh = MeshMaker.MakePlanetSurface(vertexCount, body.Seed);
        
        /*
        // Setup the polygon collider that corresponds to the mesh
        Vector2[] polyPoints = new Vector2[surfaceFilter.mesh.vertexCount];

        // Start at second vertex since the first one is the center of the mesh.
        for (int i = 1; i < surfaceFilter.mesh.vertexCount; i++) {
            polyPoints[i-1] = surfaceFilter.mesh.vertices[i];
        }

        // Connect the last vertex with the second vertex to create a closed outer polygon.
        polyPoints[surfaceFilter.mesh.vertexCount-1] = polyPoints[0];

        EdgeCollider2D bounds = body.Graphic.GetComponent<EdgeCollider2D>();
        if (bounds == null) {
            bounds = body.Graphic.AddComponent<EdgeCollider2D>();
        }

        bounds.points = polyPoints; 
        */
        CircleCollider2D bounds = body.Graphic.AddComponent<CircleCollider2D>();
        bounds.radius = body.transform.localScale.x;
    }

    private void CreateMoons(CelestialBody parentPlanet)
    {

    }

    private void CreateAsteroids()
    {

    }

    public CelestialBody GetPlanet(int index)
    {
        return _planets[index];
    }

    public CelestialBody GetStar(int index)
    {
        return _suns[index];
    }

    public static int GetRandomInt(){
        return Random.Range(0, int.MaxValue);
    }

    public List<CelestialBody> Planets { get { return _planets; } }
}

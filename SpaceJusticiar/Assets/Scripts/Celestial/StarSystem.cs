using UnityEngine;
using System.Collections.Generic;

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

    public void Init()
    {
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

        MeshRenderer renderer = body.Graphic.GetComponent<MeshRenderer>();
        renderer.material = new Material(ResourceManager.CelestialResources.PlanetShader);

        float bodySize = Random.Range(10, 30);
        body.SetScale(bodySize);
        body.SetSunPos(_barycenter.transform.position);

        // Set planet color
        float hue = Random.value;
        float value = Random.Range(0.8f, 1.0f);
        float sat = Random.Range(0.7f, 1.0f);
        renderer.material.color = Color.HSVToRGB(hue, sat, value);

        renderer.material.SetFloat("_LightAttenA", 0.0018f);
        renderer.material.SetFloat("_LightAttenB", 0.0f);
        renderer.material.SetFloat("_Albedo", Random.Range(3.8f, 5.0f));
        renderer.material.SetFloat("_Emission", Random.Range(0.02f, 0.025f));

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
}

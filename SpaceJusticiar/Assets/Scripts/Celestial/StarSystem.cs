using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates and manages a star system which may contain multiple planets, suns, moons,
/// and other celestial bodies.
/// The star system is seeded so it always generates the same celestial bodies.
/// </summary>
public class StarSystem {

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

        Color color = Color.HSVToRGB(Random.value, Random.Range(0.2f, 0.6f), 1.0f);
        float lightSize = Random.Range(1.5f, 2.5f);
        float lightGradient = Random.Range(0.4f, 1f);
        float brightness = Random.Range(1.5f, 2.5f);

        // Set the shader values
        renderer.material.SetColor("_SunLightColor", color);
        renderer.material.SetFloat("_SunLightSize", lightSize);
        renderer.material.SetFloat("_SunLightGradientExp", lightGradient);
        renderer.material.SetFloat("_SunLightBrightness", brightness);

        star.transform.position = pos + new Vector2(offset * size, 0);
    }

    private void CreatePlanets()
    {
        float currentPlanetOrbitRadius = 300f + Random.Range(0, 50);
        
        int planetCount = Random.Range(1, 15);
        for (int i = 0; i < planetCount; i++) {

            CreatePlanet(currentPlanetOrbitRadius);
            currentPlanetOrbitRadius *= Random.Range(1.7f, 2.8f);
        }
    }

    private void CreatePlanet(float orbitRadius)
    {
        GameObject planet = GameObject.Instantiate(ResourceManager.CelestialResources.PlanetPrefab);
        CelestialBody body = planet.GetComponent<CelestialBody>();
        _planets.Add(body);

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

        renderer.material.SetFloat("_LightAttenA", 0.002f);
        renderer.material.SetFloat("_LightAttenB", 0.0f);
        renderer.material.SetFloat("_Albedo", Random.Range(3.8f, 5.0f));
        renderer.material.SetFloat("_Emission", Random.Range(0.01f, 0.02f));

        Color atmoColor = Color.HSVToRGB(Random.value, 1.0f, 1.0f);
        renderer.material.SetColor("_AtmoColor", atmoColor);
        renderer.material.SetFloat("_AtmoSize", Random.Range(1.25f, 1.4f));
        renderer.material.SetFloat("_AtmoGradientExp", 0.5f);
        renderer.material.SetFloat("_AtmoBrightness", Random.Range(4.5f, 5.4f));
        renderer.material.SetFloat("_AtmoEmission", 0.156f);

        // Set planet rotations and orbit speeds
        //
        //
        //

        body.celestialParent = _barycenter;
        body.orbitRadius = orbitRadius;
        body.orbitSpeed = 10 / orbitRadius;
    }

    private void CreateMoons(CelestialBody parentPlanet)
    {

    }

    private void CreateAsteroids()
    {

    }
}

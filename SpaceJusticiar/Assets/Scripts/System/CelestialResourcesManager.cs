using UnityEngine;
using System.Collections.Generic;

public class CelestialResourcesManager
{

    private const string _celestialPrefabPath = "Prefabs/Celestial/";

    private readonly GameObject _planetPrefab;
    private readonly GameObject _starPrefab;
    private readonly GameObject _moonPrefab;

    private readonly Shader _planetShader;
    private readonly Shader _starShader;

    public CelestialResourcesManager()
    {
        _planetPrefab = Resources.Load(_celestialPrefabPath + "Planet") as GameObject;
        _starPrefab = Resources.Load(_celestialPrefabPath + "Star") as GameObject;
        _moonPrefab = Resources.Load(_celestialPrefabPath + "Moon") as GameObject;

        _planetShader = Shader.Find("Custom/PlanetShader");
        _starShader = Shader.Find("Custom/StarShader");
    }

    public GameObject PlanetPrefab
    {
        get { return _planetPrefab; }
    }

    public GameObject StarPrefab
    {
        get { return _starPrefab; }
    }

    public GameObject MoonPrefab
    {
        get { return _moonPrefab; }
    }

    public Shader PlanetShader
    {
        get { return _planetShader; }
    }

    public Shader StarShader
    {
        get { return _starShader; }
    }
}

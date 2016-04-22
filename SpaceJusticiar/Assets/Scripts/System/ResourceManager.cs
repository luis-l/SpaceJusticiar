using UnityEngine;
using System.Collections;

public class ResourceManager
{

    private static CelestialResourcesManager _celestialResources;

    // Use this for initialization
    public static void Init()
    {
        _celestialResources = new CelestialResourcesManager();
    }

    public static CelestialResourcesManager CelestialResources
    {
        get { return _celestialResources; }
    }

}

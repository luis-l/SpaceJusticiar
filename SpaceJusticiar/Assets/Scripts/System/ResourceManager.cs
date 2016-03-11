using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour
{

    private static CelestialResourcesManager _celestialResources;

    // Use this for initialization
    void Start()
    {
        _celestialResources = new CelestialResourcesManager();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static CelestialResourcesManager CelestialResources
    {
        get { return _celestialResources; }
    }

}

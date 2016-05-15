using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages star systems. Contains data about how star systems are created.
/// For example triple star systems are rare while single/binary systems are much more frequent.
/// </summary>
public class SpaceEngine : SystemBase
{

    /// <summary>
    /// The chance to make a single star system.
    /// </summary>
    public static float chanceSingleStar = 0.7f;

    public const int MAX_PLANETS_PER_SYSTEM = 9;
    public const int MIN_PLANETS_PER_SYSTEM = 1;

    private StarSystem _currentStarSys = null;
    public StarSystem ActiveStarSystem { get { return _currentStarSys; } }

    public SpaceEngine()
    {
        _currentStarSys = new StarSystem();
        _currentStarSys.Init();
    }

    // Update is called once per frame
    public override void Update()
    {
        for (int i = 0; i < _currentStarSys.Planets.Count; i++) {
            CelestialBody body = _currentStarSys.GetPlanet(i);

            float aoiRadius = body.AreaOfInfluence.radius * 2;
            float camSize = SystemUI.MAX_CAM_SIZE;

            Vector3 camWorldPos = Camera.main.transform.position;

            float sqDist = (body.transform.position - camWorldPos).sqrMagnitude;
            float radiiSum = aoiRadius + camSize;

            if (sqDist < radiiSum * radiiSum) {
                body.ActivateGraphic(true);
            }
            else {
                body.ActivateGraphic(false);
            }
        }
    }
}

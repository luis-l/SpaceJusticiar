using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages star systems. Contains data about how star systems are created.
/// For example triple star systems are rare while single/binary systems are much more frequent.
/// </summary>
public class SpaceEngine : MonoBehaviour {

    /// <summary>
    /// The chance to make a single star system.
    /// </summary>
    public static float chanceSingleStar = 0.7f;

    public const int MAX_PLANETS_PER_SYSTEM = 10;
    public const int MIN_PLANETS_PER_SYSTEM = 1;


    public StarSystem currentStarSys = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < currentStarSys.Planets.Count; i++) {
            CelestialBody body = currentStarSys.GetPlanet(i);

            float aoiRadius = body.AreaOfInfluence.radius * 2;
            float camSize = GameController.MAX_CAM_SIZE;

            Vector3 camWorldPos = Camera.main.transform.position;

            float sqDist = (body.transform.position - camWorldPos).sqrMagnitude;
            float radiiSum = aoiRadius + camSize;

            if (sqDist < radiiSum * radiiSum){
                body.ActivateGraphic(true);
            }
            else {
                body.ActivateGraphic(false);
            }
        }
	}
}

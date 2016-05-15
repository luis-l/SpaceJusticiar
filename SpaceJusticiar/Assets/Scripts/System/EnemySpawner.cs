using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public GameObject player;
    private GameObject _frigatePrefab;

    private const string ENEMY_PREFAB_PATH = "Prefabs/Enemy/";

    // Use this for initialization
    void Start()
    {
        CelestialBody planet = Systems.Instance.SpaceEngine.ActiveStarSystem.GetPlanet(0);

        _frigatePrefab = Resources.Load(ENEMY_PREFAB_PATH + "EnemyFrigate") as GameObject;

        // Spawn frigate
        int frigateCount = 2;
        for (int i = 1; i <= frigateCount; i++) {

            GameObject frigate = GameObject.Instantiate(_frigatePrefab);
            frigate.transform.parent = planet.transform;

            Ship ship = frigate.GetComponent<Ship>();
            ship.planetTarget = planet;
            ship.OrbitingHeight = planet.AreaOfInfluence.radius * 1.5f * i;
            ship.targetOrbitHeightFactor = 1 + i / 3.5f;
            ship.elevationSpeed = 0.05f;
            ship.SetCurrentOrbitAngle(Random.Range(0, 2 * Mathf.PI));
            ship.target = player;
        }
    }
}

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public GameObject player;
    private GameObject _planet;
    private CircleCollider2D _planetCollider;
    private GameObject _torpedoPrefab;
    private GameObject _fighterPrefab;

    private const string ENEMY_PREFAB_PATH = "Prefabs/Enemy/";

    private const int MAX_TORPEDOES = 10;
    private const int MAX_FIGHTERS = 15;
    private float torpedoSpawnInterval = 1.5f;
    private float torpedoSpawnTimer = 0f;

    public int fighterCount = 0;

    public GameController gameController;

    // Use this for initialization
    void Start()
    {
        _planet = gameController.StarSystem.GetPlanet(0).gameObject;
        _torpedoPrefab = Resources.Load(ENEMY_PREFAB_PATH + "Torpedo") as GameObject;
        _fighterPrefab = Resources.Load(ENEMY_PREFAB_PATH + "Fighter") as GameObject;

        _planetCollider = _planet.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTorpedoes();
    }

    void SpawnTorpedoes()
    {
        if (torpedoSpawnTimer >= torpedoSpawnInterval) {

            GameObject torpedo = GameObject.Instantiate(_torpedoPrefab);
            torpedo.transform.parent = _planet.transform;

            // Create a random direction vector.
            Vector2 spawnDir = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
            Vector2 spawnPos = spawnDir * _planetCollider.radius * 4 + (Vector2)_planet.transform.position;

            Vector2 toPlanetCenterDir = (Vector2)_planet.transform.position - spawnPos;
            toPlanetCenterDir.Normalize();

            torpedo.transform.position = spawnPos;
            torpedo.GetComponent<Rigidbody2D>().velocity = toPlanetCenterDir * 5f;

            torpedo.transform.Rotate(0, 0, Mathf.Atan2(toPlanetCenterDir.y, toPlanetCenterDir.x) * Mathf.Rad2Deg);

            torpedoSpawnTimer = 0f;

            // Spawn fighter. Temporary
            if (fighterCount < MAX_FIGHTERS && Random.value < 0.22f) {
                fighterCount++;

                GameObject fighterObj = GameObject.Instantiate(_fighterPrefab);
                Fighter fighterComponent = fighterObj.GetComponent<Fighter>();
                fighterComponent.SetPlanetTarget(_planet);
                fighterComponent.enemySpawner = this;
                fighterObj.transform.position = spawnPos;

                if (player != null) {
                    fighterComponent.targetTrans = player.transform;
                    fighterComponent.targetRigid = player.GetComponent<Rigidbody2D>();
                }

                fighterObj.GetComponent<Rigidbody2D>().AddForce(toPlanetCenterDir * 70f);
            }
        }

        torpedoSpawnTimer += Time.deltaTime;
    }
}

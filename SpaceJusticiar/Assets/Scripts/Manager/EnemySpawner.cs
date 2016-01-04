using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public GameObject player;
    public GameObject planet;
    private CircleCollider2D _planetCollider;
    private GameObject _torpedoPrefab;
    private GameObject _fighterPrefab;

    private const string ENEMY_PREFAB_PATH = "Prefabs/Enemy/";

    private const int MAX_TORPEDOES = 10;
    private const int MAX_FIGHTERS = 12;
    private float torpedoSpawnInterval = 1.5f;
    private float torpedoSpawnTimer = 0f;

    public int fighterCount = 0;


    // Use this for initialization
    void Start()
    {
        _torpedoPrefab = Resources.Load(ENEMY_PREFAB_PATH + "Torpedo") as GameObject;
        _fighterPrefab = Resources.Load(ENEMY_PREFAB_PATH + "Fighter") as GameObject;

        _planetCollider = planet.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTorpedoes();
    }

    void spawnTorpedoes()
    {
        if (torpedoSpawnTimer >= torpedoSpawnInterval) {

            GameObject torpedo = GameObject.Instantiate(_torpedoPrefab);

            // Create a random direction vector.
            Vector2 spawnDir = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
            Vector2 spawnPos = spawnDir * (_planetCollider.radius + Random.Range(10, 30));

            Vector2 toPlanetCenterDir = (Vector2)planet.transform.position - spawnPos;
            toPlanetCenterDir.Normalize();

            torpedo.transform.position = spawnPos;
            torpedo.GetComponent<Rigidbody2D>().velocity = toPlanetCenterDir * 5f;

            torpedo.transform.Rotate(0, 0, Mathf.Atan2(toPlanetCenterDir.y, toPlanetCenterDir.x) * Mathf.Rad2Deg);

            torpedoSpawnTimer = 0f;

            // Spawn fighter. Temporary
            if (fighterCount < MAX_FIGHTERS && Random.value < 0.18f) {
                fighterCount++;

                GameObject fighter = GameObject.Instantiate(_fighterPrefab);
                fighter.transform.position = Random.value < 0.5f ? spawnPos / 2f : spawnPos;

                Fighter fighterComponent = fighter.GetComponent<Fighter>();
                fighterComponent.enemySpawner = this;

                if (player != null)
                    fighterComponent.target = player.transform;
                
                //Vector2 right = new Vector2(toPlanetCenterDir.y, -toPlanetCenterDir.x);
                //fighter.GetComponent<Rigidbody2D>().velocity = right * 2f;
                //fighter.GetComponent<Rigidbody2D>().AddForce(toPlanetCenterDir * 10f);
            }
        }

        torpedoSpawnTimer += Time.deltaTime;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{

    public CelestialBody planetTarget = null;

    private float _orbitingHeight = 0;

    // Zero means stationary
    public float orbitingSpeed = 0f;

    private float _currentOrbitAngle = 0f;

    // How fast the ship moves vertically relative to the planet.
    public float elevationSpeed = 0f;

    // How high up from the planet's area of influence the ship will the target to be at.
    public float targetOrbitHeightFactor = 1.2f;

    [SerializeField]
    private Transform _dockingBay = null;

    public GameObject target = null;

    [System.Serializable]
    public class SpawnData
    {
        public GameObject spawnType;
        public float spawnRate = 2;
        public int maxSpawnCount = 10;
        public bool bVariableRate = false;
    }

    [SerializeField]
    private SpawnData[] _spawnTypes;
    private List<Spawner> _spawners;

    // Use this for initialization
    void Start()
    {
        _spawners = new List<Spawner>(_spawnTypes.Length);
        foreach (SpawnData sd in _spawnTypes) {
            Spawner spawner = new Spawner(_dockingBay, sd.spawnType, sd.spawnRate, sd.maxSpawnCount, sd.bVariableRate);
            spawner.OnSpawnEvent += OnSpawn;
            _spawners.Add(spawner);
        }

        // Set the ship height if it is too close the planet
        if (planetTarget != null && _orbitingHeight < planetTarget.AreaOfInfluence.radius) {
            _orbitingHeight = planetTarget.AreaOfInfluence.radius * 2f;
        }

        // Have all frigate canons target the player.
        foreach (TargetingSystem ts in gameObject.GetComponentsInChildren<TargetingSystem>()) {
            ts.targetTrans = target.transform;

            Vector2 nozzlePos = ts.mainGun.GetNozzle().position;
            nozzlePos.x = 1.0f;
            nozzlePos.y = 0.0f;
            ts.mainGun.GetNozzle().localPosition = nozzlePos;

            float firingForce = 3000f;
            float firingDelay = 0.2f;

            if (ts.mainGun.ProjectileType.name == "RedEnergyProjectile") {
                firingForce = 2500f;
                firingDelay = 0.35f;
            }
            else if (ts.mainGun.ProjectileType.name == "BlueProjectile") {
                firingForce = 1800f;
                firingDelay = 0.05f;
            }

            ts.mainGun.FiringDelay = firingDelay;
            ts.mainGun.firingForce = firingForce;
            ts.Range = 50f;
        }

        foreach (Spawner s in _spawners) {
            s.StartSpawning();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _spawners.Count; i++) {
            _spawners[i].Update();
        }

        if (planetTarget != null && elevationSpeed != 0) {
            
            orbitingSpeed = 1.0f / OrbitingHeight;

            float targetHeight = planetTarget.AreaOfInfluence.radius * targetOrbitHeightFactor;
            OrbitingHeight = Mathf.Lerp(OrbitingHeight, targetHeight, Time.deltaTime * elevationSpeed);
        }

        // Orbit around the parent
        if (planetTarget != null && orbitingSpeed != 0) {

            // Move planet along orbit path.
            _currentOrbitAngle += Time.deltaTime * orbitingSpeed;

            // Clamp angle between 0 and 2 pi
            if (_currentOrbitAngle > Mathf.PI * 2) {
                _currentOrbitAngle -= Mathf.PI * 2;
            }

            float x = _orbitingHeight * Mathf.Cos(_currentOrbitAngle) + planetTarget.transform.position.x;
            float y = _orbitingHeight * Mathf.Sin(_currentOrbitAngle) + planetTarget.transform.position.y;
            transform.position = new Vector3(x, y, transform.position.z);

            // Rotate the body of the ship
            Vector2 up = CelestialBody.GetUp(planetTarget, transform);
            Vector2 right = new Vector2(up.y, -up.x);

            float angle = Mathf.Atan2(right.y, right.x);
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }

    // Setup the planet and game object target to focus on.
    void OnSpawn(ObjectController oc)
    {
        oc.PlanetTarget = planetTarget;

        TargetingSystem ts = oc.gameObject.GetComponentInChildren<TargetingSystem>();
        if (ts != null) {
            ts.targetTrans = target.transform;
        }
    }

    public float OrbitingHeight
    {
        get { return _orbitingHeight; }
        set
        {
            _orbitingHeight = value;
            if (planetTarget != null && value < planetTarget.AreaOfInfluence.radius) {
                _orbitingHeight = planetTarget.AreaOfInfluence.radius * 2f;
            }
        }
    }

    public void SetCurrentOrbitAngle(float radians)
    {
        _currentOrbitAngle = radians;
    }
}

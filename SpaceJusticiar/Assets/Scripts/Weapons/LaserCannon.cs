using UnityEngine;

public class LaserCannon : MonoBehaviour
{
    private float _lowestFiringDelay = 0.05f;
    private float _highestFiringDelay = 0.25f;

    public float firingForce = 3000f;

    // Timer to limit the firing rate.
    private CountUpTimer _firingTimer = null;

    [SerializeField]
    private Transform _nozzleTrans = null;

    [SerializeField]
    private GameObject _projectileType = null;
    private EnergyObject _projectileBehavior = null;

    public AudioSource firingSfx = null;

    public float spread = 0f;

    [SerializeField]
    private float _firingDelay = 0.1f;

    void Awake()
    {
        if (_nozzleTrans == null)
            _nozzleTrans = transform;

        _firingTimer = new CountUpTimer(_firingDelay);
    }

    void Start()
    {
        _projectileBehavior = _projectileType.GetComponent<EnergyObject>();
    }

    /// <summary>
    /// Fires a projectiles at the target position, and consumes
    /// energy from the cell.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector2 targetPos, string targetTag, EnergyCell energyCell, Vector2 initialVelocity, Transform targetTrans = null)
    {
        if (!_firingTimer.HasStarted()) {
            _firingTimer.Start();
        }

        if (_firingTimer.IsDone() && energyCell.UseEnergy(_projectileBehavior.energyCost)) {
            FireProjectile(targetPos, targetTag, energyCell, initialVelocity, targetTrans);

            // Allow for 'single fire' mode by doing a forced reset of the timer.
            _firingTimer.Restart();
        }
    }

    public float FiringDelay
    {
        get { return _firingTimer.TargetTime; }
        set
        {
            if (value > _highestFiringDelay)
                _firingTimer.TargetTime = _highestFiringDelay;

            else if (value < _lowestFiringDelay)
                _firingTimer.TargetTime = _lowestFiringDelay;

            else
                _firingTimer.TargetTime = value;
        }
    }

    private void FireProjectile(Vector2 targetPos, string targetTag, EnergyCell energyCell, Vector2 initialVelocity, Transform targetTrans)
    {
        Vector2 toTarget = targetPos - (Vector2)_nozzleTrans.position;

        if (spread != 0) {
            toTarget += spread * Random.insideUnitCircle;
        }

        toTarget.Normalize();

        GameObject proj = Pools.Instance.Fetch(_projectileType.name);
        proj.transform.position = _nozzleTrans.position;

        // Setup missile
        if (_projectileType.name == "Missile" && targetTrans != null) {
            proj.GetComponent<Homing>().target = targetTrans;
        }

        if (proj.tag == "Projectile") {

            Rigidbody2D projRigid = proj.GetComponent<Rigidbody2D>();
            projRigid.velocity = initialVelocity;
            projRigid.AddForce(toTarget * firingForce);

            EnergyProjectileBehavior projBehav = proj.GetComponent<EnergyProjectileBehavior>();
            projBehav.targetTag = targetTag;

            // Rotate based on target position direction
            Vector2 velDir = toTarget;
            float z = Mathf.Atan2(velDir.y, velDir.x);
            proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * z);
        }

        else if (proj.tag == "Beam") {

            BeamBehavior beam = proj.GetComponent<BeamBehavior>();
            beam.targetPosition = targetPos;
            beam.Project();
        }

        else {
            Debug.LogWarning("Unkown object to fire, name = " + proj.name);
        }

        firingSfx.Play();
    }

    public GameObject ProjectileType
    {
        get { return _projectileType; }

        set
        {
            _projectileType = value;
            _projectileBehavior = _projectileType.GetComponent<EnergyObject>();
        }
    }

    public void SetNozzle(Transform t)
    {
        _nozzleTrans = t;
    }

    public void SetNozzle(Vector2 pos)
    {
        if (_nozzleTrans != null) {
            _nozzleTrans.position = pos;
        }
    }

    public Transform GetNozzle()
    {
        return _nozzleTrans;
    }
}
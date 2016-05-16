using UnityEngine;
using System.Collections;

public class TargetingSystem : MonoBehaviour
{
    [SerializeField]
    private float _range = 25f;
    private float _rangeSq;

    public Transform targetTrans = null;
    public LaserCannon mainGun = null;

    private bool _bTargetInRange = false;
    private bool _bTargetInSight = false;
    private bool _bNozzleIsClear = true;

    private EnergyCell _energyCell = null;
    public bool bUseTargetLeading = true;

    public bool bManual = false;
    public bool bAutoFire = true;

    public float Range
    {
        get { return _range; }
        set { _range = value; _rangeSq = value * value; }
    }

    public EnergyCell EnergyCell
    {
        get { return _energyCell; }
        set { _energyCell = value; }
    }

    void Awake()
    {
        Range = _range;

        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);
    }

    // Update is called once per frame
    void Update()
    {

        if (bManual) {
            targetTrans = Systems.Instance.SystemUI.selectedTarget;
        }

        if (targetTrans != null) {

            Vector2 distToTarget = targetTrans.position - transform.position;

            // Test if the target is within range.
            _bTargetInRange = distToTarget.sqrMagnitude <= _rangeSq;

            if (_bTargetInRange) {
                Vector2 toTarget = distToTarget.normalized;
                float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                mainGun.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }

            // Fire at the target if constraints are satisfied
            if (_bTargetInSight && _bTargetInRange && _bNozzleIsClear) {

                // Fire when gun is ready - that is, let the script fire on its own.
                if (bManual && bAutoFire || !bManual) {
                    Vector2 toTarget = distToTarget.normalized;
                    FireAtTarget(targetTrans.position, toTarget);
                }
            }
        }

        _energyCell.Update();
    }

    void FixedUpdate()
    {
        if (_bTargetInRange && targetTrans != null) {

            Vector2 toTarget = (mainGun.GetNozzle().position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(mainGun.GetNozzle().position, toTarget);

            // Did not hit player.
            if (hit.collider != null && hit.collider.gameObject == targetTrans.gameObject) {
                _bTargetInSight = true;
            }
            else {
                _bTargetInSight = false;
            }
        }
    }

    // Pass in the position to fire at and the direction to the target.
    void FireAtTarget(Vector2 targetPos, Vector2 toTarget)
    {
        targetPos = TargetLeadPosition(targetPos, ref toTarget);

        float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        mainGun.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        mainGun.Fire(targetPos, targetTrans.tag, _energyCell, new Vector2(0, 0));
    }

    // Get the position to aim at using target leading.
    private Vector2 TargetLeadPosition(Vector2 targetPos, ref Vector2 toTarget)
    {

        float projectileForce = mainGun.firingForce;
        float projectileMass = mainGun.ProjectileType.GetComponent<Rigidbody2D>().mass;
        float projectileSpeed = projectileForce / projectileMass * Time.fixedDeltaTime;

        Vector2 targetVel = targetTrans.gameObject.GetComponent<Rigidbody2D>().velocity;

        // Target leading quadratic coefficients
        // We are solving for time of impact of the projectile and target
        // a * t^2 + b*t + c
        Vector2 targetPosRel = targetPos - (Vector2)transform.position;
        float a = Vector2.Dot(targetVel, targetVel) - projectileSpeed * projectileSpeed;
        float b = 2 * Vector2.Dot(targetPosRel, targetVel);
        float c = Vector2.Dot(targetPosRel, targetPosRel);

        float discriminant = b * b - 4 * a * c;

        // It is possible to hit the target with the projectile
        if (discriminant >= 0) {

            // Note:
            // If discriminant = 0 then there is only 1 solution.
            // If it is > 0 then there are 2 solutions.
            float timeOfImpact = Mathf.Abs((-b - Mathf.Sqrt(discriminant)) / (2 * a));

            // Predict the future position of the target at t = timeOfImpact
            targetPos += targetVel * timeOfImpact;
            toTarget = targetPosRel.normalized;
        }

        return targetPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore some colliders
        if (other.tag == "Projectile" || other.name == "AreaOfInfluence" || other.tag == "Player")
            return;

        _bNozzleIsClear = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.name == "AreaOfInfluence")
            return;

        _bNozzleIsClear = true;
    }
}

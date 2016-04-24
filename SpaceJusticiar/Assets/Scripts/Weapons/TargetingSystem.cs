using UnityEngine;
using System.Collections;

public class TargetingSystem : MonoBehaviour {

    float _rangeSq = 35f * 35f;

    public Transform targetTrans = null;

    public LaserCannon mainGun = null;

    private bool _bTargetInRange = false;
    private bool _bTargetInSight = false;

    public GameObject initialProjectileType = null;

    private EnergyCell _energyCell = null;

	// Use this for initialization
	void Start () {

        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);
        
        mainGun.FiringDelay = 0.17f;
        mainGun.firingForce = 4000f;
        mainGun.ProjectileType = initialProjectileType;

        initialProjectileType.GetComponent<ProjectileBehavior>().explosionName = "RedEnergyExplosion";
	}
	
	// Update is called once per frame
	void Update () {
        if (targetTrans != null) {

            Vector2 distToTarget = targetTrans.position - transform.position;

            if (distToTarget.sqrMagnitude <= _rangeSq) {

                _bTargetInRange = true;
                Vector2 toTarget = distToTarget.normalized;

                // Fire at the player
                if (_bTargetInSight) {
                    float rotZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                    mainGun.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                    mainGun.Fire(targetTrans.position, "Player", _energyCell, new Vector2(0, 0));
                }
            }

            else {
                _bTargetInRange = false;
            }
        }

        _energyCell.Update();
	}

    void FixedUpdate()
    {
        if (_bTargetInRange && targetTrans != null) {

            Vector2 toTarget = (targetTrans.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget);

            // Did not hit player.
            if (hit.collider != null && hit.collider.gameObject == targetTrans.gameObject) {
                _bTargetInSight = true;
            }
            else {
                _bTargetInSight = false;
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class CelestialPhysical : MonoBehaviour
{

    public delegate void OnEnergyObjectHitDelegate(CelestialBody body, EnergyObject energyObject);
    public event OnEnergyObjectHitDelegate OnEnergyObjectHitEvent = delegate { };

    public delegate void OnInvaderLandingDelegate(CelestialBody body, ObjectController oc);
    public event OnInvaderLandingDelegate OnInvaderLandingEvent = delegate { };

    public delegate void OnPhysicalHitDelegate(CelestialBody body, Torpedo torpedo);
    public event OnPhysicalHitDelegate OnPhysicalHitEvent = delegate { };

    [SerializeField]
    private CelestialBody _celestialBody;

    void OnCollisionEnter2D(Collision2D collision)
    {
        FireEvents(collision.collider.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        FireEvents(collider.gameObject);
    }

    private void FireEvents(GameObject go)
    {
        switch (go.tag) {
            case "Enemy":

                Torpedo t = go.GetComponent<Torpedo>();
                if (t == null) {
                    OnInvaderLandingEvent(_celestialBody, go.GetComponent<ObjectController>());
                }
                else {
                    OnPhysicalHitEvent(_celestialBody, t);
                }

                break;

            case "Projectile":
                OnEnergyObjectHitEvent(_celestialBody, go.GetComponent<EnergyObject>());
                break;
        }
    }
}

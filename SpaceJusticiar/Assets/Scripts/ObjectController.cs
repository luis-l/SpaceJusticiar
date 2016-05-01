using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour
{
    private CelestialBody _planetTarget = null;
    private HealthComponent _health = null;

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeathEvent = delegate { };

    void Awake()
    {
        _health = new HealthComponent();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _health.Update();
    }

    public void Destroy()
    {
        OnDeathEvent();
        Destroy(gameObject, 0.01f);
    }

    private void ApplyDamage(float damage)
    {
        _health.DealDamage(damage);

        if (_health.GetHealth() == 0) {
            Destroy();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant this object
            ProjectileBehavior proj = other.gameObject.GetComponent<ProjectileBehavior>();
            if (proj.targetTag == gameObject.tag) {
                ApplyDamage(proj.damage);
            }
        }
    }

    public CelestialBody PlanetTarget
    {
        get { return _planetTarget; }
        set
        {
            transform.parent = value.gameObject.transform;
            _planetTarget = value;
        }
    }

    public HealthComponent Health
    {
        get { return _health; }
    }
}

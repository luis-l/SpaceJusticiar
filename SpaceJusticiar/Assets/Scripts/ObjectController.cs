using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour
{
    private CelestialBody _planetTarget = null;
    
    private HealthComponent _health = null;
    private EnergyCell _energyCell = null;

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeathEvent = delegate { };

    public delegate void OnOcDeathDelegate(ObjectController oc);
    public event OnOcDeathDelegate OnOcDeathEvent = delegate { };

    public delegate void OnDamageDelegate(ObjectController damagedObject, float damage);
    public event OnDamageDelegate OnDamageEvent = delegate { };

    private Rigidbody2D _rigid;

    public Rigidbody2D RigidBody { get { return _rigid; } }

    /// <summary>
    /// Make weaker or stronger.
    /// </summary>
    public float damageMultiplier = 1f;

    void Awake()
    {
        _health = new HealthComponent();
    }

    // Use this for initialization
    void Start()
    {
        OnDamageEvent += Systems.Instance.SystemUI.DisplayDamageFeedback;
        OnOcDeathEvent += Systems.Instance.SystemUI.OnOcDeath;

        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _health.Update();

        if (_energyCell != null) {
            _energyCell.Update();
        }
    }

    public void Destroy()
    {
        OnDeathEvent();
        OnOcDeathEvent(this);
        Destroy(gameObject, 0.001f);

        // Do no let the camera die if it is a child of the game object.
        if (Systems.Instance.SystemUI.CameraController.Parent == transform) {
            Systems.Instance.SystemUI.CameraController.Parent = null;
        }
    }

    public void ApplyDamage(float damage)
    {
        damage *= damageMultiplier;
        OnDamageEvent(this, damage);
        _health.DealDamage(damage);

        if (_health.GetHealth() == 0) {
            GameObject explosion = Pools.Instance.Fetch("BlueEnergyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Destroy();
        }
    }

    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant this object
            ProjectileBehavior proj = other.gameObject.GetComponent<ProjectileBehavior>();
            if (proj.targetTag == gameObject.tag) {
                ApplyDamage(proj.damage);
            }
        }
    }*/

    void OnMouseEnter()
    {

        Systems.Instance.SystemUI.selectedTarget = transform;
    }

    void OnMouseExit()
    {
        //Systems.Instance.SystemUI.selectedTarget = null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        float damage = impactForce * impactForce * 0.01f;

        if (damage > 0.08f) {
            ApplyDamage(damage);
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

    public HealthComponent Health { get { return _health; } }
    public EnergyCell EnergyCell
    {
        get { return _energyCell; }
        set { _energyCell = value; }
    }
}

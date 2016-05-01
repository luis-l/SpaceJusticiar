using UnityEngine;
using System.Collections;

public class Lander : MonoBehaviour
{
    [SerializeField]
    private ObjectController _oc;

    public LaserCannon mainGun = null;
    public TargetingSystem targetingSystem = null;

    private EnergyCell _energyCell = null;

    public Sprite secondFormSprite = null;
    public GameObject secondFormProjectileType = null;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);
        mainGun.FiringDelay = 0.2f;

        Vector2 down = -CelestialBody.GetUp(_oc.PlanetTarget, transform);
        GetComponent<Rigidbody2D>().AddForce(down * 70f);
    }

    // Update is called once per frame
    void Update()
    {
        _energyCell.Update();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Convert fighter to second form
        if (other.gameObject.name == _oc.PlanetTarget.name) {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity.Set(0, 0);
            rigid.isKinematic = true;

            GetComponent<SpriteRenderer>().sprite = secondFormSprite;
            mainGun.ProjectileType = secondFormProjectileType;
            mainGun.FiringDelay = 0.05f;
            mainGun.firingForce = 1500;
            mainGun.spread = 2f;

            AudioSource gunSound = mainGun.GetComponent<AudioSource>();
            gunSound.pitch = 1.4f;
            gunSound.volume = 0.14f;
        }
    }
}

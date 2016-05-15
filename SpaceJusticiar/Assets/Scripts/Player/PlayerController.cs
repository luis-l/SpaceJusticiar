using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public GameController gameController;

    public Rigidbody2D rigidBody;

    public float acceleration = 1f;
    public float maxVelocity = 10f;

    private CelestialBody _planet;
    public float gravityScale = 1f;

    public Transform camTransform = null;
    private CameraController _camController = null;

    private Vector3 _prevPlayerPos;

    private ParticleSystem _thrustParticles;

    private Vector3 _prevThrustDir = new Vector3();

    public enum FrameOfReference { GLOBAL, PLANET };
    public FrameOfReference currentFrameOfRef = FrameOfReference.PLANET;

    private EnergyCell _energyCell = null;

    private float _energySlowTimeDrainRate = 0.24f;
    private bool _bInSlowMotion = false;

    public Text healthText = null;
    public Text energyText = null;

    [SerializeField]
    private ObjectController _oc;

    private Vector2 _thrustDir;

    public float Energy
    {
        get { return _energyCell.Charge; }
    }

    public EnergyCell EnergyCell
    {
        get { return _energyCell; }
    }

    // Use this for initialization
    void Start()
    {
        _planet = gameController.StarSystem.GetPlanet(0);
        transform.parent = _planet.transform;
        transform.localPosition = Vector2.zero;

        _energyCell = new EnergyCell();

        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        _camController = camTransform.gameObject.GetComponent<CameraController>();

        Vector2 newPos = transform.position;
        newPos.y = _planet.transform.position.y + _planet.AreaOfInfluence.radius * 0.67f;
        transform.position = newPos;

        _thrustParticles = GameObject.Find("Player/Thrust").GetComponent<ParticleSystem>();

        _prevPlayerPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Energy > 0) {
            Time.timeScale = 0.5f;
            _bInSlowMotion = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            Time.timeScale = 1f;
            _bInSlowMotion = false;
        }

        if (_bInSlowMotion && Energy > 0) {
            _energyCell.UseEnergy(_energySlowTimeDrainRate * Time.deltaTime);
        }

        // Set to normal time scale if we ran out of energy.
        // Set other values when energy runs out too.
        if (Energy == 0) {
            _bInSlowMotion = false;
            Time.timeScale = 1f;
            energyText.text = "0";
        }

        _energyCell.Update();

        // Energy regen.
        if (Energy < EnergyCell.MAX_ENERGY) {
            energyText.text = _energyCell.GetPercentage().ToString();
        }

        // Health regen.
        if (_oc.Health.GetHealth() < HealthComponent.MAX_HEALTH) {
            healthText.text = _oc.Health.GetPercentage().ToString();
        }

        _thrustDir = GetThrustDirection();
        PlayThrustEffect(_thrustDir * acceleration, _thrustDir);
    }

    void LateUpdate()
    {
        // Make the camera align to the planet's tangent.
        if (currentFrameOfRef == FrameOfReference.PLANET && _prevPlayerPos != transform.position) {

            Vector2 upVector = up();
            float z = Mathf.Acos(upVector.y) * Mathf.Rad2Deg;

            if (upVector.x > 0)
                z *= -1;

            camTransform.eulerAngles = new Vector3(0, 0, z);
            _prevPlayerPos = transform.position;
        }
    }

    void FixedUpdate()
    {

        // If we are activating thrusters
        if (_thrustDir != Vector2.zero) {

            // Cap max velocity.
            if (rigidBody.velocity.SqrMagnitude() > maxVelocity * maxVelocity) {
                //rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxVelocity);
            }

            // Accelerate
            rigidBody.AddForce(_thrustDir * acceleration);

        }

        // Apply gravity and air resistance
        if (currentFrameOfRef == FrameOfReference.PLANET) {
            rigidBody.AddForce(-up() * gravityScale);
        }
    }


    private Vector3 GetThrustDirection()
    {
        Vector2 upDir = new Vector2(0, 0);
        Vector2 rightDir = new Vector2(0, 0);

        // Up thrust.
        if (Input.GetKey("w")) {
            upDir = up();
        }

        // Down thrust.
        else if (Input.GetKey("s")) {
            upDir = -up();
        }

        // Left thrust.
        if (Input.GetKey("a")) {
            rightDir = -right();
        }

        // Right thrust.
        else if (Input.GetKey("d")) {
            rightDir = right();
        }

        return (upDir + rightDir).normalized;
    }

    private void PlayThrustEffect(Vector3 thrustForce, Vector3 thrustDir)
    {
        // Play the thrust particles.
        if (thrustForce.sqrMagnitude > 0f) {
            if (_thrustParticles.isStopped)
                _thrustParticles.Play();

            if (_prevThrustDir != thrustDir) {

                float rotationZ = Mathf.Atan2(thrustDir.y, thrustDir.x) * Mathf.Rad2Deg;
                _thrustParticles.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
                _prevThrustDir = thrustDir;
            }
        }
        else if (_thrustParticles.isPlaying) {
            _thrustParticles.Stop();
        }
    }

    // Calculate the up vector relative to the planet's surface.
    public Vector2 up()
    {
        if (currentFrameOfRef == FrameOfReference.PLANET)
            return CelestialBody.GetUp(_planet, transform);

        // Up relative from the camera's up vector.
        return camTransform.up;
    }

    // Calculate the right vector relative to the planet's surface.
    public Vector2 right()
    {
        Vector2 upDir = up();
        return new Vector2(upDir.y, -upDir.x);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant to hit enemy
            ProjectileBehavior proj = other.gameObject.GetComponent<ProjectileBehavior>();
            if (proj.targetTag == gameObject.tag) {

                CameraShake camShake = _camController.CameraShake;
                camShake.duration = 0.65f;
                camShake.magnitude = 1.5f;
                camShake.speed = 3f;
                camShake.PlayShake();

                _camController.FillScreen(Color.white, 0.1f);
            }
        }

        else if (other.name == "AreaOfInfluence") {

            // Update the planet reference which is the owner of the AreaOfInfluence child object
            _planet = other.transform.parent.gameObject.GetComponent<CelestialBody>();
            transform.parent = _planet.transform;
            
            // Air resistance in planet
            rigidBody.drag = 0.1f;

            StopCoroutine("AlignCameraToPlanetSurface");
            StartCoroutine("AlignCameraToPlanetSurface");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "AreaOfInfluence") {
            StopCoroutine("AlignCameraToPlanetSurface");
            currentFrameOfRef = FrameOfReference.GLOBAL;
            
            // No Drag in space;
            rigidBody.drag = 0f;

            transform.parent = null;
            _planet = null;
        }
    }

    private IEnumerator AlignCameraToPlanetSurface()
    {
        Vector2 normalToPlanetSurface = (transform.position - _planet.transform.position).normalized;

        float z = Mathf.Acos(normalToPlanetSurface.y) * Mathf.Rad2Deg;
        if (normalToPlanetSurface.x > 0) {
            z *= -1;
        }

        Quaternion planetAlignment = Quaternion.Euler(0, 0, z);

        float speed = 6f;

        // Keep slerping until the camera up aligns to the planet surface normal.
        while (Vector2.Angle(normalToPlanetSurface, camTransform.up) > 1) {

            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, planetAlignment, Time.deltaTime * speed);

            // Update planet surface normal since player moves.
            normalToPlanetSurface = (transform.position - _planet.transform.position).normalized;
            z = Mathf.Acos(normalToPlanetSurface.y) * Mathf.Rad2Deg;
            if (normalToPlanetSurface.x > 0) {
                z *= -1;
            }

            planetAlignment = Quaternion.Euler(0, 0, z);
            speed *= 1.01f + Time.deltaTime;

            yield return null;
        }

        currentFrameOfRef = FrameOfReference.PLANET;
    }

}

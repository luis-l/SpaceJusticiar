using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rigidBody;

    public float acceleration = 1f;
    public float maxVelocity = 10f;

    private CelestialBody _planet;
    public float gravityScale = 1f;

    public Transform camTransform = null;

    private Vector3 _prevPlayerPos;

    private ParticleSystem _thrustParticles;

    private Vector3 _prevThrustDir = new Vector3();

    public enum FrameOfReference { GLOBAL, PLANET };
    public FrameOfReference currentFrameOfRef = FrameOfReference.PLANET;

    private float _energySlowTimeDrainRate = 0.24f;
    private bool _bInSlowMotion = false;

    [SerializeField]
    private ObjectController _oc;

    [SerializeField]
    private GameObject _shield;

    private Vector2 _thrustDir;

    public ObjectController OC { get { return _oc; } }

    void Awake()
    {
        _oc.EnergyCell = new EnergyCell(0.1f);
    }

    // Use this for initialization
    void Start()
    {
        _planet = Systems.Instance.SpaceEngine.ActiveStarSystem.GetPlanet(0);
        _oc.PlanetTarget = _planet;

        //transform.parent = _planet.transform;
        transform.localPosition = Vector2.zero;

        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        Vector2 newPos = transform.position;
        newPos.y = _planet.transform.position.y + _planet.GetSurfaceRadius() + 0.1f;
        transform.position = newPos;

        _thrustParticles = GameObject.Find("Player/Thrust").GetComponent<ParticleSystem>();

        _prevPlayerPos = transform.position;

        _shield.SetActive(false);

        Systems.Instance.SystemUI.SetFocusObject(_oc);
    }

    // Update is called once per frame
    void Update()
    {
        HandleSpecial();

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

        // If we are activating thrusters, accelerate
        if (_thrustDir != Vector2.zero) {
            rigidBody.AddForce(_thrustDir * acceleration);
        }

        // Apply gravity
        if (currentFrameOfRef == FrameOfReference.PLANET) {
            rigidBody.AddForce(-up() * gravityScale);
        }
    }


    private void HandleSpecial()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _oc.EnergyCell.Charge > 0) {
            //Time.timeScale = 0.5f;
            _bInSlowMotion = true;
            _shield.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            //Time.timeScale = 1f;
            _bInSlowMotion = false;
            _shield.SetActive(false);
        }

        if (_bInSlowMotion && _oc.EnergyCell.Charge > 0) {
            _oc.EnergyCell.UseEnergy(_energySlowTimeDrainRate * Time.deltaTime);
        }

        // Set to normal time scale if we ran out of energy.
        // Set other values when energy runs out too.
        if (_oc.EnergyCell.Charge == 0) {
            _bInSlowMotion = false;
            //Time.timeScale = 1f;
            _shield.SetActive(false);
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
        if (other.name == "AreaOfInfluence") {

            // Update the planet reference which is the owner of the AreaOfInfluence child object
            _planet = other.transform.parent.gameObject.GetComponent<CelestialBody>();
            transform.parent = _planet.transform;

            // Air resistance in planet
            rigidBody.drag = 0.3f;

            StopCoroutine("AlignCameraToPlanetSurface");
            StartCoroutine("AlignCameraToPlanetSurface");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "AreaOfInfluence") {
            StopCoroutine("AlignCameraToPlanetSurface");
            currentFrameOfRef = FrameOfReference.GLOBAL;
            
            // No Drag in space but we need to limit it
            rigidBody.drag = 0.18f;

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

    public Vector2 Acceleration { get { return _thrustDir * acceleration; } }
}

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;

    public float acceleration = 1f;
    public float maxVelocity = 10f;

    private float _colorTimer = 0;

    // How often to change the player color in seconds.
    public float colorChangeRate = 0.5f;

    private float _minColor = 0.5f;
    private float _maxColor = 1f;

    public GameObject planet;
    public float gravityScale = 1f;

    public Camera camera = null;

    private ParticleSystem _thrustParticles;

    private Vector3 _prevThrustDir = new Vector3();

    public enum FrameOfReference { GLOBAL, PLANET };
    public FrameOfReference currentFrameOfRef = FrameOfReference.PLANET;

    // Use this for initialization
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Vector2 newPos = transform.position;
        newPos.y = planet.transform.position.y + planet.GetComponent<CircleCollider2D>().radius + 1;
        transform.position = newPos;

        _thrustParticles = GameObject.Find("Player/Thrust").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 thrustForce = thrust();

        // Cap velocity if accelerating.
        if (thrustForce.sqrMagnitude != 0 && _rigidBody.velocity.SqrMagnitude() > maxVelocity * maxVelocity) {
            _rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, maxVelocity);
        }

        _rigidBody.AddForce(-up() * gravityScale);
    }

    void LateUpdate()
    {
        // Make the camera align to the planet's tangent.
        camera.gameObject.transform.right = right();
    }

    void FixedUpdate()
    {
        setColor();
    }

    Vector3 thrust()
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            acceleration *= 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) {
            acceleration /= 2;
        }

        Vector3 thrustDir = (upDir + rightDir).normalized;
        Vector3 thrust = thrustDir * acceleration;

        generateThrustParticles(thrust, thrustDir);
        _rigidBody.AddForce(thrust);

        return thrust;
    }

    void generateThrustParticles(Vector3 thrust, Vector3 thrustDir)
    {
        // Play the thrust particles.
        if (thrust.sqrMagnitude > 0f) {
            if (_thrustParticles.isStopped)
                _thrustParticles.Play();

            if (_prevThrustDir != thrustDir) {

                // Set the direction of the particles opposite to the player's motion.
                //_thrustParticles.gameObject.transform.LookAt(transform.position - thrustDir);

                float rotationZ = Mathf.Atan2(thrustDir.y, thrustDir.x) * Mathf.Rad2Deg;
                _thrustParticles.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
                _prevThrustDir = thrustDir;
            }
        }
        else {

            //_thrustParticles.gameObject.transform.LookAt(transform.position + Vector3.forward);

            if (_thrustParticles.isPlaying)
                _thrustParticles.Stop();
        }
    }

    // Calculate the up vector relative to the planet's surface.
    Vector2 up()
    {
        if (currentFrameOfRef == FrameOfReference.PLANET)
            return (transform.position - planet.transform.position).normalized;

        // Global up
        return Vector2.up;
    }

    // Calculate the right vector relative to the planet's surface.
    Vector2 right()
    {
        Vector2 upDir = up();
        return new Vector2(upDir.y, -upDir.x);
    }

    void setColor()
    {
        // Change color based on the timer.
        _colorTimer += Time.deltaTime;
        if (_colorTimer >= colorChangeRate) {
            _colorTimer = 0;

            // Generate a random color.
            float r = Random.Range(_minColor, _maxColor);
            float g = Random.Range(_minColor, _maxColor);
            float b = Random.Range(_minColor, _maxColor);
            Color c = new Color(r, g, b);

            _spriteRenderer.material.color = c;
        }
    }
}

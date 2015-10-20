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

    public Camera camera;

    // Use this for initialization
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Vector2 newPos = transform.position;
        newPos.y = planet.transform.position.y + planet.GetComponent<CircleCollider2D>().radius + 1;
        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {

        thrust();

        if (_rigidBody.velocity.SqrMagnitude() > maxVelocity * maxVelocity) {
            _rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, maxVelocity);
        }

        _rigidBody.AddForce(-up() * gravityScale);

        setColor();

    }

    void FixedUpdate()
    {
        // Make the camera go around the planet.
        camera.gameObject.transform.right = right();
    }

    void thrust()
    {
        Vector2 upDir = new Vector2(0, 0);
        Vector2 rightDir = new Vector2(0, 0);

        // Up - Down movement.
        if (Input.GetKey("w")) {
            upDir = up();
        }

        else if (Input.GetKey("s")) {
            upDir = -up();
        }

        // Left - Right movement.
        if (Input.GetKey("a")) {
            rightDir = -right();
        }

        else if (Input.GetKey("d")) {
            rightDir = right();
        }

        else {

        }

        Vector2 thrust = (upDir + rightDir).normalized * acceleration;
        _rigidBody.AddForce(thrust);

    }

    // Calculate the up vector relative to the planet's surface.
    Vector2 up()
    {
        //return new Vector2(0, 1);
        return (transform.position - planet.transform.position).normalized;

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

    void rotateAroundPlanet()
    {
      
    }
}

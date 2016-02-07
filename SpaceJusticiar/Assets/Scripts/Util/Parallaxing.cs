using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour
{

    // Backgrounds and foregrounds to parallax
    public Transform[] backgrounds;

    // Proportions of camera's movement to the backgrounds
    float[] parallaxScales;

    // How smooth the parallax is going to be. Must be > 0
    public float smoothing = 1f;

    Transform cam;
    Vector3 prevCameraPos;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start()
    {
        prevCameraPos = cam.position;

        // Associate the parallax scales to a background
        int len = backgrounds.Length;
        parallaxScales = new float[len];
        for (int i = 0; i < len; i++)
            parallaxScales[i] = backgrounds[i].position.z * -1f;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++) {
            //the parallax is the opposite of the camera movement
            //because the previous frame is multiplied by the scale
            float xParallax = (prevCameraPos.x - cam.position.x) * parallaxScales[i];
            float yParallax = (prevCameraPos.y - cam.position.y) * parallaxScales[i];
            
            // Apply parallax to the current background's x position
            float newX = backgrounds[i].position.x + xParallax;
            float newY = backgrounds[i].position.y + yParallax;

            // Create a new position for the background with the modified X,Y position
            Vector3 newPos = new Vector3(newX, newY, backgrounds[i].position.z);

            // Fade between the current position and the new position using lerp.
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, newPos, smoothing * Time.deltaTime);
        }
        // Update previous position
        prevCameraPos = cam.position;
    }
}

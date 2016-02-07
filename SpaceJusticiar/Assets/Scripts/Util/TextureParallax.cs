using UnityEngine;
using System.Collections.Generic;

public class TextureParallax : MonoBehaviour
{
    public float smoothing = 1f;
    
    // Parallax scales for each mesh renderer's texture.
    private List<float> _parallaxScales;
    public List<MeshRenderer> meshRenderers;

    // To lerp in between the delta camera position in order to do parallaxing.
    private Vector3 _prevCamPos;
    private Transform _camTransform;

    // private PlayerController _playerController;

    // Use this for initialization
    void Start()
    {
        _prevCamPos = Camera.main.transform.position;
        _parallaxScales = new List<float>();

        foreach (MeshRenderer meshR in meshRenderers) {
            _parallaxScales.Add(meshR.gameObject.transform.position.z * -1f);
        }
    }

    void Awake()
    {
        //_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _camTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        if (_prevCamPos != _camTransform.position) {

            int i = 0;
            foreach (MeshRenderer meshR in meshRenderers) {

                Material mat = meshR.material;
                Vector2 offset = mat.mainTextureOffset;

                // the parallax is the opposite of the camera movement
                // because the previous frame is multiplied by the scale
                float xParallax = (_prevCamPos.x - _camTransform.position.x) * _parallaxScales[i];
                float yParallax = (_prevCamPos.y - _camTransform.position.y) * _parallaxScales[i];


                // Apply parallax to the current background's x position
                float newX = offset.x + xParallax;
                float newY = offset.y + yParallax;

                // Create a new position for the background with the modified X,Y position
                //Vector2 up = _playerController.up();
                Vector2 newOffset = new Vector2(newX, newY);

                // Debug.Log(newOffset + "   " + up);

                if (newOffset.sqrMagnitude < 0.05)
                    continue;

                // Fade between the current position and the new position using lerp.
                mat.mainTextureOffset = Vector2.Lerp(mat.mainTextureOffset, newOffset, smoothing * Time.deltaTime);

                i++;
            }

            // Update previous position
            _prevCamPos = _camTransform.position;
        }
    }
}

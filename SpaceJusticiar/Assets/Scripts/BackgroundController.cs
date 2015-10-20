using UnityEngine;
using System.Collections;

using Vectrosity;

public class BackgroundController : MonoBehaviour {

    private Rect _worldBounds = new Rect(0, 0, 100, 100);
    public int starCount = 1000;
    public float starSize = 1f;

    private int _minStarColor = 140;
    private int _maxStarColor = 255;

    public GameObject planet;

    // Scales the background relative to planet radius.
    private float _backgroundScalar = 1.5f;

	// Use this for initialization
	void Start () {

        float planetRadius = planet.GetComponent<CircleCollider2D>().radius;
        _worldBounds.width = planetRadius * _backgroundScalar * 2;
        _worldBounds.height = planetRadius * _backgroundScalar * 2;
        _worldBounds.center = new Vector2(0, 0);

        // Populate with stars.
        Vector3[] starPositions = new Vector3[starCount];
        
        for(int i = 0; i < starCount; i++) {

            float left = _worldBounds.xMin;
            float right = _worldBounds.xMax;
            float top = _worldBounds.yMax;
            float bottom = _worldBounds.yMin;

            float planetLeft = planet.transform.position.x - planetRadius;
            float planetRight = planet.transform.position.x + planetRadius;
            float planetTop = planet.transform.position.y + planetRadius;
            float planetBottom = planet.transform.position.y - planetRadius;
            
            // How many tries to place a star outside a planet.
            int tries = 0;

            float x = 0;
            float y = 0;

            // Do not put a star in the middle of the planet.
            while (tries < 100){
                x = Random.Range(left, right);
                y = Random.Range(bottom, top);

                if ((x < planetLeft || x > planetRight) || (y < planetBottom || y > planetTop)) {
                    break;
                }

                tries += 1;
            }

            if (x == 0 && y == 0) {
                Debug.Log("Took too long to place star");
            }

            starPositions[i] = new Vector2(x, y);
        }

        Color32[] starColors = new Color32[starCount];
        for (int i = 0; i < starCount; i++) {

            byte r = (byte)Random.Range(_minStarColor, _maxStarColor);
            byte g = (byte)Random.Range(_minStarColor, _maxStarColor);
            byte b = (byte)Random.Range(_minStarColor, _maxStarColor);
            starColors[i] = new Color32(r, g, b, 255);
        }

        VectorPoints stars = new VectorPoints("stars", starPositions, null, starSize);
        stars.SetColors(starColors);
        stars.Draw3DAuto();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

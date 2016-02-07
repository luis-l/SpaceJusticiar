using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlanetController : MonoBehaviour {

    public Canvas canvas = null;
    public Text planetIntegrityText = null;
    private float _planetIntegrity = 1f;

    public static GameObject planet = null;

    public float rotationSpeed = 0.1f;

	// Use this for initialization
	void Start () {
        planet = gameObject;
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        float damage = 0f;

        if (other.tag == "Projectile") {
            damage = 0.001f;
        }
        else if (other.name == "Torpedo(Clone)") {
            damage = 0.01f;
        }

        _planetIntegrity -= damage;

        if (_planetIntegrity <= 0) {
            Time.timeScale = 0;

            GameObject gameOverObject = new GameObject("GameOver Text");
            gameOverObject.transform.parent = canvas.transform;
            
            RectTransform rectTrans = gameOverObject.AddComponent<RectTransform>();

            Text text = gameOverObject.AddComponent<Text>();
            text.font = (Font)Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 40;
            text.text = "PLANET DESTROYED! GAME OVER!";
            text.color = new Color(1, 0, 0);
            text.fontStyle = FontStyle.Bold;

            rectTrans.sizeDelta = new Vector2(800, 100);
            rectTrans.localPosition = new Vector3(0, 0, 0);
        }
        
        if (damage != 0) {
            int integrityPercent = Mathf.CeilToInt(_planetIntegrity * 100);
            planetIntegrityText.text = integrityPercent.ToString();
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBehavior : MonoBehaviour
{

    public float life = 3f;

    private float _lifeCounter;

    public Vector2 endPosition;
    public float lerpSpeed = 0;
    private RectTransform _rectTrans;
    private Text _text;

    void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        _rectTrans.position = Vector2.Lerp(_rectTrans.position, endPosition, lerpSpeed * Time.deltaTime);

        // Lerp the alpha
        Color currentColor = _text.color;
        currentColor.a = Mathf.Lerp(currentColor.a, 0f, Time.deltaTime * 1.2f);
        _text.color = currentColor;

        _lifeCounter -= Time.deltaTime;

        if (_lifeCounter <= 0) {
            UIPools.Instance.Recycle(gameObject);
        }
    }

    void OnEnable()
    {
        _lifeCounter = life;

        // Reset the alpha
        Color color = _text.color;
        color.a = 1f;
        _text.color = color;
    }

    public Text Text { get { return _text; } }
    public RectTransform RectTransform { get { return _rectTrans; } }
}

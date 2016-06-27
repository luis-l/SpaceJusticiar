using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIFader : MonoBehaviour
{


    public float life = 3f;

    private float _lifeCounter;

    public Vector2 endPosition;
    public float lerpSpeed = 0;
    public float alphaDecayRate = 1.2f;

    private RectTransform _rectTrans;

    private List<MaskableGraphic> _graphics = new List<MaskableGraphic>();

    public delegate void OnFaderEndDelegate(UIFader fader);
    public event OnFaderEndDelegate OnFaderEndEvent = delegate { };

    public ObjectController associatedOC;

    void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();

        foreach (MaskableGraphic g in GetComponents<MaskableGraphic>()) {
            _graphics.Add(g);
        }

        foreach (MaskableGraphic g in GetComponentsInChildren<MaskableGraphic>()) {
            _graphics.Add(g);
        }
    }

    void Start()
    {
        OnFaderEndEvent += Systems.Instance.SystemUI.OnUI_FaderEnd;
    }

    // Update is called once per frame
    void Update()
    {

        _rectTrans.position = Vector2.Lerp(_rectTrans.position, endPosition, lerpSpeed * Time.deltaTime);

        for (int i = 0; i < _graphics.Count; i++) {

            MaskableGraphic graphic = _graphics[i];

            // Lerp the alpha
            Color currentColor = graphic.color;
            currentColor.a = Mathf.Lerp(currentColor.a, 0f, Time.deltaTime * alphaDecayRate);
            graphic.color = currentColor;
        }

        _lifeCounter -= Time.deltaTime;

        if (_lifeCounter <= 0) {
            OnFaderEndEvent(this);
            CleanUp();
        }
    }

    void OnEnable()
    {
        ResetLife();
        SetAlpha(1f);
    }

    public void CleanUp()
    {
        associatedOC = null;
        UIPools.Instance.Recycle(gameObject);
    }

    public void SetAlpha(float alpha)
    {
        for (int i = 0; i < _graphics.Count; i++) {

            MaskableGraphic graphic = _graphics[i];

            // Reset the alpha
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }

    public void ResetLife()
    {
        _lifeCounter = life;
    }

    public bool DoneFading
    {
        get { return _lifeCounter <= 0f; }
    }

    public List<MaskableGraphic> Graphics { get { return _graphics; } }
    public RectTransform RectTransform { get { return _rectTrans; } }
}

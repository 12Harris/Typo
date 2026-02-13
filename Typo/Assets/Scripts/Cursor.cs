using UnityEngine;

public class Cursor : MonoBehaviour
{
    private bool show = true;
    private float blinkSeconds = 0.5f;

    private Vector3 _startingPosition;
    public Vector3 StartingPosition => _startingPosition;

    void Awake()
    {
        //_startingPosition = Utils.UIToWorldPosition(GetComponent<RectTransform>(), transform.position, GameObject.Find("RootGame").GetComponent<Canvas>());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("blinkCursor", 0f, blinkSeconds);
        _startingPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reset()
    {
        transform.position = _startingPosition;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    void blinkCursor()
    {
        show = !show;
        gameObject.SetActive(show);
    }
}

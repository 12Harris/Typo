using UnityEngine;
using TMPro;
using System.Runtime.Serialization;

public class NumberButton : PhoneButton
{
    private TextMeshProUGUI _letters;
    private int _currentLetterIndex;
    private float _timeSinceLastButtonClick = -1.0f;

    [SerializeField]
    private float _fastTypeDelay = 0.25f;

    private bool _typeLetter = false;

    [SerializeField]
    private string number;

    private bool _update = true;

    private string _possibleChars;
    public string PossibleChars => _possibleChars;

    public override void Awake()
    {
        base.Awake();
        _letters = transform.Find("Text (TMP) (1)").gameObject.GetComponent<TextMeshProUGUI>();
        _possibleChars = _letters.text + number;
        _currentLetterIndex = 0;
    }

    public override void OnButtonClick()
    {
        // Debug.Log("Number Button clicked!");
        base.OnButtonClick();

        // Enable Update to track timing
        _update = true;

        if (_timeSinceLastButtonClick > _fastTypeDelay)
        {
            _currentLetterIndex = 0;
        }
        else
        {
            _currentLetterIndex++;
            _currentLetterIndex = _currentLetterIndex % _possibleChars.Length;
        }
        _typeLetter = true;
    }

    public void Update()
    {
        if (!_update)
            return;

        _timeSinceLastButtonClick += Time.deltaTime;

        if (_typeLetter)
        {
            PhoneController_Web.Instance.TypeLetter(
                _possibleChars[_currentLetterIndex],
                _timeSinceLastButtonClick < _fastTypeDelay, gameObject);
            _typeLetter = false;
            _timeSinceLastButtonClick = 0.0f;

        }
        if (_timeSinceLastButtonClick > _fastTypeDelay)
        {
            _update = false;
        }
    }
}
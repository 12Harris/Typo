using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class PhoneController_Web : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _inputWord;
    [SerializeField] private InputScreen _inputScreen;
    [SerializeField] private AudioSource audioSource;

    public string InputWord => _inputWord.text;
    public InputScreen InputScreen => _inputScreen;
    public float MusicTime { get; set; }

    private static PhoneController_Web _instance;

    // Audio
    private List<AudioClip> _loadedClips = new List<AudioClip>(12);
    private Dictionary<string, int> _buttonIndexMap = new Dictionary<string, int>(12);

    // Timing
    private float _timeSinceLastFastType = -1.0f;
    private const float FAST_TYPE_THRESHOLD = 0.25f;

    // Remove text
    private float _removeTextDelay = 0.1f;
    private float _removeTextTimer;

    // Cached references
    private NumberButton _pressedNumberButton;
    private StringBuilder _textBuilder = new StringBuilder(100);

    public static PhoneController_Web Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PhoneController_Web>();
                if (_instance == null)
                {
                    Debug.LogError("PhoneController Instance not found in Scene");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Initialize button index map (avoids List.IndexOf lookups)
        _buttonIndexMap["Pad 1"] = 0;
        _buttonIndexMap["Pad 2"] = 1;
        _buttonIndexMap["Pad 3"] = 2;
        _buttonIndexMap["Pad 4"] = 3;
        _buttonIndexMap["Pad 5"] = 4;
        _buttonIndexMap["Pad 6"] = 5;
        _buttonIndexMap["Pad 7"] = 6;
        _buttonIndexMap["Pad 8"] = 7;
        _buttonIndexMap["Pad 9"] = 8;
        _buttonIndexMap["Pad 0"] = 9;
        _buttonIndexMap["Asterisk"] = 10;
        _buttonIndexMap["Hashtag"] = 11;
    }

    private void Start()
    {
        _inputWord.text = string.Empty;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Load audio clips
        string[] clipPaths = {
            "Sounds/button1", "Sounds/button2", "Sounds/button3",
            "Sounds/button4", "Sounds/button5", "Sounds/button6",
            "Sounds/button7", "Sounds/button8", "Sounds/button9",
            "Sounds/button0", "Sounds/button_star", "Sounds/button#"
        };

        foreach (string path in clipPaths)
        {
            AudioClip clip = Resources.Load<AudioClip>(path);
            _loadedClips.Add(clip);
        }
    }

    public void TypeLetter(char letter, bool fastTyping, GameObject phoneButton)
    {
        PlaySound(phoneButton);
        _timeSinceLastFastType = 0.0f;

        int textLength = _inputWord.text.Length;

        if (!fastTyping || textLength == 0)
        {
            Debug.Log("NO FAST TYPING...");
            // Append new character
            _inputWord.text += letter;
            _inputScreen.UpdateCursor(true, false);
        }
        else
        {
            // Replace last character - use StringBuilder to avoid string allocations
            _textBuilder.Clear();
            _textBuilder.Append(_inputWord.text, 0, textLength - 1);
            _textBuilder.Append(letter);
            _inputWord.text = _textBuilder.ToString();
            _inputScreen.UpdateCursor(false, false);
        }

        _pressedNumberButton = phoneButton.GetComponent<NumberButton>();
    }

    private void Update()
    {
        // Cache GameManager reference if accessed frequently
        var gameManager = GameManager_Web.Instance;
        if (gameManager != null && gameManager.AudioSource != null)
        {
            MusicTime = gameManager.AudioSource.time;
        }

        if (_timeSinceLastFastType > -1.0f)
        {
            _timeSinceLastFastType += Time.deltaTime;

            if (_timeSinceLastFastType > FAST_TYPE_THRESHOLD)
            {
                if (gameManager != null && _pressedNumberButton != null)
                {
                    gameManager.CheckPhoneText2(_inputWord.text, _pressedNumberButton);
                }
                _timeSinceLastFastType = -1.0f;
            }
        }
    }

    public void RemoveText(float clickTimer = 0.0f)
    {
        int textLength = _inputWord.text.Length;
        if (textLength == 0) return;

        if (clickTimer <= 0.0f)
        {
            RemoveLastCharacter();
        }
        else if (clickTimer > 1.0f)
        {
            _removeTextTimer += Time.deltaTime;

            if (_removeTextTimer > _removeTextDelay)
            {
                RemoveLastCharacter();
                _removeTextTimer = 0.0f;
                _removeTextDelay -= clickTimer * Time.deltaTime * 0.04f;
            }
        }
    }

    private void RemoveLastCharacter()
    {
        var gameManager = GameManager_Web.Instance;
        if (gameManager != null)
        {
            gameManager.TryDecreasePoints(_inputWord.text);
        }

        _inputWord.text = _inputWord.text.Remove(_inputWord.text.Length - 1);
        _inputScreen.UpdateCursor(false, true);
    }

    public void PlaySound(GameObject phoneButton)
    {
        if (_buttonIndexMap.TryGetValue(phoneButton.name, out int index))
        {
            audioSource.PlayOneShot(_loadedClips[index]);
        }
    }
}
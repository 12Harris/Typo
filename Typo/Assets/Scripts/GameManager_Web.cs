using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager_Web : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private TextAsset wordListFile;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreOver;
    [SerializeField] private TextMeshProUGUI highscore;
    [SerializeField] private Button restart;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> musicClips;

    public TextAsset WordsListFile => wordListFile;
    public AudioSource AudioSource => audioSource;

    private bool isGameOver = false;
    private const float COUNTDOWN_TIME = 64f; // 60 + 4
    private float countDown = COUNTDOWN_TIME;

    private List<string> _wordsTemplate;
    public List<string> WordsTemplate => _wordsTemplate;

    private int _currentWordTemplateIndex;
    public string CurrentWordTemplate { get => WordsTemplate[_currentWordTemplateIndex]; set => WordsTemplate[_currentWordTemplateIndex] = value; }

    [SerializeField] private float _wordsTemplateAnimDelay;

    private static GameManager_Web _instance;
    public static GameManager_Web Instance => _instance;

    private int _points = 0;
    private List<int> _pointsCache = new List<int>(100); // Pre-allocate capacity
    private bool audioStarted = false;

    // Cached strings to avoid allocations
    private const string TIME_FORMAT = "Time: {0:00}:{1:00}";
    private const string SCORE_FORMAT = "Current Score: ";
    private const string HIGHSCORE_FORMAT = "Highscore: ";
    private const string HIGHSCORE_KEY = "highscore";

    // Cache for Update calculations
    private int cachedSeconds = -1;
    private int cachedMinutes = -1;

    void Awake()
    {
        // Singleton pattern without FindFirstObjectByType
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        UIManager_Web._onWordDisappeared += UpdateCurrentWordTemplate;
    }

    void Start()
    {
        int randomIndex = UnityEngine.Random.Range(0, musicClips.Count);
        audioSource.clip = musicClips[randomIndex];
        _currentWordTemplateIndex = 0;
    }

    void Update()
    {
        // Handle audio start with new Input System
        if (!audioStarted && Mouse.current.leftButton.wasPressedThisFrame)
        {
            audioSource.Play();
            audioStarted = true;
            InitializeWordList();
            //UIManager_Web.Initialize(_wordsTemplate.ToArray());
            //UIManager_Web.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
            StartCoroutine(UpdateAnimatedText(_wordsTemplateAnimDelay));
        }

        if (!audioStarted)
            return;

        UpdateTimer();
    }

    // Separated timer logic for clarity and optimization
    private void UpdateTimer()
    {
        float totalSeconds = countDown - PhoneController_Web.Instance.MusicTime;
        int seconds = (int)(totalSeconds % 60);
        int minutes = (int)(totalSeconds / 60f);

        // Only update text if values changed (reduces GC)
        if (!isGameOver && (seconds != cachedSeconds || minutes != cachedMinutes))
        {
            cachedSeconds = seconds;
            cachedMinutes = minutes;
            timerTMP.text = string.Format(TIME_FORMAT, minutes, seconds);
        }

        if (seconds == 0 && minutes == 0 && !isGameOver)
        {
            isGameOver = true;
            GameOver();
        }
    }

    // Pre-allocated word list to avoid LINQ allocations
    private void InitializeWordList()
    {
        // Initialize with capacity to avoid resizing
        /*_wordsTemplate = new List<string>(70)
        {
            "g2g", "brb", "hi", "u", "r", "2nite", "nite", "2morrow", "4ever", "4life",
            "bro", "gf", "bf", "bff", "true", "typo", "hello", "good", "bye", "goodbye",
            "sms", "txt", "cool", "scool", "school", "you", "are", "here", "that", "there",
            "and", "is", "not", "no", "yes", "stop", "go", "state", "left", "right",
            "up", "down", "north", "south", "east", "west", "pen", "paper", "bad", "idk",
            "idc", "phone", "late", "b4", "after", "false", "gone", "boy", "girl", "kid",
            "road", "home", "door", "hill", "tree", "in", "out"
        };*/

        _wordsTemplate = new List<string>(20)
        {
            "g2g", "brb", "hi", "u", "r", "2nite", "nite", "2morrow", "4ever", "4life",
            "bro", "gf", "bf", "bff", "true", "typo", "hello", "good", "bye", "goodbye"
        };

        // Fisher-Yates shuffle (more efficient than LINQ OrderBy)
        ShuffleList(_wordsTemplate);
    }

    // Fisher-Yates shuffle - no LINQ, no allocations
    private void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        restart.onClick.AddListener(OnClickRestart);

        int savedHighscore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        if (_points > savedHighscore)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, _points);
            PlayerPrefs.Save();
            savedHighscore = _points;
        }

        scoreOver.text = SCORE_FORMAT + _points.ToString();
        highscore.text = HIGHSCORE_FORMAT + savedHighscore.ToString();
    }

    private void OnClickRestart()
    {
        audioStarted = false;
        UIManager_Web.Reset();
        SceneLoader.ReloadCurrentScene();
    }

    public void TryDecreasePoints(string inputWord)
    {
        if (inputWord.Length > _wordsTemplate[_currentWordTemplateIndex].Length)
            return;

        int lastIndex = inputWord.Length - 1;
        if (_wordsTemplate[_currentWordTemplateIndex][lastIndex] == inputWord[lastIndex])
        {
            int lastCacheIndex = _pointsCache.Count - 1;
            if (lastCacheIndex >= 0)
            {
                IncreasePoints(-_pointsCache[lastCacheIndex]);
                _pointsCache.RemoveAt(lastCacheIndex);
            }
        }
    }

    public void CheckPhoneText2(string inputWord, NumberButton phoneButton)
    {
        if (_currentWordTemplateIndex >= _wordsTemplate.Count ||
            inputWord.Length > _wordsTemplate[_currentWordTemplateIndex].Length ||
            string.IsNullOrEmpty(inputWord))
            return;

        int i = inputWord.Length - 1;

        if (_wordsTemplate[_currentWordTemplateIndex][i] == inputWord[i])
        {
            int points = (phoneButton.PossibleChars.IndexOf(inputWord[i]) + 1) * 10;
            _pointsCache.Add(points);
            IncreasePoints(points);
        }
    }

    public void ResetInput(string inputWord)
    {
        int currentWordLength = _wordsTemplate[_currentWordTemplateIndex].Length;
        for (int i = inputWord.Length; i > currentWordLength; i--)
        {
            IncreasePoints(-10);
        }
        StartCoroutine(ResetPhoneText());
    }

    public void NextWord(bool increasePoints = true)
    {
        UIManager_Web.SaveOldWordTemplateWordIndex();
        _currentWordTemplateIndex++;
        UIManager_Web.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);

        if (increasePoints)
            UIManager_Web.IncreasePoints(_points);
    }

    private IEnumerator ResetPhoneText()
    {
        yield return new WaitForSeconds(0.2f);
        PhoneController.Instance.InputScreen.ResetText();
    }

    private void UpdateCurrentWordTemplate(bool currentWordDisappeared)
    {
        if (currentWordDisappeared)
        {
            UIManager_Web.SaveOldWordTemplateWordIndex();
            _currentWordTemplateIndex++;
        }
        //UIManager_Web.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
    }

    public void IncreasePoints(int points)
    {
        _points += points;
    }

    private IEnumerator UpdateAnimatedText(float interval)
    {
        WaitForSeconds wait = new WaitForSeconds(interval); // Cache to avoid allocations
        int wordCount = _wordsTemplate.Count;

        for (int i = 0; i < wordCount; i++)
        {
            string word = _wordsTemplate[i];
            for (int j = 0; j < word.Length; j++)
            {
                UIManager_Web.UpdateAnimatedText1(word, j);
                //UIManager_Web.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
                yield return wait;
            }

            if (i < wordCount - 1)
            {
                UIManager_Web.UpdateAnimatedText1("-", 0);
                //UIManager_Web.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
                yield return wait;
            }
            Debug.Log("i = " + i);
        }
        while (UIManager_Web.AnimatedWordsText.Length > 0)
        {
            UIManager_Web.UpdateAnimatedText2();
            yield return wait;
        }


    }

    private void OnDestroy()
    {
        UIManager_Web._onWordDisappeared -= UpdateCurrentWordTemplate;
    }
}
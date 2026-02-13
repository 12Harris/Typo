using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerTMP;

    [SerializeField] TextAsset wordListFile;
    public TextAsset WordsListFile => wordListFile;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI scoreOver;
    [SerializeField] TextMeshProUGUI highscore;
    [SerializeField] Button restart;
    [SerializeField] private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;
    [SerializeField] List<AudioClip> musicClips;
    private bool isGameOver = false;
    private float countDown = 60 + 4; // manual 

    private List<string> _wordsTemplate;

    public List<string> WordsTemplate => _wordsTemplate;

    private int _currentWordIndex = 0;

    public string CurrentWordTemplate => WordsTemplate[_currentWordTemplateIndex];

    private int _currentWordTemplateIndex;

    [SerializeField]
    private float _wordsTemplateAnimDelay;

    private static GameManager _instance;

    private int _points = 0;

    private List<int> _pointsCache = new List<int>();


    private bool audioStarted = false;


    public static GameManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.Log("Error, Game Manager Instance not found in Scene");
                }
            }
            return _instance;

        }
    }

    void Awake()
    {
        UIManager._onWordDisappeared += UpdateCurrentWordTemplate;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int randomIndex = UnityEngine.Random.Range(0, musicClips.Count);
        audioSource.clip = musicClips[randomIndex]; // set a random clip each time
                                                    //audioSource.Play();
        /*_wordsTemplate = new List<string>(wordListFile.text.Split(
            System.Environment.NewLine.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries
        ));

        // randomize the wordlist
        System.Random rnd = new System.Random();
        _wordsTemplate = _wordsTemplate.OrderBy(_ => rnd.Next()).ToList();*/

        // _wordsTemplate = new List<string>
        // { "lorem","ipsum","dolor","sit","amet","consetetur","sadipiscing","elitr","sed","diam"};

        //UIManager.Initialize(_wordsTemplate);
        _currentWordIndex = 0;
        _currentWordTemplateIndex = 0;
        //UIManager.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);

        //StartCoroutine(UpdateAnimatedText(_wordsTemplateAnimDelay));

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioStarted && Mouse.current.leftButton.wasPressedThisFrame)
        {
            audioSource.Play();
            audioStarted = true;

            //wordListFile = Resources.Load<TextAsset>("wordList");

            /*_wordsTemplate = new List<string>(wordListFile.text.Split(
                System.Environment.NewLine.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries
            ));*/


            _wordsTemplate = new List<string>{  "g2g",
                                                "brb",
                                                "hi",
                                                "u",
                                                "r",
                                                "2nite",
                                                "nite",
                                                "2morrow",
                                                "4ever",
                                                "4life",
                                                "bro",
                                                "gf",
                                                "bf",
                                                "bff",
                                                "true",
                                                "typo",
                                                "hello",
                                                "good",
                                                "bye",
                                                "goodbye",
                                                "sms",
                                                "txt",
                                                "cool",
                                                "scool",
                                                "school",
                                                "you",
                                                "are",
                                                "here",
                                                "that",
                                                "there",
                                                "and",
                                                "is",
                                                "not",
                                                "no",
                                                "yes",
                                                "stop",
                                                "go",
                                                "state",
                                                "left",
                                                "right",
                                                "up",
                                                "down",
                                                "north",
                                                "south",
                                                "east",
                                                "west",
                                                "pen",
                                                "paper",
                                                "bad",
                                                "idk",
                                                "idc",
                                                "phone",
                                                "late",
                                                "b4",
                                                "after",
                                                "false",
                                                "gone",
                                                "boy",
                                                "girl",
                                                "kid",
                                                "road",
                                                "home",
                                                "door",
                                                "hill",
                                                "tree",
                                                "in",
                                                "out"};



            // randomize the wordlist
            System.Random rnd = new System.Random();
            _wordsTemplate = _wordsTemplate.OrderBy(_ => rnd.Next()).ToList();
            var wordsTemplateArr = _wordsTemplate.ToArray();

            //StartCoroutine(UIManager.Initialize(wordsTemplateArr));
            //UIManager.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);

        }

        if (!audioStarted)
            return;
        //countDown -= Time.deltaTime;
        /*float seconds = countDown % 60f;
        float minutes = Mathf.Floor(countDown / 60f);
        timerTMP.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);*/

        float totalSeconds = countDown - PhoneController.Instance.MusicTime;
        var seconds = (int)(totalSeconds % 60);
        var minutes = (int)(totalSeconds / 60f);
        if (!isGameOver)
            timerTMP.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);

        if (seconds == 0 && minutes == 0 && !isGameOver)
        {
            // time is over
            isGameOver = true;
            gameOver();
        }
    }

    void gameOver()
    {
        Debug.Log("game over");
        gameOverPanel.SetActive(true);
        restart.onClick.AddListener(onclickRestart);
        if (_points > PlayerPrefs.GetInt("highscore"))
        {
            // new highscore
            PlayerPrefs.SetInt("highscore", _points);
            PlayerPrefs.Save();
        }
        scoreOver.text = "Current Score: " + _points;
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("highscore");

    }
    void onclickRestart()
    {
        SceneLoader.ReloadCurrentScene();
    }
    /*public void CheckPhoneText(string inputWord)
    {

        if (_currentWordTemplateIndex == _wordsTemplate.Count)
            return;

        int i = inputWord.Length - 1;

        if (_wordsTemplate[_currentWordTemplateIndex][i] == inputWord[i])
        {
            IncreasePoints(10);
        }


        if (inputWord == _wordsTemplate[_currentWordTemplateIndex] || inputWord.Length == _wordsTemplate[_currentWordTemplateIndex].Length)
        {
            StartCoroutine(ResetPhoneText());
            if (_currentWordTemplateIndex == _wordsTemplate.Count - 1)
            {
                GameOver();
            }

            _currentWordTemplateIndex++;
        }

    }*/

    public void TryDecreasePoints(string inputWord)
    {
        if (inputWord.Length > _wordsTemplate[_currentWordTemplateIndex].Length)
            return;

        if (_wordsTemplate[_currentWordTemplateIndex][inputWord.Length - 1] == inputWord[inputWord.Length - 1])
        {
            //IncreasePoints(-10);
            IncreasePoints(-_pointsCache[_pointsCache.Count - 1]);
            _pointsCache.RemoveAt(_pointsCache.Count - 1);

        }
    }

    public void CheckPhoneText2(string inputWord, NumberButton phoneButton)
    {

        if (_currentWordTemplateIndex == _wordsTemplate.Count || inputWord.Length > _wordsTemplate[_currentWordTemplateIndex].Length)
            return;

        //This can happen and cause bugs
        if (inputWord == "")
            return;

        int i = inputWord.Length - 1;

        if (_wordsTemplate[_currentWordTemplateIndex][i] == inputWord[i])
        {
            var points = (phoneButton.PossibleChars.IndexOf(inputWord[i]) + 1) * 10;
            _pointsCache.Add(points);
            IncreasePoints(points);
        }


        /*if (inputWord == _wordsTemplate[_currentWordTemplateIndex] || inputWord.Length == _wordsTemplate[_currentWordTemplateIndex].Length)
        {
            //Bug: phone text gets reset before last character is entered correctly
            Debug.Log("PHONE TEXT RESET");
            StartCoroutine(ResetPhoneText());
            if (_currentWordTemplateIndex == _wordsTemplate.Count - 1)
            {
                GameOver();
            }

            _currentWordTemplateIndex++;
            UIManager.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
        }*/
    }

    public void ResetInput(string inputWord)
    {
        //add minus points if input word is longer than current word
        for (int i = inputWord.Length; i > _wordsTemplate[_currentWordTemplateIndex].Length; i--)
        {
            IncreasePoints(-10);
        }
        StartCoroutine(ResetPhoneText());
    }

    public void NextWord()
    {
        UIManager.SaveOldWordTemplateWordIndex();
        _currentWordTemplateIndex++;
        UIManager.UpdateWordsTemplate(_wordsTemplate[_currentWordTemplateIndex]);
        UIManager.IncreasePoints(_points);

    }

    private IEnumerator ResetPhoneText()
    {
        yield return new WaitForSeconds(0.2f);
        PhoneController.Instance.InputScreen.ResetText();
    }

    private void UpdateCurrentWordTemplate()
    {
        _currentWordTemplateIndex++;

    }

    private void GameOver()
    {

    }

    public void IncreasePoints(int points)
    {
        _points += points;
        //UIManager.IncreasePoints(_points);
    }

    private IEnumerator UpdateAnimatedText(float interval)
    {
        int i = 0;
        while (i < _wordsTemplate.Count)
        {
            int j = 0;
            while (j < _wordsTemplate[i].Length)
            {
                UIManager.UpdateAnimatedText(_wordsTemplate[i], j++);
                yield return new WaitForSeconds(interval);
            }
            if (i < _wordsTemplate.Count - 1)
            {
                UIManager.UpdateAnimatedText("-", 0);
                yield return new WaitForSeconds(interval);
            }
            i++;
        }
    }
}

using UnityEngine;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;


/*
Assert.AreEqual(expected, actual);
Assert.IsTrue(condition);
Assert.IsFalse(condition);
Assert.IsNull(value);
Assert.IsNotNull(value);
Assert.Throws<System.Exception>(() => method());
*/


public class PlayerTests
{
    private NumberButton[] _numberButtons;
    private void Initialize()
    {
        _numberButtons = new NumberButton[8];

        _numberButtons[0] = GameObject.Find("Pad 2").GetComponent<NumberButton>();
        _numberButtons[1] = GameObject.Find("Pad 3").GetComponent<NumberButton>();
        _numberButtons[2] = GameObject.Find("Pad 4").GetComponent<NumberButton>();
        _numberButtons[3] = GameObject.Find("Pad 5").GetComponent<NumberButton>();
        _numberButtons[4] = GameObject.Find("Pad 6").GetComponent<NumberButton>();
        _numberButtons[5] = GameObject.Find("Pad 7").GetComponent<NumberButton>();
        _numberButtons[6] = GameObject.Find("Pad 8").GetComponent<NumberButton>();
        _numberButtons[7] = GameObject.Find("Pad 9").GetComponent<NumberButton>();
    }

    [UnityTest]
    public IEnumerator Correctly_Typed_Test()
    {

        SceneManager.LoadScene("JustType2");
        yield return null;
        Initialize();
        GameManager_Web.Instance.InitializeForTests();

        var referenceWord = "abc";
        GameManager_Web.Instance.WordsTemplate[0] = referenceWord;

        var typedWord = "a";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "ab";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "abc";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);

        Assert.IsTrue(GameManager_Web.Instance.Points == 60, "actual points: " + GameManager_Web.Instance.Points);

        yield return null;

    }

    [UnityTest]
    public IEnumerator Incorrectly_Typed_Test()
    {

        SceneManager.LoadScene("JustType2");
        yield return null;
        Initialize();
        GameManager_Web.Instance.InitializeForTests();

        var referenceWord = "abc";
        GameManager_Web.Instance.WordsTemplate[0] = referenceWord;

        var typedWord = "a";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "ab";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "abd";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[1]);

        Assert.IsTrue(GameManager_Web.Instance.Points == 20, "actual points: " + GameManager_Web.Instance.Points);

        yield return null;

    }

    [UnityTest]
    public IEnumerator Removed_Test()
    {

        SceneManager.LoadScene("JustType2");
        yield return null;
        Initialize();
        GameManager_Web.Instance.InitializeForTests();

        var referenceWord = "abc";
        GameManager_Web.Instance.WordsTemplate[0] = referenceWord;

        //Test 1
        var typedWord = "a";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "ab";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "abc";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        GameManager_Web.Instance.RemoveLastLetter(typedWord);//-c=-30

        //60-30-10 = 20
        Assert.IsTrue(GameManager_Web.Instance.Points == 20, "actual points: " + GameManager_Web.Instance.Points);

        //Test 2
        GameManager_Web.Instance.Points = 0;
        typedWord = "a";//points = 10
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "ab";//points = 10+20=30
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[0]);
        typedWord = "abd";
        GameManager_Web.Instance.CheckPhoneText2(typedWord, _numberButtons[1]);
        GameManager_Web.Instance.RemoveLastLetter(typedWord);//-d=-10

        //30-10-10 = 10
        Assert.IsTrue(GameManager_Web.Instance.Points == 10, "actual points: " + GameManager_Web.Instance.Points);

        yield return null;

    }
}
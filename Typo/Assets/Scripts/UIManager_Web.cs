using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public static class UIManager_Web
{
    // Cached component references
    private static TextMeshProUGUI _pointsText;
    private static TextMeshProUGUI _animatedWordsText;
    public static string AnimatedWordsText => _animatedWordsText.text;
    private static GameObject _animatedWordsTemplate;
    private static Vector3 _animatedWordsTemplateContainerStartPos = Vector3.zero;

    private static string _oldWordTemplate = "";
    private static string _currentWordTemplate = "";

    public static int LastWordTemplateOffset { get; set; } = 0;
    public static event Action<bool> _onWordDisappeared;

    // Cache to avoid repeated GetComponent calls
    private static bool _isInitialized = false;

    private static void EnsureInitialized()
    {
        if (_isInitialized) return;

        GameObject pointsObj = GameObject.Find("RootGame/Points");
        _animatedWordsTemplate = GameObject.Find("RootGame/AnimatedWords");

        if (pointsObj != null)
            _pointsText = pointsObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        if (_animatedWordsTemplate != null)
        {
            _animatedWordsText = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            Vector3[] corners = new Vector3[4];
            _animatedWordsTemplate.GetComponent<RectTransform>().GetWorldCorners(corners);
            _animatedWordsTemplateContainerStartPos = (corners[0] + corners[1]) / 2f;
        }

        _isInitialized = true;
        LastWordTemplateOffset = 0;
    }

    public static void Reset()
    {
        _isInitialized = false;
    }

    public static void Initialize(string[] wordsTemplateArr)
    {
        EnsureInitialized();

        if (_animatedWordsText == null) return;

        // Use System.Text.StringBuilder to avoid string concatenation allocations
        System.Text.StringBuilder sb = new System.Text.StringBuilder(wordsTemplateArr.Length * 10);

        for (int i = 0; i < wordsTemplateArr.Length; i++)
        {
            sb.Append(wordsTemplateArr[i]);

            if (i < wordsTemplateArr.Length - 1)
            {
                sb.Append("-");
            }
        }

        _animatedWordsText.text = sb.ToString();
    }

    public static void IncreasePoints(int points)
    {
        EnsureInitialized();

        if (_pointsText != null)
            _pointsText.text = points.ToString();
    }

    public static void SaveOldWordTemplateWordIndex(bool nextWord = true)
    {
        EnsureInitialized();

        if (_animatedWordsText == null) return;

        string text = _animatedWordsText.text;
        string currentWord = GameManager_Web.Instance.CurrentWordTemplate;
        var pos = Utils.FindFirstMatchingSubstring2(_animatedWordsText.text, currentWord + "-", 0, out string matchingWord);

        Debug.Log("match word: " + matchingWord);
        if (pos < 0)
            LastWordTemplateOffset = 0;
        else
        {
            LastWordTemplateOffset = pos;

            if (nextWord)
                LastWordTemplateOffset += matchingWord.Length;

        }
        //LastWordTemplateOffset = text.IndexOf(currentWord, StringComparison.Ordinal) + currentWord.Length;
        Debug.Log("Lastword template offset: " + LastWordTemplateOffset + ", current word: " + currentWord);

    }

    public static void UpdateWordsTemplate(string wordTemplate, bool disappearing = false)
    {
        EnsureInitialized();

        if (_animatedWordsText == null) return;

        _animatedWordsText.ForceMeshUpdate();

        string text = _animatedWordsText.text;
        TMP_TextInfo textInfo = _animatedWordsText.textInfo;

        //int startIndex = text.IndexOf(wordTemplate, LastWordTemplateOffset, StringComparison.Ordinal);
        int startIndex;
        string matchingWord = "";
        /*string matchingWord2 = "";
        var temp = "";

        if (_animatedWordsText.text.IndexOf('-') > -1)
        {
            temp = _animatedWordsText.text.Substring(0, _animatedWordsText.text.IndexOf('-'));

            //do the first letters match the last letters of the current word template?
            var i1 = Utils.FindFirstMatchingSubstring2(_animatedWordsText.text, temp, 0, out matchingWord);
            var i2 = Utils.FindFirstMatchingSubstring2(wordTemplate, temp, 0, out matchingWord2);
        }

        if (matchingWord != "" && matchingWord == matchingWord2)
            startIndex = 0;
        else
            startIndex = Utils.FindFirstMatchingSubstring2(_animatedWordsText.text, wordTemplate, LastWordTemplateOffset, out matchingWord);*/

        startIndex = Utils.FindFirstMatchingSubstring2(_animatedWordsText.text, wordTemplate, LastWordTemplateOffset, out matchingWord);

        if (startIndex == -1) return;

        int endIndex = startIndex + matchingWord.Length;

        //int endIndex = startIndex + wordTemplate.Length;

        // Cache Color32 to avoid struct allocation
        Color32 blueColor = Color.blue;

        for (int j = startIndex; j < endIndex; j++)
        {
            TMP_CharacterInfo chInfo = textInfo.characterInfo[j];

            int meshIndex = chInfo.materialReferenceIndex;
            int vertexIndex = chInfo.vertexIndex;

            Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

            // Assign cached color instead of creating new Color32
            colors[vertexIndex] = blueColor;
            colors[vertexIndex + 1] = blueColor;
            colors[vertexIndex + 2] = blueColor;
            colors[vertexIndex + 3] = blueColor;
        }

        // Apply changes
        _animatedWordsText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }


    private static Vector3 GetCharPosition(int index)
    {
        //Get the world position of the first character and check if its x coords are less than the left corner of 
        //the animated words template container
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
        TMP_TextInfo textInfo = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];

        int vIndex = charInfo.vertexIndex;
        int mIndex = charInfo.materialReferenceIndex;

        Vector3[] vertices = textInfo.meshInfo[mIndex].vertices;

        var width = vertices[vIndex + 2] - vertices[vIndex + 1];
        width.y = 0;

        // The center position of the first character.
        Vector3 charCenter =
        (vertices[vIndex + 0] +
        vertices[vIndex + 1] +
        vertices[vIndex + 2] +
        vertices[vIndex + 3]) / 4f;

        charCenter.y = 0;

        return _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().transform.TransformPoint(charCenter) - width / 2;
    }

    public static void UpdateAnimatedText2()
    {
        Debug.Log("in animated text 2");
        EnsureInitialized();
        Vector3 firstCharWorldPos = GetCharPosition(0);

        List<Vector3> charPositions = new List<Vector3>();
        for (int i = 1; i < _animatedWordsText.text.Length; i++)
        {
            charPositions.Add(GetCharPosition(i));
        }

        var xDif = charPositions[0] - firstCharWorldPos;

        _animatedWordsText.ForceMeshUpdate();

        _animatedWordsText.text = WordDisappearing(_animatedWordsText.text);


        float preferredWidth = _animatedWordsText.preferredWidth;
        _animatedWordsText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            preferredWidth
        );

        //shift the text container one letter to the left
        Vector3[] corners = new Vector3[4];
        _animatedWordsTemplate.GetComponent<RectTransform>().GetWorldCorners(corners);
        var newPos = corners[0] + Vector3.right * preferredWidth / 2;
        _animatedWordsText.transform.position = new Vector3(newPos.x, _animatedWordsText.transform.position.y, newPos.z) + Vector3.right * 50;
        //var textStartPos = (textcorners[0] + textcorners[1]) / 2f;
    }

    public static void UpdateAnimatedText1(string word, int charIndex)
    {
        EnsureInitialized();

        bool disappearing = false;

        if (_animatedWordsText == null) return;

        var text = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;

        text += word[charIndex];

        Vector3 firstCharWorldPos = GetCharPosition(0);

        Debug.Log("first char world pos: " + firstCharWorldPos + ", startPosX: " + _animatedWordsTemplateContainerStartPos.x);


        float preferredWidth = _animatedWordsText.preferredWidth;
        _animatedWordsText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            preferredWidth
        );


        if (firstCharWorldPos.x <= _animatedWordsTemplateContainerStartPos.x + 50.0f)
        {
            Debug.Log("disappear!");
            text = WordDisappearing(text);

        }

        _animatedWordsText.text = text;
        UpdateWordsTemplate(GameManager_Web.Instance.CurrentWordTemplate, disappearing);
    }


    private static string WordDisappearing(string text)
    {

        if (text.IndexOf(GameManager_Web.Instance.CurrentWordTemplate + "-") == 0 &&
            GameManager_Web.Instance.CurrentWordTemplate.Length > 0)
            GameManager_Web.Instance.CurrentWordTemplate = GameManager_Web.Instance.CurrentWordTemplate.Substring(1);

        text = text.Substring(1);
        //_animatedWordsText.text = text;

        if (text[0] == '-')
        {

            _onWordDisappeared?.Invoke(GameManager_Web.Instance.CurrentWordTemplate == "");
            Debug.Log("WORD TEMPLATE DISAPPEARED!");
        }
        else
        {
            SaveOldWordTemplateWordIndex(false);

        }
        return text;
    }
}
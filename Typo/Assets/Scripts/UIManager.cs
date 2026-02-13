using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using System;
using System.Linq;

public static class UIManager
{
    private static GameObject _points;
    private static GameObject _animatedWordsTemplate;
    private static Vector3 _animatedWordsTemplateContainerStartPos = Vector3.zero;

    private static string _oldWordTemplate = "";
    private static string _currentWordTemplate = "";

    public static int LastWordTemplateOffset { get; set; } = 0;

    public static event Action _onWordDisappeared;

    public static void Initialize(string[] wordsTemplateArr)
    {
        _points = GameObject.Find("RootGame/Points");
        _animatedWordsTemplate = GameObject.Find("RootGame/AnimatedWords");

        Vector3[] corners = new Vector3[4];
        _animatedWordsTemplate.transform.GetComponent<RectTransform>().GetWorldCorners(corners);
        _animatedWordsTemplateContainerStartPos = (corners[0] + corners[1]) / 2f;

        string fullText = "";

        //_animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().overrideColorTags = true;
        //GameManager.Instance.StartCoroutine(BuildWordsTemplate(wordsTemplate));
        foreach (var word in wordsTemplateArr)
        {
            //_animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text += word;
            fullText += word;

            if (Array.IndexOf(wordsTemplateArr, word) < wordsTemplateArr.Length - 1)
            {
                //_animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text += "-";
                fullText += "-";

            }
        }
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = fullText;
        //GameManager.Instance.WordsTemplate


        //_animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text += "aloha";
        //_animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text += "lola";
    }

    public static void IncreasePoints(int points)
    {
        _points.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = points.ToString();
    }

    public static void SaveOldWordTemplateWordIndex()
    {
        var text = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        LastWordTemplateOffset = text.IndexOf(GameManager.Instance.CurrentWordTemplate) + GameManager.Instance.CurrentWordTemplate.Length;
    }

    public static void UpdateWordsTemplate(string wordTemplate)
    {
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
        var text = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        TMP_TextInfo textInfo = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().textInfo;

        int i = text.IndexOf(wordTemplate, LastWordTemplateOffset);

        var start = i;
        var end = i + wordTemplate.Length;

        for (int j = start; j < end; j++)
        {
            TMP_CharacterInfo chInfo = textInfo.characterInfo[j];

            int meshIndex = chInfo.materialReferenceIndex;
            int vertexIndex = chInfo.vertexIndex;

            Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

            colors[vertexIndex + 0] = Color.blue;
            colors[vertexIndex + 1] = Color.blue;
            colors[vertexIndex + 2] = Color.blue;
            colors[vertexIndex + 3] = Color.blue;

        }
        // Apply changes
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }


    public static void UpdateAnimatedText(string wordTemplate, int index)
    {
        //Get the world position of the first character and check if its x coords are less than the left corner of 
        //the animated words template container
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
        TMP_TextInfo textInfo = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[0];

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

        Vector3 firstCharWorldPos = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().transform.TransformPoint(charCenter) - width / 2;

        var text = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;

        text += wordTemplate[index];

        string matchingWord = "";
        int i = Utils.FindFirstMatchingSubstring(text, GameManager.Instance.CurrentWordTemplate, out matchingWord);
        if (firstCharWorldPos.x <= _animatedWordsTemplateContainerStartPos.x + 50.0f)
        {
            text = text.Substring(1);
            //The current word template is not visible anymore in the animated text, so update it with the next word in animated text
            if (text[0] == '-' && matchingWord != GameManager.Instance.CurrentWordTemplate)
            {
                _onWordDisappeared?.Invoke();
                Debug.Log("WORD TEMPLATE DISAPPEARED!");
            }

        }

        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
        textInfo = _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().textInfo;

        matchingWord = "";
        //Display the current template word in green once it is displayed completely
        //int i = text.IndexOf(GameManager.Instance.CurrentWordTemplate);

        //Display all remaining characters of the word template in green
        i = Utils.FindFirstMatchingSubstring(text, GameManager.Instance.CurrentWordTemplate, out matchingWord);

        if (i > -1)
        {
            Debug.Log("mark current word template!");
            var start = i;
            var end = i + matchingWord.Length;

            for (int j = start; j < end; j++)
            {
                TMP_CharacterInfo chInfo = textInfo.characterInfo[j];

                int meshIndex = chInfo.materialReferenceIndex;
                int vertexIndex = chInfo.vertexIndex;

                Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

                colors[vertexIndex + 0] = Color.blue;
                colors[vertexIndex + 1] = Color.blue;
                colors[vertexIndex + 2] = Color.blue;
                colors[vertexIndex + 3] = Color.blue;

            }
            // Apply changes
            _animatedWordsTemplate.transform.Find("Text").GetComponent<TextMeshProUGUI>().UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }

}
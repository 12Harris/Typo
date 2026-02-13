using UnityEngine;
using UnityEngine.UI;
using System;
public static class Utils
{

    public static Vector3 UIToWorldPosition(RectTransform uiElement, Vector3 worldPos, Canvas canvas)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiElement.parent as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );
        return localPos;
    }

    public static void MoveUIToWorldPosition(RectTransform uiElement, Vector3 worldPos, Canvas canvas)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiElement.parent as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );
        uiElement.anchoredPosition = localPos;
    }


    public static int FindFirstMatchingSubstring2(
    string searchText,
    string word,
    int startPos,
    out string matchingWord)
    {
        matchingWord = string.Empty;

        if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(word))
            return -1;

        ReadOnlySpan<char> searchSpan = searchText.AsSpan();
        ReadOnlySpan<char> wordSpan = word.AsSpan();

        int bestIndex = -1;
        int bestLength = 0;

        Debug.Log("STARTPOS: " + startPos);

        // Check prefixes (word start)
        for (int len = 1; len <= wordSpan.Length; len++)
        {
            //int index = searchSpan.IndexOf(wordSpan.Slice(0, len));
            Debug.Log("slice: " + searchSpan.Slice(startPos, searchSpan.Length - startPos).ToString());

            int index = searchSpan.Slice(startPos, searchSpan.Length - startPos).IndexOf(wordSpan.Slice(0, len));

            if (index >= 0)
            {
                bestIndex = index;
                bestLength = len;
            }
        }

        // If nothing found yet, check full word directly
        if (bestIndex == -1)
        {
            //int index = searchSpan.IndexOf(wordSpan);
            int index = searchSpan.Slice(startPos, searchSpan.Length - startPos).IndexOf(wordSpan);
            if (index >= 0)
            {
                matchingWord = word;
                return index;
            }
            return -1;
        }

        // Check suffixes (word end)
        for (int start = 1; start < wordSpan.Length; start++)
        {
            //int index = searchSpan.IndexOf(wordSpan.Slice(start));
            int index = searchSpan.Slice(startPos, searchSpan.Length - startPos).IndexOf(wordSpan.Slice(start));
            if (index >= 0 && (bestIndex == -1 || index < bestIndex))
            {
                bestIndex = index;
                bestLength = wordSpan.Length - start;
                Debug.Log("Best Index: " + bestIndex);
            }
            else
            {
                break;
            }
        }

        matchingWord = word.Substring(
            wordSpan.Length - bestLength,
            bestLength);

        return bestIndex + startPos;
    }

    public static int FindFirstMatchingSubstring(string searchText, string word, out string matchingWord)
    {
        //Did we find the whole word?
        int foundIndex1 = -1;
        int foundIndex2 = -1;
        string matchingWord1 = "", matchingWord2 = "";
        matchingWord = "";

        if (searchText == "")
            return -1;

        //check if the first letters in the searchText match the first letters in the word
        for (int i = 0; i < word.Length; i++)
        {
            var result = searchText.IndexOf(word.Substring(0, i + 1));
            if (result != -1)
            {
                foundIndex1 = result;
                matchingWord1 = word.Substring(0, i + 1);
            }
        }

        //check if the whole word is found
        if (foundIndex1 == -1)
        {
            foundIndex1 = searchText.IndexOf(word);
            matchingWord1 = word;
        }

        //check if the first letters in the searchText match the last letters in the word
        for (int i = 0; i < word.Length && foundIndex2 == -1; i++)
        {
            var result = searchText.IndexOf(word.Substring(i + 1));
            if (result != -1)
            {
                foundIndex2 = result;
                matchingWord2 = word.Substring(i + 1);
            }
            else
            {
                break;
            }
        }

        if (foundIndex2 > -1 && foundIndex2 < foundIndex1)
        {
            matchingWord = matchingWord2;
            return foundIndex2;
        }

        matchingWord = matchingWord1;
        return foundIndex1;

    }
}
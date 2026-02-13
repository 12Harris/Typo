using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

public class InputScreen : MonoBehaviour
{
    private TextMeshProUGUI _letters;

    private List<Vector3> _letterPositions = new List<Vector3>();

    private Vector3[] _corners = new Vector3[4];
    private Vector3 _cursorOffset = Vector3.zero;

    private Vector3 _inputFieldStart = Vector3.zero;

    [SerializeField]
    private Cursor _cursor;

    private string full_text = "hello how are you all doing today what a lovely day it is bike car bridge brb gtg cya u2 4ever";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _letters = GetComponent<TextMeshProUGUI>();
        // corners:
        // 0 = bottom-left
        // 1 = top-left
        // 2 = top-right
        // 3 = bottom-right
        transform.parent.GetComponent<RectTransform>().GetWorldCorners(_corners);
        _inputFieldStart = (_corners[0] + _corners[1]) / 2f;
        Vector3[] cursorWorldCorners = new Vector3[4];
        _cursor.gameObject.GetComponent<RectTransform>().GetWorldCorners(cursorWorldCorners);
        var cursorLeft = (cursorWorldCorners[0] + cursorWorldCorners[1]) / 2f;
        _cursorOffset = cursorLeft - _inputFieldStart;

    }

    void UpdateCharacterPositions(bool add, bool remove)
    {
        _letters.ForceMeshUpdate();
        TMP_TextInfo textInfo = _letters.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[textInfo.characterCount - 1];

        int vIndex = charInfo.vertexIndex;
        int mIndex = charInfo.materialReferenceIndex;

        Vector3[] vertices = textInfo.meshInfo[mIndex].vertices;

        var width = vertices[vIndex + 2] - vertices[vIndex + 1];
        width.y = 0;

        // The center position of the character.
        Vector3 charCenter =
        (vertices[vIndex + 0] +
        vertices[vIndex + 1] +
        vertices[vIndex + 2] +
        vertices[vIndex + 3]) / 4f;

        charCenter.y = 0;

        Vector3 worldPos = _letters.transform.TransformPoint(charCenter) + width / 2;

        if (add)
        {
            _letterPositions.Add(worldPos);
        }
        else if (remove)
        {
            if (_letterPositions.Count > 0)
                _letterPositions.RemoveAt(_letterPositions.Count - 1);
        }
        else
        {
            _letterPositions[_letterPositions.Count - 1] = worldPos;
        }

    }

    public void UpdateCursor(bool add, bool remove)
    {
        if (_letters.text.Length > 0)
            UpdateCharacterPositions(add, remove);
        if (_letters.text.Length == 0)
            _cursor.Reset();
        else
            _cursor.SetPosition(_letterPositions[_letterPositions.Count - 1]);
    }

    public void ResetText()
    {
        _letters.text = "";
        _letterPositions.Clear();
        _cursor.Reset();
    }
}

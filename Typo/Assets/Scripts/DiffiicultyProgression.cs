using UnityEngine;

[CreateAssetMenu(fileName = "DiffiicultyProgression", menuName = "Scriptable Objects/DifficultyProgression")]
public class DifficultyProgression : ScriptableObject
{
    public int _tier;
    public int _minWords;
    public int _maxWords;
    public float _speed;
    public int _wordLengthMin;
    public int _wordLengthMax;
    public float _spawnRate;
}

using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MoodSlider)), CanEditMultipleObjects]
public class MoodSliderEditor : Editor
{
    private MoodSlider _base;

    public override void OnInspectorGUI()
    {
        _base = (MoodSlider)target;
        DrawDefaultInspector();
        if(_base.characterStats == null)
            return;
        string[] _allMoodsNames = GetAllMoodsNames(_base.characterStats);
        _base.moodToDisplayIndex = EditorGUILayout.Popup("Mood to display", _base.moodToDisplayIndex, _allMoodsNames);
        _base.moodToDisplay = _allMoodsNames[_base.moodToDisplayIndex];
    }

    private string[] GetAllMoodsNames(CharacterStats _characterStats)
    {
        CharacterStats.Mood[] _allMoods = _characterStats.moods;
        List<string> _moodNames = new List<string>();
        for (int i = 0; i < _allMoods.Length; i++)
        {
            _moodNames.Add(_allMoods[i].nameInInk);
        }

        return _moodNames.ToArray();
    }
}

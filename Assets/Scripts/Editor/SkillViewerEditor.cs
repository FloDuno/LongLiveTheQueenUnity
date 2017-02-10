using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;

[CustomEditor(typeof(SkillViewer)), CanEditMultipleObjects]
public class SkillViewerEditor : Editor
{
    private SkillViewer _base;

    public override void OnInspectorGUI()
    {
        _base = (SkillViewer)target;
        DrawDefaultInspector();
        if (_base.characterStats == null)
            return;

        string[] _allSkillsTypes = GetAllStatTypes(_base.characterStats);
        _base.skillToViewIndex = EditorGUILayout.Popup("SkillGroup", _base.skillToViewIndex, _allSkillsTypes);
        _base.skillToView = _allSkillsTypes[_base.skillToViewIndex];
        _base.myStatGroups = GetAllCorrespondingGroups(_base.skillToView);
    }

    private CharacterStats.StatGroup[] GetAllCorrespondingGroups(string _skillToView)
    {
        List<CharacterStats.StatGroup> _statGroups = new List<CharacterStats.StatGroup>();
        for (int i = 0; i < _base.characterStats.statGroups.Length; i++)
        {
            if(_base.characterStats.statGroups[i].type == _skillToView)
               _statGroups.Add(_base.characterStats.statGroups[i]);
        }

        return _statGroups.ToArray();
    }

    private string[] GetAllStatTypes(CharacterStats _characterStats)
    {
        List<string> _allStatsTypes = new List<string>();
        for (int i = 0; i < _characterStats.typesStat.Length; i++)
        {
            _allStatsTypes.Add(_characterStats.typesStat[i]);
        }

        return _allStatsTypes.ToArray();
    }
}
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterStats))]
public class CharacterStatsEditor : Editor
{
    private CharacterStats _base;
    private int _lessonsNumber;
    public bool showLessons;
    public bool[] showIndividualLesson;
    public bool[] showSpecialities;

    public override void OnInspectorGUI()
    {
        //Same as usual
        _base = (CharacterStats)target;
        DrawDefaultInspector();

        showLessons = EditorGUILayout.Foldout(showLessons, "Show statGroups");
        if (!showLessons)
            return;

        //Fake array system
        EditorGUI.indentLevel++;
        _lessonsNumber = _base.statGroups.Length;
        _lessonsNumber = EditorGUILayout.IntField("Number of statGroups", _lessonsNumber);
        if (_lessonsNumber > 0)
        {
            Array.Resize(ref _base.statGroups, _lessonsNumber);
            Array.Resize(ref showSpecialities, _lessonsNumber);
            Array.Resize(ref showIndividualLesson, _lessonsNumber);
        }
        //Display every stat and substat
        for (int i = 0; i < _base.statGroups.Length; i++)
        {
            //Prevent empty error
            if (_base.statGroups[i] == null)
                _base.statGroups[i] = new CharacterStats.StatGroup();
            //Show statGroup
            showIndividualLesson[i] = EditorGUILayout.Foldout(showIndividualLesson[i], _base.statGroups[i].name);
            if (showIndividualLesson[i])
            {
                _base.statGroups[i].typeIndex = EditorGUILayout.Popup(
                    "StatGroup type",
                    _base.statGroups[i].typeIndex,
                    _base.typesStat);
                _base.statGroups[i].type = _base.typesStat[_base.statGroups[i].typeIndex];
                if (_base.statGroups[i] == null)
                    _base.statGroups[i] = new CharacterStats.StatGroup();
                _base.statGroups[i].name = EditorGUILayout.TextField("StatGroup name", _base.statGroups[i].name);
                int _speCount = _base.statGroups[i].stats.Length;
                _speCount = EditorGUILayout.IntField("Number of stats", _speCount);
                _speCount = Mathf.Clamp(_speCount, 1, int.MaxValue);
                Array.Resize(ref _base.statGroups[i].stats, _speCount);
                EditorGUI.indentLevel++;
                showSpecialities[i] = EditorGUILayout.Foldout(showSpecialities[i], _base.statGroups[i].name + " stats");
                //Show stat
                if (showSpecialities[i])
                {
                    for (int j = 0; j < _base.statGroups[i].stats.Length; j++)
                    {
                        if (_base.statGroups[i].stats[j] == null)
                            _base.statGroups[i].stats[j] = new CharacterStats.Stat();
                        //Make it one line for asthetic purposes
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Stat name", GUILayout.MaxWidth(100));
                        _base.statGroups[i].stats[j].name =
                            EditorGUILayout.TextField(_base.statGroups[i].stats[j].name);
                        GUIStyle _alignRight = new GUIStyle {alignment = TextAnchor.MiddleRight};
                        EditorGUILayout.LabelField("Default value", _alignRight, GUILayout.Width(100));
                        _base.statGroups[i].stats[j].initialValue =
                            EditorGUILayout.IntField(_base.statGroups[i].stats[j].initialValue);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorUtility.SetDirty(_base);
    }
}
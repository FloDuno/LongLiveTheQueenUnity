using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Welcome to custom editor hell (I mean it's not THAT bad but still)
[CustomEditor(typeof(ActivityManager))]
public class ActivityManagerEditor : Editor
{
    private ActivityManager _base;

    //Foldouts (aka bool as arrow)
    private bool _showLessons;

    private bool[] _showLesson;
    private bool[] _showOptions;

    public int totalLessonNumber;

    public override void OnInspectorGUI()
    {
        //Basic stuff
        _base = (ActivityManager)target;
        DrawDefaultInspector();
        if (_base.activitiesEqualStats)
        {
            AutoFillClasses();
        }
        else
        {
            //Shit just got real
            //Make a "fake array style" editor
            totalLessonNumber = EditorGUILayout.IntField("Number of lessons", totalLessonNumber);
            totalLessonNumber = Mathf.Clamp(totalLessonNumber, 1, int.MaxValue);
            //Array.Resize = my savior (Heavy ram eater but we don't care since it's not even real time based)
            Array.Resize(ref _base.allLessons, totalLessonNumber);
            Array.Resize(ref _showLesson, totalLessonNumber);
            Array.Resize(ref _showOptions, totalLessonNumber);
            _showLessons = EditorGUILayout.Foldout(_showLessons, "Display lessons");
            if (_showLessons)
                DisplayLessons();
        }
    }

    private void DisplayLessons()
    {
        string[] _allStatType = GetAllStatsType(_base.characterStats);
        //Display every possible lesson and every option for every option
        for (int i = 0; i < _base.allLessons.Length; i++)
        {
            EditorGUI.indentLevel++; //Make it prettier and easier to read
            _showLesson[i] = EditorGUILayout.Foldout(_showLesson[i], "Show " + _base.allLessons[i].name + " params");
            if (_showLesson[i])
            {
                //Display options
                _base.allLessons[i].typeIndex = EditorGUILayout.Popup("Lesson type",_base.allLessons[i].typeIndex, _allStatType);
                _base.allLessons[i].type = _base.characterStats.typesStat[_base.allLessons[i].typeIndex];
                _base.allLessons[i].name = EditorGUILayout.TextField("Lesson name", _base.allLessons[i].name);
                EditorGUI.indentLevel++;
                _showOptions[i] = EditorGUILayout.Foldout(_showOptions[i], "Display options");
                if (_showOptions[i])
                {
                    DisplayLessonOptions(_base.allLessons[i]);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
    }

    private string[] GetAllStatsType(CharacterStats _characterStats)
    {
        List<string> _statTypes = new List<string>();
        for (int i = 0; i < _characterStats.typesStat.Length; i++)
        {
            _statTypes.Add(_characterStats.typesStat[i]);
        }

        return _statTypes.ToArray();
    }

    private void DisplayLessonOptions(ActivityManager.Lesson _lesson)
    {
        //Make sure there's at least one option by lesson (still thinking about how to make it optionnal)
        if (_lesson.lessonOptions.Length == 0)
            _lesson.lessonOptions = new ActivityManager.Lesson.LessonOption[1];
        //Get all stat to modify
        string[] _allCharacStats = GetAllStats(_base.characterStats);
        //Setup number of options for the current lesson
        int _numberOfOptions = _lesson.lessonOptions.Length;
        _numberOfOptions = EditorGUILayout.IntField("Number of options", _numberOfOptions);
        _numberOfOptions = Mathf.Clamp(_numberOfOptions, 1, int.MaxValue);
        Array.Resize(ref _lesson.lessonOptions, _numberOfOptions);
        EditorGUILayout.Space();
        //Display the options
        for (int i = 0; i < _lesson.lessonOptions.Length; i++)
        {
            EditorGUI.indentLevel++;
            //If the option doesn't exist, setup it
            if (_lesson.lessonOptions[i].valueToChange == null)
            {
                _lesson.lessonOptions[i].valueToChange = new string[1];
                _lesson.lessonOptions[i].valueToAdd = new int[1];
                _lesson.lessonOptions[i].valueToChangeIndex = new int[1];
            }
            //Name
            _lesson.lessonOptions[i].name = EditorGUILayout.TextField("Option name", _lesson.lessonOptions[i].name);
            //How many stats does this option modify
            int _numberOfModifications = _lesson.lessonOptions[i].valueToAdd.Length;
            _numberOfModifications = EditorGUILayout.IntField("Number of modifications", _numberOfModifications);
            _numberOfModifications = Mathf.Clamp(_numberOfModifications, 1, int.MaxValue);
            Array.Resize(ref _lesson.lessonOptions[i].valueToAdd, _numberOfModifications);
            Array.Resize(ref _lesson.lessonOptions[i].valueToChange, _numberOfModifications);
            Array.Resize(ref _lesson.lessonOptions[i].valueToChangeIndex, _numberOfModifications);
            //Ask for the values to modify and how many to change
            for (int j = 0; j < _numberOfModifications; j++)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                _lesson.lessonOptions[i].valueToChangeIndex[j] = EditorGUILayout.Popup(
                    _lesson.lessonOptions[i].valueToChangeIndex[j],
                    _allCharacStats);
                _lesson.lessonOptions[i].valueToChange[j] =
                    _allCharacStats[_lesson.lessonOptions[i].valueToChangeIndex[j]];
                _lesson.lessonOptions[i].valueToAdd[j] = EditorGUILayout.IntField(
                    "Value to add",
                    _lesson.lessonOptions[i].valueToAdd[j]);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        EditorUtility.SetDirty(_base);
    }

    //Pack nested arrays in only one
    private string[] GetAllStats(CharacterStats _characterStats)
    {
        List<string> _allStats = new List<string>();
        for (int i = 0; i < _characterStats.statGroups.Length; i++)
        {
            for (int j = 0; j < _characterStats.statGroups[i].stats.Length; j++)
            {
                _allStats.Add(_characterStats.statGroups[i].stats[j].name);
            }
        }

        return _allStats.ToArray();
    }

    //Take all stats and change it to classic lessons (name are the same, increase by one)
    private void AutoFillClasses()
    {
        _base.allLessons = new ActivityManager.Lesson[_base.characterStats.statGroups.Length];
        for (int i = 0; i < _base.allLessons.Length; i++)
        {
            _base.allLessons[i] = new ActivityManager.Lesson
            {
                name = _base.characterStats.statGroups[i].name,
                type = _base.characterStats.statGroups[i].type,
                lessonOptions = new ActivityManager.Lesson.LessonOption[_base.characterStats.statGroups[i].stats.Length]
            };
            for (int j = 0; j < _base.characterStats.statGroups[i].stats.Length; j++)
            {
                _base.allLessons[i].lessonOptions[j].name = _base.characterStats.statGroups[i].stats[j].name;
                _base.allLessons[i].lessonOptions[j].valueToAdd = new[] {1};
                _base.allLessons[i].lessonOptions[j].valueToChangeIndex = new[] {0};
                _base.allLessons[i].lessonOptions[j].valueToChange =
                    new[] {_base.characterStats.statGroups[i].stats[j].name};
            }
        }
    }
}
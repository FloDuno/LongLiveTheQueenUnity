using UnityEditor;

[CustomEditor(typeof(LessonTypeContainer)), CanEditMultipleObjects]
public class LessonTypeContainerEditor : Editor
{
    private LessonTypeContainer _base;

    public override void OnInspectorGUI()
    {
        _base = (LessonTypeContainer)target;
        DrawDefaultInspector();

        if(_base.weekManager == null || _base.characterStats == null || _base.specialities)
            return;
        _base.lessonTypeIndex = EditorGUILayout.Popup(
            "StatGroup Type",
            _base.lessonTypeIndex,
            _base.characterStats.typesStat);
        _base.lessonType = _base.characterStats.typesStat[_base.lessonTypeIndex];
    }
}

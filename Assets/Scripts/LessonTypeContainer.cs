using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Parent of lessons buttons. Will spawn them depending on their type
public class LessonTypeContainer : MonoBehaviour
{
    public bool specialities;
    public ActivityManager weekManager;
    public CharacterStats characterStats;

    [SerializeField] private GameObject _togglePrefab;

    //Need to have this in order to be able to select only one lesson by block
    [SerializeField] private int _dayGroup;

    //Index necesary for the "popup" view
    [HideInInspector] public int lessonTypeIndex;

    [HideInInspector] public string lessonType;

    private void Start()
    {
        //Find how much button we need to spawn (nothing if spe)
        int _numberOfLessonOfThisType = 0;
        List<ActivityManager.Lesson> _lessons = new List<ActivityManager.Lesson>();
        if (!specialities)
        {
            for (int i = 0; i < weekManager.allLessons.Length; i++)
            {
                if (weekManager.allLessons[i].type == lessonType)
                {
                    _numberOfLessonOfThisType++;
                    _lessons.Add(weekManager.allLessons[i]);
                }
            }
        }

        EventManager.ChangeStatEvent += ResetButtons;

        //Spawn buttons
        for (int i = 0; i < _numberOfLessonOfThisType; i++)
        {
            GameObject _newToggle = Instantiate(_togglePrefab, transform);
            _newToggle.transform.localScale = Vector3.one;
            _newToggle.GetComponent<ActivityToggle>().groupOfActivities =
                specialities ? 2 * _dayGroup + 1 : 2 * _dayGroup;
            _newToggle.GetComponent<ActivityToggle>().isSpeciality = specialities;
            _newToggle.GetComponent<ActivityToggle>().lesson = _lessons[i];
            _newToggle.GetComponentInChildren<Text>().text = _lessons[i].name;
        }
    }

    private void ResetButtons(CharacterStats _characterstats)
    {
        if (specialities)
        {
            foreach (Transform _child in transform)
            {
                Destroy(_child.gameObject);
            }
        }
        else
        {


            Toggle[] _childrenLessons = GetComponentsInChildren<Toggle>();
            for (int i = 0; i < _childrenLessons.Length; i++)
            {
                _childrenLessons[i].isOn = false;
            }
        }
    }
}
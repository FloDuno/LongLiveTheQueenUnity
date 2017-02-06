using System;
using UnityEngine;
using UnityEngine.UI;

//Kinda messy since I wanted it to handle lessons and options at the same time
[RequireComponent(typeof(Toggle))]
public class ActivityToggle : MonoBehaviour
{
    //HideInInspector since it's passed through script
    [HideInInspector] public Toggle myToggle;

    [HideInInspector, Range(1, 100)] public int groupOfActivities;
    [HideInInspector] public ActivityManager.Lesson lesson;
    [HideInInspector] public ActivityManager.Lesson.LessonOption lessonOption;
    [HideInInspector] public bool isSpeciality;

    // Use this for initialization
    private void Start()
    {
        myToggle = GetComponent<Toggle>();
        //Do something when clicked
        if (isSpeciality)
            myToggle.onValueChanged.AddListener(ChangeActivitySpe);
        else
        {
            myToggle.onValueChanged.AddListener(ChangeActivityLesson);
        }
        myToggle.isOn = false;
    }

    private void ChangeActivitySpe(bool _arg0)
    {
        //Make sure only one option has been selected and hook event
        if (_arg0)
        {
            if (EventManager.optionsOfTheDay.Length < groupOfActivities)
                Array.Resize(ref EventManager.optionsOfTheDay, groupOfActivities);
            if (EventManager.optionsOfTheDay[groupOfActivities - 1] != null)
                EventManager.optionsOfTheDay[groupOfActivities - 1].myToggle.isOn = false;
            EventManager.optionsOfTheDay[groupOfActivities - 1] = this;
            EventManager.ChangeStatEvent += lessonOption.Launch;
        }
        //Else unhook
        else
        {
            EventManager.ChangeStatEvent -= lessonOption.Launch;
        }
    }

    private void ChangeActivityLesson(bool _arg0)
    {
        //Make sure only one option has been selected and instantiate options
        if (_arg0)
        {
            if (EventManager.activitiesOfTheDay.Length < groupOfActivities)
                Array.Resize(ref EventManager.activitiesOfTheDay, groupOfActivities);
            if (EventManager.activitiesOfTheDay[groupOfActivities - 1] != null)
                EventManager.activitiesOfTheDay[groupOfActivities - 1].myToggle.isOn = false;
            EventManager.activitiesOfTheDay[groupOfActivities - 1] = this;

            MakeSpecialitiesAppear();
        }
    }

    private void MakeSpecialitiesAppear()
    {
        //Get the container, destroy children and instantiate new ones
        Transform _speParents = GetSpeParent();
        foreach (Transform _child in _speParents)
        {
            Destroy(_child.gameObject);
        }
        for (int i = 0; i < lesson.lessonOptions.Length; i++)
        {
            GameObject _instanciatedSpe = Instantiate(gameObject, _speParents);
            _instanciatedSpe.transform.localScale = Vector3.one;
            _instanciatedSpe.GetComponent<ActivityToggle>().isSpeciality = true;
            _instanciatedSpe.GetComponent<ActivityToggle>().groupOfActivities++;
            _instanciatedSpe.GetComponent<ActivityToggle>().lessonOption = lesson.lessonOptions[i];
            _instanciatedSpe.GetComponentInChildren<Text>().text = lesson.lessonOptions[i].name;
        }
    }

    private Transform GetSpeParent()
    {
        Transform _allRowsParent = transform.parent.parent;
        for (int i = 0; i < _allRowsParent.childCount; i++)
        {
            if (_allRowsParent.GetChild(i).CompareTag("Specialities"))
                return _allRowsParent.GetChild(i);
        }

        throw new NotSupportedException();
    }
}
using UnityEngine;

//I'm still trying to figure out how to use it efficiently
[CreateAssetMenu(menuName = "Managers/WeekManager")]
public class WeekManager : ScriptableObject {

    public ActivityManager activityManagerWeek,activityManagerWeekEnd,randomActivity;
    public int numberOfDays;

}

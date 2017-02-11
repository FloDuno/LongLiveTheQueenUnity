using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Can make an asset easily
[CreateAssetMenu(menuName = "Managers/ActivityManager")]
//This class is here to regroup possible lessons so you can make different managers if the week end lessons aren't the same as normal lessons
public class ActivityManager : ScriptableObject
{
    //Handled by custom editor
    [HideInInspector] public Lesson[] allLessons;

    //Automatic lessons (every lesson increase a stat by one and has the same name as the stat)
    public bool activitiesEqualStats;

    //Reference to all stats
    public CharacterStats characterStats;

    //Every lesson is made of a name, a group/type and some options
    //Every option has a name and can modify one or more value
    [Serializable]
    public class Lesson
    {
        public string name;
        public string type;
        public int typeIndex;

        [Serializable]
        public struct LessonOption
        {
            public string[] valueToChange;
            [HideInInspector] public int[] valueToChangeIndex;
            public int[] valueToAdd;
            public string name;

            //The delegate used to increase stats
            //Get all needed value from stats and apply increase/decrease
            public void Launch(CharacterStats _characterStats)
            {
                List<string> _valuesToChange =
                    valueToChange
                        .ToList(); //Use a List instead of an Array because Contains is so much straight forward compared to IndexOf
                for (int i = 0; i < _characterStats.statGroups.Length; i++)
                {
                    for (int j = 0; j < _characterStats.statGroups[i].stats.Length; j++)
                    {
                        if (_valuesToChange.Contains(
                            _characterStats
                                .statGroups[i]
                                .stats[j]
                                .name)) //If one of the stat is the stat we want to change, change it modafuka
                        {
                            _characterStats.statGroups[i].stats[j].value += valueToAdd[_valuesToChange.FindIndex(
                                x => x == _characterStats.statGroups[i].stats[j].name)];
                        }
                    }
                }
            }
        }

        public LessonOption[] lessonOptions = new LessonOption[1];
    }
}
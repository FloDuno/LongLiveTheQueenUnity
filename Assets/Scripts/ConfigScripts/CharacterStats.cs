using UnityEngine;

[CreateAssetMenu(menuName = "Managers/CharacterStats")]
public class CharacterStats : ScriptableObject
{

    public string[] typesStat;
    public Mood[] moods;
    [HideInInspector] public StatGroup[] statGroups = new StatGroup[1];

    [System.Serializable]
    public class Mood
    {
        public string nameInInk;

        public string minName;
        public string maxName;
        [Range(-5,5)]
        public int intialValue;
        [HideInInspector]
        public int value;
    }

    [System.Serializable]
    public class StatGroup
    {
        public string type;
        public int typeIndex;
        public string name;
        public Stat[] stats = new Stat[1];

        public StatGroup()
        {
            name = "default";
        }
    }

    public void ResetStat()
    {
        for (int i = 0; i < statGroups.Length; i++)
        {
            for (int j = 0; j < statGroups[i].stats.Length; j++)
            {
                statGroups[i].stats[j].value = statGroups[i].stats[j].initialValue;
            }
        }
    }

    [System.Serializable]
    public class Stat
    {
        public string name;
        public int initialValue;
        public int value;

        public Stat()
        {
            name = "default";
            value = 0;
        }
    }
}

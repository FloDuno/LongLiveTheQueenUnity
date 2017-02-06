using UnityEngine;

[CreateAssetMenu(menuName = "Managers/CharacterStats")]
public class CharacterStats : ScriptableObject
{

    public string[] typesStat;
    [HideInInspector] public StatGroup[] statGroups = new StatGroup[1];

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

    [System.Serializable]
    public class Stat
    {
        public string name;
        public int value;

        public Stat()
        {
            name = "default";
            value = 0;
        }
    }
}

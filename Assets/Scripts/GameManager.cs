using UnityEngine;
using Ink.Runtime;

//Singleton
public class GameManager : MonoBehaviour
{
    //Debug purpose
    public int elegance;

    [HideInInspector] public int dayNumber;
    public CharacterStats characterStats;
    private static GameManager _instance;

    // Set this file to your compiled json asset
    public TextAsset inkAsset;

    // The ink story that we're wrapping
    private Story _inkStory;

    // Game Instance Singleton
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStats();
        EventManager.ChangeStatEvent += DisplayTextOfTheDay;
        _inkStory = new Story(inkAsset.text);
        _inkStory.Continue();
        dayNumber = 1;
    }

    private void DisplayTextOfTheDay(CharacterStats _characterStats)
    {
        _inkStory.ChoosePathString("Day"+dayNumber);
        print(_inkStory.ContinueMaximally());
        dayNumber++;
    }

    private void ResetStats()
    {
        for (int i = 0; i < characterStats.statGroups.Length; i++)
        {
            for (int j = 0; j < characterStats.statGroups[i].stats.Length; j++)
            {
                characterStats.statGroups[i].stats[j].value = 0;
            }
        }
    }

    private void Update()
    {
        elegance = characterStats.statGroups[0].stats[1].value;
    }
}
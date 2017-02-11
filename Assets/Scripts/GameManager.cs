using UnityEngine;
using UnityEngine.UI;

//Singleton
public class GameManager : MonoBehaviour
{
    //Check if we finished the game
    [HideInInspector] public int dayNumber;

    public CharacterStats characterStats;

    [SerializeField] private ContentManager _contentManager;
    public GameObject menu;

    [SerializeField] private int _maxDays;
    private static GameManager _instance;

    //We can launch the event only if lessons has been selected
    public bool CanEndDay
    {
        get
        {
            Toggle[] _allToggles = FindObjectsOfType<Toggle>();
            int _clickedToggle = 0;
            for (int i = 0; i < _allToggles.Length; i++)
            {
                if (_allToggles[i].isOn)
                    _clickedToggle++;
            }

            return _clickedToggle >= 4; //Lesson + option
        }
    }

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
        characterStats.ResetCharacterValues();
        EventManager.ChangeStatEvent += DisplayTextOfTheDay;
        dayNumber = 1;
    }

    //Hide menu and display text
    private void DisplayTextOfTheDay(CharacterStats _characterStats)
    {
        _contentManager.gameObject.SetActive(true);
        if (dayNumber < _maxDays)
        {
            _contentManager.ChooseDialog(dayNumber);
            dayNumber++;
        }
        else
        {
            _contentManager.GoToEnd();
        }
        menu.SetActive(false);
    }
}
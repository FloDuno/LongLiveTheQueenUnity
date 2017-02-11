using UnityEngine;
using UnityEngine.UI;

//Singleton
public class GameManager : MonoBehaviour
{
    [HideInInspector] public int dayNumber;

    public CharacterStats characterStats;

    [SerializeField] private ContentManager _contentManager;
    public GameObject menu;

    [SerializeField] private int _maxDays;
    private static GameManager _instance;


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

    private void Update()
    {
        //        print(characterStats.moods[0].value);
    }
}
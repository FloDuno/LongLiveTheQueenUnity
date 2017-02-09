using UnityEngine;

//Singleton
public class GameManager : MonoBehaviour
{

    [HideInInspector] public int dayNumber;
    public CharacterStats characterStats;

    [SerializeField] private ContentManager _contentManager;
    public GameObject menu;

    private static GameManager _instance;

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
        characterStats.ResetStat();
        EventManager.ChangeStatEvent += DisplayTextOfTheDay;
        dayNumber = 1;
    }

    private void DisplayTextOfTheDay(CharacterStats _characterStats)
    {
        _contentManager.gameObject.SetActive(true);
        _contentManager.ChooseDialog(dayNumber);
        dayNumber++;
        menu.SetActive(false);
    }
}
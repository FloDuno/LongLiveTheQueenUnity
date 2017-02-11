using UnityEngine;
using UnityEngine.UI;

//Like for the skills, slider are better feedbacks
//Get the right mood reference and display it, nothing awesome
public class MoodSlider : MonoBehaviour
{
    public CharacterStats characterStats;
    private Slider _mySlider;
    private CharacterStats.Mood _moodToDisplay;
    [HideInInspector] public string moodToDisplay;
    [HideInInspector] public int moodToDisplayIndex;

    [SerializeField] private Text _topText, _bottomText;
    // Use this for initialization
    private void Start()
    {
        for (int i = 0; i < characterStats.moods.Length; i++)
        {
            if (characterStats.moods[i].nameInInk == moodToDisplay)
                _moodToDisplay = characterStats.moods[i];
        }

        _mySlider = GetComponentInChildren<Slider>();
        _mySlider.minValue = -5;
        _mySlider.maxValue = 5;
        _mySlider.wholeNumbers = true;
        _topText.text = _moodToDisplay.maxName;
        _bottomText.text = _moodToDisplay.minName;
    }

    // Update is called once per frame
    private void Update()
    {
        _mySlider.value = _moodToDisplay.value;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class MoodSlider : MonoBehaviour
{
    public CharacterStats characterStats;
    private Slider _mySlider;
    private CharacterStats.Mood _moodToDisplay;
    [HideInInspector] public string moodToDisplay;
    [HideInInspector] public int moodToDisplayIndex;

    // Use this for initialization
    private void Start()
    {
        for (int i = 0; i < characterStats.moods.Length; i++)
        {
            if (characterStats.moods[i].nameInInk == moodToDisplay)
                _moodToDisplay = characterStats.moods[i];
        }

        _mySlider = GetComponent<Slider>();
        _mySlider.minValue = -5;
        _mySlider.maxValue = 5;
        _mySlider.wholeNumbers = true;
    }

    // Update is called once per frame
    private void Update()
    {
        _mySlider.value = _moodToDisplay.value;
    }
}
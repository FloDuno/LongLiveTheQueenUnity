using UnityEngine;
using UnityEngine.UI;

public class SkillSlider : MonoBehaviour
{
    public int[] statIndex = new int[2];

    private CharacterStats.Stat _statToDisplay;
    private Slider _slider;

    // Use this for initialization
    private void Start ()
    {
        _slider = GetComponent<Slider>();
        _slider.wholeNumbers = true;
        _statToDisplay = GameManager.Instance.characterStats.statGroups[statIndex[0]].stats[statIndex[1]];
        _slider.minValue = _statToDisplay.initialValue;

    }
	
	// Update is called once per frame
    private void Update ()
    {
        _statToDisplay = GameManager.Instance.characterStats.statGroups[statIndex[0]].stats[statIndex[1]];
        if (_statToDisplay.value > _slider.maxValue)
            _slider.maxValue = _statToDisplay.value;
        _slider.value = _statToDisplay.value;
    }
}

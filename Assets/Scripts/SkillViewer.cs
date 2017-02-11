using UnityEngine;
using UnityEngine.UI;

public class SkillViewer : MonoBehaviour
{
    public CharacterStats characterStats;
    [HideInInspector] public string skillToView;
    [HideInInspector] public int skillToViewIndex;
    [HideInInspector] public CharacterStats.StatGroup[] myStatGroups;
    [HideInInspector] public int myStatGroupsIndex;

    [SerializeField] private GameObject _statSliderPrefab, _titleGroupPrefab;


    // Use this for initialization
    void Start()
    {
        Text _header = GetComponentInChildren<Text>();
        _header.text = myStatGroups[0].type;
        Transform _slidersParent = GetComponentInChildren<VerticalLayoutGroup>().transform;
        for (int i = 0; i < myStatGroups.Length; i++)
        {
            GameObject _title = Instantiate(_titleGroupPrefab, _slidersParent);
            _title.GetComponent<Text>().text = myStatGroups[i].name;
            _title.transform.localScale = Vector3.one;
            for (int j = 0; j < myStatGroups[i].stats.Length; j++)
            {
                CharacterStats.Stat _statToPass = myStatGroups[i].stats[j];
                GameObject _skillSlider = Instantiate(_statSliderPrefab, _slidersParent);
                _skillSlider.GetComponentInChildren<SkillSlider>().statIndex = new []{GetStatGroupIndex(myStatGroups[i]),j};
                _skillSlider.GetComponentInChildren<Text>().text = _statToPass.name;
                _skillSlider.transform.localScale = Vector3.one;
            }
        }
    }

    private int GetStatGroupIndex(CharacterStats.StatGroup _statGroup)
    {
        for (int i = 0; i < characterStats.statGroups.Length; i++)
        {
            if (characterStats.statGroups[i].name == _statGroup.name)
                return i;
        }
        return -1;
    }
}
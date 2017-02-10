using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class IntroDisplayer : MonoBehaviour
{
    [SerializeField] private Text _nameCharacter;
    [SerializeField] private Text _mainText;
    [SerializeField] private Image _spriteHandler;
    [SerializeField] private DialogManager _dialogManager;
    [SerializeField] private GameObject _mainMenu;

    // Set this file to your compiled json asset
    public TextAsset inkAsset;

    // The ink story that we're wrapping
    private Story _inkStory;

    private DialogManager.DialogParameter _actualDialogParameter;

    // Use this for initialization
    void Start()
    {
        _inkStory = new Story(inkAsset.text);
        _inkStory.Continue();
    }
    // Update is called once per frame
    private void Update()
    {
        _mainText.text = _inkStory.currentText;
        if (Input.GetMouseButtonDown(0) && _inkStory.canContinue && _inkStory.currentChoices.Count == 0)
        {
            _inkStory.Continue();
            UpdateDisplay();
        }
        else if (Input.GetMouseButtonDown(0) && !_inkStory.canContinue)
        {
            _mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void UpdateDisplay()
    {
        if (_inkStory.currentTags.Count <= 0)
            return;

        if (_dialogManager.HasParameter(_inkStory.currentTags[0], out _actualDialogParameter))
        {
            _nameCharacter.text = _actualDialogParameter.nameInUnity;
            _spriteHandler.sprite = _actualDialogParameter.sprite;
        }
    }
}

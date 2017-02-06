using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

//Not implemented yet
public class ContentManager : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _choicesHandler;

    // Set this file to your compiled json asset
    public TextAsset inkAsset;

    // The ink story that we're wrapping
    private Story _inkStory;

    private Button[] _choicesButtons;

    // Use this for initialization
    private void Start()
    {
        _inkStory = new Story(inkAsset.text);
        _inkStory.Continue();
        _choicesButtons = _choicesHandler.GetComponentsInChildren<Button>();
        //        DisplayTextDebug();
    }

    private void DisplayTextDebug()
    {
        _inkStory.ChoosePathString("Jour1");
        print(_inkStory.ContinueMaximally());
        _inkStory.ChoosePathString("Jour2");
        print(_inkStory.ContinueMaximally());
    }

    // Update is called once per frame
    private void Update()
    {
        _text.text = _inkStory.currentText;
        if (Input.GetMouseButtonDown(0) && _inkStory.canContinue)
            _inkStory.Continue();

        CheckChoices();
    }

    private void CheckChoices()
    {
        if (_inkStory.currentChoices.Count <= 0)
        {
            _choicesHandler.SetActive(false);
            return;
        }

        _choicesHandler.SetActive(true);
        for (int i = 0; i < _choicesButtons.Length; i++)
        {
            if (i < _inkStory.currentChoices.Count)
            {
                int _choice = i;
                _choicesButtons[i].gameObject.SetActive(true);
                if (_choicesButtons[i].GetComponentInChildren<Text>().text != _inkStory.currentChoices[i].text)
                {
                    _choicesButtons[i].GetComponentInChildren<Text>().text = _inkStory.currentChoices[i].text;
                    _choicesButtons[i]
                        .onClick.AddListener(
                            delegate
                            {
                                _inkStory.ChooseChoiceIndex(_choice);
                                _inkStory.Continue();
                            });
                }
            }
            else
            {
                _choicesButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void CheckChoicesDebug()
    {
        if (_inkStory.currentChoices.Count > 0 && Input.inputString != null)
        {
            string _actualKey = Input.inputString;
            int _choice;
            if (int.TryParse(_actualKey, out _choice) && _choice < _inkStory.currentChoices.Count)
            {
                _inkStory.ChooseChoiceIndex(_choice);
                _inkStory.Continue();
            }
        }
    }
}
﻿using System.Collections;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

//Not implemented yet
public class ContentManager : MonoBehaviour
{
    [SerializeField] private Text _nameCharacter;
    [SerializeField] private Text _mainText;
    [SerializeField] private Image _spriteHandler;
    [SerializeField] private GameObject _choicesHandler;
    [SerializeField] private DialogManager _dialogManager;

    // Set this file to your compiled json asset
    public TextAsset inkAsset;

    // The ink story that we're wrapping
    private Story _inkStory;

    private Button[] _choicesButtons;
    private DialogManager.DialogParameter _actualDialogParameter;

    // Use this for initialization
    private void Start()
    {
        _inkStory = new Story(inkAsset.text);
        _inkStory.Continue();
        _choicesButtons = _choicesHandler.GetComponentsInChildren<Button>();
        for (int i = 0; i < GameManager.Instance.characterStats.moods.Length; i++)
        {
            _inkStory.ObserveVariable(
                GameManager.Instance.characterStats.moods[i].nameInInk,
                (string _varName, object _newValue) =>
                {
                    GameManager.Instance.characterStats.moods[i].value = (int)_newValue;
                });
        }
    }

    // Update is called once per frame
    private void Update()
    {
        _mainText.text = _inkStory.currentText;
        if (Input.GetMouseButtonDown(0) && _inkStory.canContinue)
        {
            _inkStory.Continue();
            UpdateDisplay();
        }
        else if(Input.GetMouseButtonDown(0) && !_inkStory.canContinue)
        {
            GameManager.Instance.menu.SetActive(true);
            gameObject.SetActive(false);
        }

        CheckChoices();
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

    public void ChooseDialog(int _day)
    {
        if (_inkStory == null)
        {
            StartCoroutine(DelayChooseDialog(_day));
            return;
        }
        _inkStory.ChoosePathString("Day" + _day);
        _inkStory.Continue();
        UpdateDisplay();
    }

    private IEnumerator DelayChooseDialog(int _day)
    {
        while (_inkStory == null)
        {
            yield return null;
        }
        ChooseDialog(_day);
    }
}
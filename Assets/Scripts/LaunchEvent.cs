using UnityEngine;
using UnityEngine.UI;

//Button to confirm lessons of the day
public class LaunchEvent : MonoBehaviour
{
    private Button _myButton;

	// Use this for initialization
    private void Start ()
	{
	    _myButton = GetComponent<Button>();
	    //We pass the delegate function to the OnClick event which means when we click on it, it'll launch what's inside brackets
	    _myButton.onClick.AddListener(delegate
	    {
	        EventManager.EndOfTheDay(GameManager.Instance.characterStats);
	    });

	}
}

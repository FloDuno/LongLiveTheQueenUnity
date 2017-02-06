//All the event should have this signature : return void and characterStats as parameters

public delegate void ChangeStatEventHandler(CharacterStats _characterStats);

//Stock the events and launch them
public static class EventManager
{
    public static event ChangeStatEventHandler ChangeStatEvent;

    public static ActivityToggle[] activitiesOfTheDay = new ActivityToggle[2];
    public static ActivityToggle[] optionsOfTheDay = new ActivityToggle[2];

    public static void EndOfTheDay(CharacterStats _characterStats)
    {
        if (ChangeStatEvent != null)
        {
            ChangeStatEvent(_characterStats);
        }
    }
}


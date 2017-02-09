# LongLiveTheQueenUnity
Base to make a Long Live The Queen clone

Story made with Ink : http://www.inklestudios.com/ink/

Text scene is only here for testing. MainMenu is the real scene.

In Assets/ConfigFiles you can can change the player stats via the CharacterStats asset and the "lessons" with the ActivityWeek asset (the other ones aren't implemented yet).
In the ActivityWeek asset, the Activities Equal Stat is to make a lesson for every stat and every lesson increase chosen stat by one.

If you want to change the story, edit the Story.ink file and don't forget to export to JSON. You'll figure how to choose which day equals wich text by reading the file itself.

After changing the character stats, look at the ActivitiyManager and every GameObject in the SkillsMenu. Don't change anything, Unity just need to reload the informations

You can set the name above every dialog and the sprite used for the character in the DialogParameters object.

In the ink story, you have to put a tag (it's what's after a #) to specify which character is talking (mainCharacter in the example)

To change the mood based on the choices in the story, you have to change in ink the variable named like in CharacterStats -> Mood -> Name in ink as shown in the example

It's been a pain in the ass, really…
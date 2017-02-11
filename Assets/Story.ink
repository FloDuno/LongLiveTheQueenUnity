VAR Mood1 = 0

===Day1===
Ceci est le jour1 #MainCharac
->END

===Day2===
Ceci est le jour2 avec une question premettant de changer le mood #SecondCharac
* Mood + 1
~Mood1 = Mood1 + 1
->END
* Mood + 5
~Mood1 = Mood1 + 5
->END

===Day3===
Ceci est un autre choix qui fait croire que c'est une histoire à embranchement #ThirdCarac
* Histoire 1
->Day3Choix1
* Histoire 2
->Day3Choix2
* Histoire 3
->Day3Choix3
->END


===Day3Choix1===
Ceci est le choix 1
->END

===Day3Choix2===
Ceci est le choix 2
->END

===Day3Choix3===
Ceci est le choix 3
->END

===Day4===
C'est le jour 4 et je commence à être à court d'idées…
->END

===Day5===
Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
->END

===Day6===
Nam lacinia neque quis elementum mattis. 
->END

===Day7===
Nulla tempor erat velit, id molestie mi pharetra non. Aliquam id arcu efficitur, porta lorem ut, lacinia augue.
->END

===End===
Fin du game ! #MainCharac
->END
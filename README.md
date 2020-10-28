# Mini Hero Game
Made with Unity

In this game you play as a Super-Hero of your own making.
You choose the aperance. You then pick a primary and secondary power for them and unlock abilities from those powers.
For example, if you choose energy powers as you primary, you unlock abilities from among four dirernt energy baset poweres, from a beam that hits enemies from a far
to a burst that hits all enemies near you, and more!
There are over a dozen abilities currenlt in the game and more in development.

Once you have made your character, you explore levels, defeat enemies and collect xp to gain unlock-Points which can be used to unlock and upgrade new abilites.
At the moment here are very few levels and enemy types, but I have plans to add more soon.

About the projects design:
The basic structure of the project is that of a regular Unity project. The C# assets are found in Mini-Hero-Project/Assets/Scripts.
Some examples of how thing are handled: 
Each Character (player or enemy AI character) has a Manager that manages its inputs and transilates them to in game actions. 
For the player its pysical inputs, for AI its logic inputs based on what the AI thins it should do at that moment. There is a manager interface that all the managers 
implamanet to allow the dirrerent types of characters to interact with other classes. In the project the characters have a large amount of abilities they can use. 
The managers are in charge of when to use the abilities and the abilities can request data from the manager. Abilities are implemented as an abstract class that 
implements all the similar functions different abilities have and then each inherating ability implements the abstract members in the base class based on its own needs,
such as what happens when it's tuned on or off.
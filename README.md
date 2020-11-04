# Mini Hero Game
Mini-Hero Game

Make your own mini superhero!  They’re mini in stature, but stacked with powers!

First, you choose the appearance (costume, colors, gender, etc). Then, you pick primary and secondary powers, and unlock abilities within those powers. For example, if you choose energy powers as you primary, you unlock abilities from a list of energy-based powers, such as a beam that hits enemies from afar, a burst that hits all the enemies near you, and more.
Once you have made your character, you explore levels, defeat enemies and collect xp to gain unlock-points, which can be used to unlock new abilities and upgrade current ones. 

Version 1.0 has only 4 levels and 4 enemy types, but I have plans to add more soon!


Project Design:

This game was developed using Unity, and its basic structure is that of a regular Unity project. The C# assets are found in https://github.com/ramisirote/Mini-Hero-Project/tree/main/Assets/Scripts.
Some examples of how thing are handled:
 
Each Character (player or enemy AI character) has a Manager that manages its inputs and translates them to game actions. For the player it’s physical game control inputs, for AI it’s logic inputs based on what the AI thinks it should do at that moment. 
There is a Manager interface that all the Managers implement to allow different types of characters to interact with similar classes. Characters have many Abilities they can use. The Managers are in charge of when to use Abilities, and Abilities can request data from the Manager. Abilities are defined as an abstract class that implements all the similar functions different abilities have. Each inheriting Ability implements the abstract members in the base class based on its own needs.
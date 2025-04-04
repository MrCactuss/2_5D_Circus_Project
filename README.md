# 2.5D Circus Game (Unity Project)

## Description

This project was created in Unity to learn and demonstrate various engine features, focusing initially on UI elements, basic animation, C# scripting fundamentals, and saving player preferences. It has evolved into a simple 2.5D circus-themed **roll-and-move board game** where players race to the finish line. Key learning areas include UI setup, scene management, physics (for dice), basic game loop management, and data persistence using PlayerPrefs.

## Screenshots / GIFs

*(Add screenshots or GIFs of your game here!)*

* ![Gameplay Screenshot 1](path/to/your/screenshot1.png)
* ![Dice Roll GIF](path/to/your/diceroll.gif)
* *(You can add links or embed images depending on where this README is hosted, like GitHub)*

## Features

* **Custom Cursor:** Implemented a custom cursor script.
* **UI Animation:** Basic animation applied to UI elements.
* **Audio:** Background music and sound effects integrated.
* **Character Selection:** A screen allowing the player to choose a character, enter a name, and set the player count (saved via PlayerPrefs).
* **Player Spawning:** Automatically spawns the selected main character and randomly chosen/named opponents based on PlayerPrefs.
* **Physics-Based Dice Rolling:** A clickable 3D die that uses Rigidbody physics for rolling.
* **Dice Result Detection & Display:** Detects which face landed up and displays the number on UI Text.
* **Basic Board Game Logic (`GameManager`):**
    * Manages player turns.
    * Reads dice results.
    * Handles starting rules (roll 1 or 6).
    * Handles "Roll 6, go again" rule.
    * Calculates target square, including overshoot/bounce-back logic.
    * Detects win condition (exact landing).
    * Moves pieces visually (currently smooth Lerp movement with basic walk animation trigger).
* **Pause Menu:** A functional pause menu (activated by key press) with options to Resume and Quit (loads Main Menu scene), correctly handling `Time.timeScale`.
* **Options Menu (Partial):** Includes functional Volume slider, Graphics Quality presets, and Resolution/Fullscreen options with Apply/Revert logic (may need further debugging/polish).
* **Other Effects:** Includes a `JiggleScript` for visual feedback on objects.

## Technology

* **Engine:** Unity (Specify Version, e.g., Unity 202X.Y.Z)
* **Language:** C#

## Core Scripts

* `GameManager.cs`: Controls the main board game loop, turns, and piece movement.
* `PlayerScript.cs`: Handles spawning player characters based on PlayerPrefs at the start.
* `DiceRollScript.cs`: Manages the physics and state of the clickable dice.
* `SideDetectorScript.cs`: Detects which side of the dice landed up.
* `RolledNumberScript.cs`: Displays the dice result on the UI.
* `CharacterAnimationScript.cs`: Controls character animations (Idle, Walk, Hurt).
* `PauseManager.cs`: Handles pausing, resuming, and the pause menu UI.
* `GraphicsSettingsManager.cs` (or similar): Handles Volume, Quality, Resolution settings.
* `NameScript.cs` (Assumed): Attached to characters to handle displaying names.
* `JiggleScript.cs`: Adds oscillation effect.
* `SceneChangeScript.cs` (Mentioned earlier): Potentially used for scene transitions.

## How To Play (Basic)

1.  Start the game (presumably from a Main Menu scene).
2.  Navigate to the Character Selection screen, choose a character, enter a name, set player count.
3.  Start the main game scene.
4.  When it's your turn, click the Dice object to roll it.
5.  Watch the dice result appear in the UI and your character piece move along the board.
6.  The first player to land exactly on square 120 wins.
7.  Press `Escape` (or assigned key) to pause the game.

## Development Log / To-Do List

* [x] Create script to change cursor
* [x] Add and animate UI elements
* [x] Add background music and sounds
* [ ] Add animated characters and prefabs *(Animation linking needs polish)*
* [x] Create character selection screen
* [x] Learn about player prefs ~~and saving in json~~ *(Implemented PlayerPrefs, discussed JSON)*
* [x] Write script for dice rolling
* [x] Implement basic board game turn structure and movement
* [x] Implement pause menu and scene changing
* [x] Implement basic options menu (Volume, Graphics)
* **Next Steps / Future Improvements:**
    * [ ] Implement Special Square logic (Snakes/Ladders).
    * [ ] Fully debug/polish Options Menu UI and functionality (e.g., dropdown layout).
    * [ ] Fully debug/polish character animations during movement.
    * [ ] Add more visual feedback (UI updates for turns, highlights, etc.).
    * [ ] Implement AI for opponent turns (currently they likely just exist visually).
    * [ ] Refine dice physics/detection if needed.
    * [ ] Add visual polish to board and pieces.
    * *(Continue to write to-do list when you start development on your own)*

## License

*Specify your license here (e.g., MIT, Apache 2.0, or leave blank if unsure)*

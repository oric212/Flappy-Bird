# Flappy Bird Clone

A Flappy Bird-inspired arcade game built in Unity.

The project recreates the classic flap-and-dodge gameplay while adding randomized obstacles, persistent high scores, audio controls, and optional Arcade Mode features such as Coins and Sonic speed boosts.

## Features

- Physics-based flap controls
- Endless ground and background
- Procedural obstacle generation with route validation, safe rerolls, and fallback layouts to prevent impossible pipe combinations
- Standard, asymmetric, top-only, and bottom-only pipes
- Score and persistent high score
- Optional Arcade Mode
- Coins and Sonic speed boosts
- Main Menu and Game Over screens
- Music and SFX controls
- Keyboard and mouse input

## Controls

- **Space / Left Click** - Flap
- **Space** - Start / Restart
- **Mouse** - Navigate menus and settings
- **Escape** - Quit the game in a build

## Main Scripts

- `BirdController.cs` - Bird movement, flap input, collisions, death, and speed boost
- `GameManager.cs` - Game states, restarting, Arcade Mode, and high score
- `PipeSpawner.cs` - Spawns obstacles and collectibles and handles cleanup
- `PipeLayoutGenerator.cs` - Generates randomized pipe layouts
- `PipeLayoutValidator.cs` - Validates pipe sizes and playable routes
- `PipeObstacle.cs` - Handles scoring when passing obstacles
- `ScoreManager.cs` - Stores and updates the current score
- `CoinCollectible.cs` - Handles Coin pickups
- `SonicPowerUp.cs` - Handles temporary speed boosts
- `GroundLooper.cs` - Recycles ground chunks
- `BackgroundLooper.cs` - Recycles background chunks
- `CameraFollow.cs` - Follows the Bird horizontally
- `BirdAnimation.cs` - Handles Bird sprite animation
- `GameUI.cs` - Controls menus, HUD, score, and boost display
- `AudioManager.cs` - Handles music, sound effects, and volume settings

## Project Structure

```text
Flappy-Bird/
├── Assets/
│   ├── Art/
│   ├── Audio/
│   ├── fonts/
│   ├── Prefabs/
│   ├── Scenes/
│   │   └── FlappyBird.unity
│   └── Scripts/
│       ├── AudioManager.cs
│       ├── BackgroundLooper.cs
│       ├── BirdAnimation.cs
│       ├── BirdController.cs
│       ├── CameraFollow.cs
│       ├── CoinCollectible.cs
│       ├── GameManager.cs
│       ├── GameUI.cs
│       ├── GroundLooper.cs
│       ├── PipeLayoutGenerator.cs
│       ├── PipeLayoutValidator.cs
│       ├── PipeObstacle.cs
│       ├── PipeSpawner.cs
│       ├── ScoreManager.cs
│       └── SonicPowerUp.cs
├── Packages/
├── ProjectSettings/
├── .gitattributes
├── .gitignore
└── README.md
```

## Built With

- Unity `6000.3.20f1`
- Universal Render Pipeline / Universal 2D
- Unity Input System
- TextMeshPro
- C#

## Running the Project

1. Clone the repository.
2. Open it in Unity `6000.3.20f1` or a compatible Unity 6 version.
3. Open `Assets/Scenes/FlappyBird.unity`.
4. Press Play.

## License

See the included asset credits and licenses for third-party artwork, audio, and fonts.

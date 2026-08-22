# Flappy Bird Clone

A Flappy Bird-inspired arcade game built in Unity.

It includes the classic flap-and-dodge gameplay, procedural pipe generation, scoring, high scores, audio controls, and optional Arcade Mode features such as Coins and Sonic speed boosts.

## Features

- Space / Left Click flap controls
- Endless ground and background
- Randomized pipe layouts
- Score and persistent high score
- Coins and Sonic speed boosts
- Arcade Mode toggle
- Main Menu and Game Over UI
- Music and SFX controls

## Main Scripts

- `BirdController.cs` - Bird movement, flap input, collisions, death, and speed boost
- `GameManager.cs` - Game states, restart flow, Arcade Mode, and high score
- `PipeSpawner.cs` - Pipe spawning, collectibles, tracking, and cleanup
- `PipeLayoutGenerator.cs` - Generates randomized pipe layouts
- `PipeLayoutValidator.cs` - Validates pipe sizes and playable routes
- `PipeObstacle.cs` - Handles scoring when passing obstacles
- `ScoreManager.cs` - Stores the current score
- `CoinCollectible.cs` - Handles Coin pickups
- `SonicPowerUp.cs` - Handles Sonic speed boosts
- `GroundLooper.cs` - Recycles ground chunks
- `BackgroundLooper.cs` - Recycles background chunks
- `CameraFollow.cs` - Follows the Bird horizontally
- `BirdAnimation.cs` - Handles Bird sprite animation
- `GameUI.cs` - Updates menus, score, high score, and boost HUD
- `AudioManager.cs` - Handles music, SFX, and volume settings

## Project Structure

Assets/
├── Art/        # Bird, pipes, ground, background, Coins, Sonic
├── Audio/      # Music and sound effects
├── fonts/      # PixelOperator / TextMeshPro fonts
├── Prefabs/    # Pipes and collectible prefabs
├── Scenes/     # FlappyBird.unity
└── Scripts/    # Gameplay and UI C# scripts

Packages/       # Unity package configuration
ProjectSettings/# Unity project settings

## Built With

- Unity 6000.3.20f1
- Universal 2D / URP
- Unity Input System
- TextMeshPro
- C#

## Running

1. Clone the repository.
2. Open it in Unity 6000.3.20f1 or a compatible Unity 6 version.
3. Open `Assets/Scenes/FlappyBird.unity`.
4. Press Play.

## License

See the included asset credits and licenses for third-party artwork, audio, and fonts.

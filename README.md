# Flappy Bird Clone

A Flappy Bird-inspired arcade game built in Unity.

The project recreates the core Flappy Bird gameplay loop while adding optional arcade-style features such as Coins, Sonic speed boosts, persistent high scores, audio settings, and multiple randomized obstacle layouts.

## Features

- Physics-based flap controls
- Endless ground and background recycling
- Procedurally generated pipe obstacles
- Standard, asymmetric, top-only, and bottom-only pipe layouts
- Score and persistent high-score tracking
- Optional Arcade Mode
- Coins that award bonus score
- Sonic pickups that temporarily increase movement speed
- Main Menu and Game Over screens
- Music and SFX volume controls
- Keyboard and mouse input

## Controls

- Space / Left Click - Flap
- Space - Start / Restart
- Mouse - Navigate menus and settings
- Escape - Quit in a build

## Arcade Mode

Arcade Mode can be enabled or disabled from the Main Menu.

When enabled:

- Coins can appear and award +1 score
- Sonic pickups can appear and temporarily increase the Bird's forward speed

Pipe generation remains randomized regardless of Arcade Mode.

## Main Scripts

### BirdController.cs

Handles the Bird's core gameplay behavior:

- flap input
- horizontal movement
- gravity
- Sonic speed boost
- top-screen boundary detection
- collision-based death
- freezing physics after Game Over

### GameManager.cs

Controls the main game states:

- Main Menu
- Playing
- Game Over

It also manages:

- starting and restarting runs
- returning to the Main Menu
- Arcade Mode preference
- persistent high score

### PipeSpawner.cs

Coordinates runtime obstacle spawning.

Responsibilities include:

- deciding when and where obstacles spawn
- maintaining obstacle density
- spawning Coins and Sonic pickups
- tracking active obstacles
- cleaning up obstacles behind the camera

### PipeLayoutGenerator.cs

Generates pipe layouts.

It handles:

- weighted obstacle-type selection
- randomized pipe dimensions
- standard pairs
- asymmetric pairs
- top-only and bottom-only pipes
- safe fallback layouts

### PipeLayoutValidator.cs

Checks generated pipe layouts before they are spawned.

It validates:

- minimum visible pipe height
- minimum playable route height
- safe overlap between nearby obstacle routes

### PipeObstacle.cs

Tracks an individual obstacle group and awards one score point after the Bird safely passes it.

### ScoreManager.cs

Stores the current score and allows score increases only while gameplay is active.

### CoinCollectible.cs

Handles Coin collection and awards bonus score.

### SonicPowerUp.cs

Handles Sonic pickup collection and applies the temporary speed boost.

### GroundLooper.cs

Recycles ground chunks to create an endless floor without continuously spawning new terrain.

### BackgroundLooper.cs

Recycles background chunks so the environment can continue indefinitely.

### CameraFollow.cs

Follows the Bird horizontally while keeping the intended vertical camera framing.

### BirdAnimation.cs

Cycles through the Bird sprite frames to create the flying animation.

### GameUI.cs

Controls and updates the game's UI, including:

- current score
- high score
- Main Menu
- Game Over screen
- Arcade Mode display
- Sonic boost indicator
- Music and SFX sliders

### AudioManager.cs

Manages background music, sound effects, and persistent volume settings.

## Project Structure

Flappy-Bird/
├── Assets/
│   ├── Art/
│   │   ├── Background/
│   │   │   └── Background sprites used by the endless background system
│   │   ├── Coins/
│   │   │   └── Coin artwork and related assets
│   │   ├── Player/
│   │   │   └── Bird sprite sheets and animation frames
│   │   ├── Sonic/
│   │   │   └── Sonic speed-boost artwork
│   │   └── Tiles/
│   │       └── Ground and pipe sprite assets
│   │
│   ├── Audio/
│   │   ├── music/
│   │   │   └── Background music
│   │   └── sounds/
│   │       └── Flap, score, death, Coin, and power-up sound effects
│   │
│   ├── fonts/
│   │   ├── PixelOperator8.ttf
│   │   ├── PixelOperator8-Bold.ttf
│   │   └── Generated/
│   │       └── TextMeshPro font assets
│   │
│   ├── Prefabs/
│   │   └── Reusable gameplay objects such as pipe groups,
│   │       Coins, and Sonic pickups
│   │
│   ├── Scenes/
│   │   └── FlappyBird.unity
│   │
│   ├── Scripts/
│   │   ├── AudioManager.cs
│   │   ├── BackgroundLooper.cs
│   │   ├── BirdAnimation.cs
│   │   ├── BirdController.cs
│   │   ├── CameraFollow.cs
│   │   ├── CoinCollectible.cs
│   │   ├── GameManager.cs
│   │   ├── GameUI.cs
│   │   ├── GroundLooper.cs
│   │   ├── PipeLayoutGenerator.cs
│   │   ├── PipeLayoutValidator.cs
│   │   ├── PipeObstacle.cs
│   │   ├── PipeSpawner.cs
│   │   ├── ScoreManager.cs
│   │   └── SonicPowerUp.cs
│   │
│   ├── InputSystem_Actions.inputactions
│   └── DefaultVolumeProfile.asset
│
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
│
├── ProjectSettings/
│   └── Unity project configuration
│
├── .gitattributes
├── .gitignore
└── README.md

## How the Main Systems Work Together

The main gameplay flow is:

GameManager
    ↓
BirdController
    ↓
PipeSpawner
    ↓
PipeLayoutGenerator
    ↓
PipeLayoutValidator
    ↓
PipeObstacle / CoinCollectible / SonicPowerUp
    ↓
ScoreManager
    ↓
GameUI

GroundLooper and BackgroundLooper independently recycle their chunks as the camera moves forward, allowing the game world to continue without creating an unlimited number of objects.

## Main Scene

The main Unity scene is:

Assets/Scenes/FlappyBird.unity

## Built With

- Unity 6000.3.20f1
- Universal Render Pipeline / Universal 2D
- Unity Input System
- TextMeshPro
- C#

## Running the Project

1. Clone the repository.
2. Open it using Unity 6000.3.20f1 or a compatible Unity 6 version.
3. Open Assets/Scenes/FlappyBird.unity.
4. Press Play.

## License

See the included asset credits and licenses for third-party artwork, audio, and fonts.

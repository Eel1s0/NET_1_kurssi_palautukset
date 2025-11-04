# CaveShooter

A 2-4 player multiplayer cave shooter game built with C# and Raylib.

## Features

- **Multiplayer Support**: 2-4 players can play simultaneously
- **Multiple Game States**: Start menu, in-game, paused, scoreboard, and game over screens
- **Game Controller Support**: Full gamepad/controller support for all players
- **Multiple Weapon Types**: Basic, Laser, and Mine weapons with different characteristics
- **Cave Environment**: Navigate through cave systems with collision detection

## Project Structure

### Game Management
- **Game.cs**: Main game class managing the game loop and states
- **Map.cs**: Handles map loading and collision detection with walls

### Entities
- **Player.cs**: Manages player state, input (keyboard/gamepad), and score
- **Ship.cs**: Represents the player's vehicle with physics
- **Camera.cs**: Camera system that follows the player's ship

### Weapon System
- **IWeapon.cs**: Interface for all weapon types
- **Basic.cs**: Standard weapon with moderate fire rate
- **Laser.cs**: High-speed, rapid-fire weapon
- **Mine.cs**: Drops stationary explosive projectiles
- **BulletManager.cs**: Manages the lifecycle of all projectiles
- **StandardBullet.cs**: Basic projectile implementation

### Menus
- **StartMenu.cs**: Main menu for selecting number of players

## Controls

### Player 1 (Keyboard)
- **W**: Thrust
- **A**: Rotate Left
- **D**: Rotate Right
- **Space**: Fire

### Player 2 (Keyboard)
- **Up Arrow**: Thrust
- **Left Arrow**: Rotate Left
- **Right Arrow**: Rotate Right
- **Right Control**: Fire

### Player 3 (Keyboard)
- **I**: Thrust
- **J**: Rotate Left
- **L**: Rotate Right
- **H**: Fire

### Player 4 (Keyboard)
- **Numpad 8**: Thrust
- **Numpad 4**: Rotate Left
- **Numpad 6**: Rotate Right
- **Numpad 0**: Fire

### Gamepad/Controller
- **Left Stick**: Rotate
- **A Button / Right Trigger**: Thrust
- **B Button / Right Bumper**: Fire

### General Controls
- **ESC**: Pause game / Return to menu
- **1-4**: Select number of players (from main menu)

## Building and Running

```bash
# Build the project
dotnet build

# Run the project
dotnet run
```

## Requirements

- .NET 8.0 or higher
- Raylib-cs 7.0.1
- Windows, macOS, or Linux

## Game States

1. **Start Menu**: Select the number of players (1-4)
2. **In-Game**: Active gameplay with cave navigation and combat
3. **Paused**: Pause the game and choose to resume or quit
4. **Game Over**: Displayed when all players lose their lives
5. **Scoreboard**: Shows final scores for all players

## Gameplay

- Navigate your ship through the cave environment
- Avoid collision with walls (costs lives)
- Each player starts with 3 lives
- Score points by surviving and using weapons strategically
- Last player(s) standing wins!

## Future Enhancements

This project structure supports easy addition of:
- More weapon types
- Different map layouts
- Power-ups and collectibles
- AI enemies
- Network multiplayer
- Sound effects and music

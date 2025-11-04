# CaveShooter Architecture

## Class Diagram Overview

This document describes the architecture and relationships between classes in the CaveShooter game.

## Component Hierarchy

```
Program (Entry Point)
    └── Game (Main Game Loop)
        ├── GameState Enum (StartMenu, InGame, Paused, Scoreboard, GameOver)
        ├── Player[] (1-4 players)
        │   ├── Ship (Physics & Rendering)
        │   ├── IWeapon (Current weapon)
        │   │   ├── Basic
        │   │   ├── Laser
        │   │   └── Mine
        │   └── Score/Lives Management
        ├── Camera (Follows player ship)
        ├── Map (Collision detection)
        │   └── Wall[] (Cave boundaries)
        ├── BulletManager (Projectile lifecycle)
        │   └── StandardBullet[] (Active projectiles)
        └── StartMenu (UI)
```

## Core Classes

### Game Management Layer

#### **Game**
- **Purpose**: Main game loop and state management
- **Responsibilities**:
  - Initialize Raylib window and game components
  - Manage game states (menu, playing, paused, etc.)
  - Coordinate updates between all game systems
  - Handle rendering pipeline
- **Key Methods**:
  - `Run()`: Main game loop
  - `StartNewGame(int playerCount)`: Initialize new game session
  - `Update(float deltaTime)`: Update all game systems
  - `Draw()`: Render current game state

#### **Map**
- **Purpose**: World representation and collision detection
- **Responsibilities**:
  - Store wall/obstacle data
  - Check collisions between entities and walls
  - Render cave environment
- **Key Methods**:
  - `LoadBasicCave()`: Generate default cave layout
  - `CheckCircleCollision(Vector2, float)`: Collision detection
  - `Draw()`: Render walls

### Entity Layer

#### **Player**
- **Purpose**: High-level player state and input handling
- **Responsibilities**:
  - Process keyboard/gamepad input
  - Manage score and lives
  - Control weapon selection and firing
  - Update owned ship
- **Key Properties**:
  - `PlayerId`: Unique player identifier (1-4)
  - `Ship`: Reference to player's ship
  - `CurrentWeapon`: Active weapon
  - `Score` and `Lives`: Player stats
- **Key Methods**:
  - `Update(deltaTime, bulletManager, screenWidth, screenHeight)`: Process input and update
  - `HandleKeyboardInput()` / `HandleGamepadInput()`: Input processing
  - `Fire(bulletManager)`: Weapon firing

#### **Ship**
- **Purpose**: Player vehicle with physics simulation
- **Responsibilities**:
  - Physics (position, velocity, rotation)
  - Movement (thrust, rotation)
  - Rendering as triangle
  - Screen wrapping
- **Key Methods**:
  - `Thrust(deltaTime)`: Apply forward acceleration
  - `RotateLeft/Right(deltaTime)`: Rotate ship
  - `Update(deltaTime)`: Update physics
  - `Draw()`: Render ship

#### **Camera**
- **Purpose**: Viewport control following player
- **Responsibilities**:
  - Smooth camera following
  - Coordinate transformations
- **Key Methods**:
  - `SetTarget(Ship)`: Assign ship to follow
  - `Update(deltaTime)`: Smooth camera movement
  - `BeginMode()` / `EndMode()`: Camera rendering scope

### Weapon System Layer

#### **IWeapon** (Interface)
- **Purpose**: Contract for all weapon types
- **Required Properties**:
  - `Cooldown`: Time between shots
  - `CanFire`: Ready to fire
- **Required Methods**:
  - `Fire(position, direction, bulletManager)`: Create projectile
  - `Update(deltaTime)`: Update cooldown

#### **Weapon Implementations**

**Basic**
- Standard fire rate (5 shots/second)
- Moderate speed (500 units/second)
- Yellow projectiles

**Laser**
- Fast fire rate (10 shots/second)  
- High speed (800 units/second)
- Red projectiles

**Mine**
- Slow fire rate (1 per second)
- Stationary (zero velocity)
- Orange projectiles

#### **BulletManager**
- **Purpose**: Centralized projectile management
- **Responsibilities**:
  - Store all active bullets
  - Update bullet physics
  - Remove out-of-bounds bullets
  - Render all bullets
- **Key Methods**:
  - `AddBullet(StandardBullet)`: Register new projectile
  - `Update(deltaTime)`: Update all bullets
  - `Draw()`: Render all bullets
  - `GetBullets()`: Access for collision detection

#### **StandardBullet**
- **Purpose**: Individual projectile representation
- **Properties**:
  - `Position`, `Velocity`: Physics
  - `Radius`, `Color`: Visual
  - `IsActive`: Lifecycle state
  - `OwnerId`: Which player fired it
- **Key Methods**:
  - `Update(deltaTime)`: Update position
  - `Draw()`: Render bullet
  - `IsOutOfBounds()`: Check if off-screen

### UI Layer

#### **StartMenu**
- **Purpose**: Main menu interface
- **Responsibilities**:
  - Display game title and options
  - Show control instructions
  - Handle player count selection

## Data Flow

### Input → Action Flow
```
Keyboard/Gamepad Input
    ↓
Player.HandleInput()
    ↓
Ship.Thrust/Rotate() OR Weapon.Fire()
    ↓
Ship physics update OR BulletManager.AddBullet()
```

### Game Update Flow
```
Game.Update(deltaTime)
    ├→ Player.Update() for each player
    │   ├→ Ship.Update()
    │   └→ Weapon.Update()
    ├→ BulletManager.Update()
    ├→ Camera.Update()
    └→ CheckCollisions()
        ├→ Map.CheckCircleCollision() for players
        └→ Map.CheckCircleCollision() for bullets
```

### Rendering Flow
```
Game.Draw()
    ↓
Camera.BeginMode()
    ├→ Map.Draw()
    ├→ BulletManager.Draw()
    └→ Player.Draw() for each player
        └→ Ship.Draw()
Camera.EndMode()
    ↓
DrawUI() (scores, lives)
```

## Design Patterns

### Strategy Pattern
- **IWeapon** interface allows runtime weapon switching
- Different weapon behaviors (Basic, Laser, Mine) implement same interface
- Player can change `CurrentWeapon` without code changes

### State Pattern
- **GameState** enum manages different game modes
- Different update/draw logic based on current state
- Clean state transitions

### Manager Pattern
- **BulletManager** centrally manages all projectiles
- **Game** class orchestrates all subsystems
- Single point of control for lifecycle management

## Extension Points

### Adding New Weapons
1. Create class implementing `IWeapon`
2. Define fire rate, bullet speed, and visual properties
3. Implement `Fire()` and `Update()` methods
4. Add to player's weapon selection

### Adding New Game Modes
1. Add new value to `GameState` enum
2. Implement Update method for new state
3. Implement Draw method for new state
4. Add state transition logic

### Adding AI Players
1. Create `AIPlayer` class extending `Player`
2. Override `HandleInput()` methods
3. Implement AI decision logic
4. Add to players list in `Game`

### Network Multiplayer
1. Abstract input handling to support remote input
2. Add network synchronization layer
3. Implement client-server architecture
4. Sync game state across network

## Performance Considerations

- **Object Pooling**: BulletManager could pool bullet objects for reuse
- **Spatial Partitioning**: Map collision could use quadtree for large maps
- **Delta Time**: All movement uses delta time for frame-rate independence
- **List Cleanup**: Inactive bullets removed each frame to prevent memory leaks

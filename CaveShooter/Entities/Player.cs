using System.Numerics;
using Raylib_cs;
using CaveShooter.Weapons;

namespace CaveShooter.Entities
{
    /// <summary>
    /// Represents a player with their ship, input handling, and score.
    /// Supports up to 4 players with different control schemes.
    /// </summary>
    public class Player
    {
        public int PlayerId { get; private set; }
        public Ship Ship { get; private set; }
        public IWeapon CurrentWeapon { get; set; }
        public int Score { get; set; }
        public int Lives { get; set; }
        public bool IsActive { get; set; }

        // Controller/gamepad support
        private int gamepadId;
        private bool useGamepad;

        // Keyboard controls for different players
        private KeyboardKey thrustKey;
        private KeyboardKey leftKey;
        private KeyboardKey rightKey;
        private KeyboardKey fireKey;

        public Player(int playerId, Vector2 startPosition, Color shipColor, bool useGamepad = false, int gamepadId = 0)
        {
            PlayerId = playerId;
            Ship = new Ship(startPosition, shipColor);
            CurrentWeapon = new Basic(); // Default weapon
            Score = 0;
            Lives = 3;
            IsActive = true;

            this.useGamepad = useGamepad;
            this.gamepadId = gamepadId;

            // Set up keyboard controls based on player ID
            SetupControls(playerId);
        }

        /// <summary>
        /// Configures keyboard controls for each player.
        /// </summary>
        private void SetupControls(int playerId)
        {
            switch (playerId)
            {
                case 1: // Player 1 - WASD
                    thrustKey = KeyboardKey.W;
                    leftKey = KeyboardKey.A;
                    rightKey = KeyboardKey.D;
                    fireKey = KeyboardKey.Space;
                    break;
                case 2: // Player 2 - Arrow keys
                    thrustKey = KeyboardKey.Up;
                    leftKey = KeyboardKey.Left;
                    rightKey = KeyboardKey.Right;
                    fireKey = KeyboardKey.RightControl;
                    break;
                case 3: // Player 3 - IJKL
                    thrustKey = KeyboardKey.I;
                    leftKey = KeyboardKey.J;
                    rightKey = KeyboardKey.L;
                    fireKey = KeyboardKey.H;
                    break;
                case 4: // Player 4 - Numpad
                    thrustKey = KeyboardKey.Kp8;
                    leftKey = KeyboardKey.Kp4;
                    rightKey = KeyboardKey.Kp6;
                    fireKey = KeyboardKey.Kp0;
                    break;
                default:
                    thrustKey = KeyboardKey.W;
                    leftKey = KeyboardKey.A;
                    rightKey = KeyboardKey.D;
                    fireKey = KeyboardKey.Space;
                    break;
            }
        }

        /// <summary>
        /// Updates the player state, handling input and weapon cooldowns.
        /// </summary>
        public void Update(float deltaTime, BulletManager bulletManager, int screenWidth, int screenHeight)
        {
            if (!IsActive || !Ship.IsAlive) return;

            // Handle input
            if (useGamepad && Raylib.IsGamepadAvailable(gamepadId))
            {
                HandleGamepadInput(deltaTime, bulletManager);
            }
            else
            {
                HandleKeyboardInput(deltaTime, bulletManager);
            }

            // Update ship physics
            Ship.Update(deltaTime);
            Ship.WrapAroundScreen(screenWidth, screenHeight);

            // Update weapon
            CurrentWeapon.Update(deltaTime);
        }

        /// <summary>
        /// Handles keyboard input for player controls.
        /// </summary>
        private void HandleKeyboardInput(float deltaTime, BulletManager bulletManager)
        {
            if (Raylib.IsKeyDown(thrustKey))
            {
                Ship.Thrust(deltaTime);
            }

            if (Raylib.IsKeyDown(leftKey))
            {
                Ship.RotateLeft(deltaTime);
            }

            if (Raylib.IsKeyDown(rightKey))
            {
                Ship.RotateRight(deltaTime);
            }

            if (Raylib.IsKeyDown(fireKey))
            {
                Fire(bulletManager);
            }
        }

        /// <summary>
        /// Handles gamepad input for player controls.
        /// </summary>
        private void HandleGamepadInput(float deltaTime, BulletManager bulletManager)
        {
            // Left stick for movement and rotation
            float axisX = Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX);
            float axisY = Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftY);

            // Thrust with right trigger or A button
            if (Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.RightTrigger) > 0.1f ||
                Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceDown))
            {
                Ship.Thrust(deltaTime);
            }

            // Rotation with left stick
            if (MathF.Abs(axisX) > 0.2f)
            {
                if (axisX < 0)
                    Ship.RotateLeft(deltaTime * MathF.Abs(axisX));
                else
                    Ship.RotateRight(deltaTime * axisX);
            }

            // Fire with right bumper or B button
            if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceRight) ||
                Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightTrigger1))
            {
                Fire(bulletManager);
            }
        }

        /// <summary>
        /// Fires the current weapon.
        /// </summary>
        private void Fire(BulletManager bulletManager)
        {
            if (CurrentWeapon.CanFire)
            {
                Vector2 firePosition = Ship.Position + Ship.GetForwardDirection() * Ship.Radius;
                CurrentWeapon.Fire(firePosition, Ship.GetForwardDirection(), bulletManager);
            }
        }

        /// <summary>
        /// Draws the player's ship.
        /// </summary>
        public void Draw()
        {
            if (IsActive)
            {
                Ship.Draw();
            }
        }

        /// <summary>
        /// Adds points to the player's score.
        /// </summary>
        public void AddScore(int points)
        {
            Score += points;
        }

        /// <summary>
        /// Reduces player lives and respawns if lives remain.
        /// </summary>
        public void LoseLife(Vector2 respawnPosition)
        {
            Lives--;
            if (Lives > 0)
            {
                Ship.Position = respawnPosition;
                Ship.Velocity = Vector2.Zero;
                Ship.IsAlive = true;
            }
            else
            {
                Ship.IsAlive = false;
                IsActive = false;
            }
        }
    }
}

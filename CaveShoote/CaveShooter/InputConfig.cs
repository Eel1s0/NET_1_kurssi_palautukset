using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Holds keyboard key bindings for player input.
    /// </summary>
    public struct InputConfig
    {
        /// <summary>Key for thrusting/moving up.</summary>
        public KeyboardKey Up;

        /// <summary>Key for moving down (unused in current physics).</summary>
        public KeyboardKey Down;

        /// <summary>Key for rotating left.</summary>
        public KeyboardKey Left;

        /// <summary>Key for rotating right.</summary>
        public KeyboardKey Right;

        /// <summary>Key for firing weapons.</summary>
        public KeyboardKey Shoot;
    }
}
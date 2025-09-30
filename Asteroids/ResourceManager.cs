/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System;
using System.IO;
using System.Collections.Generic;
using Raylib_cs;

namespace Asteroids
{
    public static class ResourceManager
    {
        // Change this to your preferred centralized image folder
        public static string ImagesBasePath { get; set; } = Path.Combine("Images");

        // Track loaded textures so we can unload them later
        private static readonly List<global::Raylib_cs.Texture2D> loadedTextures = new();

        public static global::Raylib_cs.Texture2D LoadTexture(string filename)
        {
            // Try base path first, fallback to filename as given
            string path = Path.Combine(ImagesBasePath, filename);
            if (!File.Exists(path))
            {
                path = filename;
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Texture not found: {filename} (tried '{ImagesBasePath}' and raw filename)");
            }

            var tex = Raylib.LoadTexture(path);
            loadedTextures.Add(tex);
            return tex;
        }

        public static void UnloadAll()
        {
            // Unload all textures we've loaded. Some Raylib-cs versions may not expose internal fields,
            // so call UnloadTexture directly and ignore any exceptions.
            foreach (var t in loadedTextures)
            {
                try
                {
                    Raylib.UnloadTexture(t);
                }
                catch
                {
                    // Ignore unload failures (texture might already be unloaded or invalid)
                }
            }
            loadedTextures.Clear();
        }
    }
}
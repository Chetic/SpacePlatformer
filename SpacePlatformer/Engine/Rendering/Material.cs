using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using GameStateManagement.Screens;
using SpacePlatformer.Engine.Procedural;
using System.Xml.Serialization;

namespace SpacePlatformer.Engine.Rendering
{
    public class Material
    {
        public const int MaxLayers = 5;
        public Texture2D[] textureLayers = new Texture2D[MaxLayers];
        public Vector2 scale = new Vector2(1.0f, 1.0f); //Scales the material by this factor when it is rendered

        public Material()
        {
        }

        /// <summary>
        /// Creates a material and applies a texture to layer 1 of the material
        /// </summary>
        /// <param name="texture">The texture to apply</param>
        public Material(string textureName)
        {
            ApplyTexture(1, textureName);
        }

        /// <summary>
        /// Applies texture to specified layer of material
        /// </summary>
        /// <param name="layer">Layer index</param>
        /// <param name="textureName">The resource name of the texture to apply</param>
        public void ApplyTexture(uint layer, string textureName)
        {
            if (layer > Material.MaxLayers)
                throw new Exception("Attempted setting texture to layer which is out of bounds");
            
            textureLayers[layer] = GameplayScreen.content.Load<Texture2D>(textureName);
            textureLayers[layer].Name = textureName;
        }

        /// <summary>
        /// Applies texture to specified layer of material
        /// </summary>
        /// <param name="layer">Layer index</param>
        /// <param name="texture">The texture to apply</param>
        public void ApplyTexture(uint layer, Texture2D texture)
        {
            if (layer > Material.MaxLayers)
                throw new Exception("Attempted setting texture to layer which is out of bounds");

            textureLayers[layer] = texture;
        }

        /// <summary>
        /// Removes texture from specified layer of material
        /// </summary>
        /// <param name="layer">Layer index</param>
        public void RemoveTexture(uint layer)
        {
            if (layer > Material.MaxLayers)
                throw new Exception("Attempted removing texture from layer which is out of bounds");

            textureLayers[layer].Dispose();
        }

        /// <summary>
        /// Retrieves texture from specified layer of material
        /// </summary>
        /// <param name="layer">Layer index</param>
        public Texture2D GetTexture(uint layer)
        {
            if (layer > Material.MaxLayers)
                throw new Exception("Attempted retrieving texture from layer which is out of bounds");

            return textureLayers[layer];
        }

        /// <summary>
        /// Checks if specified material layer has texture applied
        /// </summary>
        /// <param name="layer">Layer index</param>
        public bool HasTexture(uint layer)
        {
            if (layer > Material.MaxLayers)
                throw new Exception("Attempted indexing texture layer out of range");

            return (textureLayers[layer] != null);
        }
    }
}

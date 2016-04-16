using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.GUI;
using GameStateManagement.Screens;

namespace SpacePlatformer.Engine.Rendering
{
    public enum RenderMode
    {
        ColorMap,
        DepthMap,
        NormalMap
    };

    static class EntityRendering
    {
        private static Effect selectedEntityEffect;
        private static int entityToRender, entityToRenderCount;
        
        internal static void Initialize()
        {
            selectedEntityEffect = GameplayScreen.content.Load<Effect>("Shaders\\SelectedEntity");
        }

        internal static void ResetRenderQueue()
        {
            entityToRender = 0;
            entityToRenderCount = EntityHandler.ActiveMap.entities.Count;
        }

        /// <summary>
        /// After ResetRenderQueue is called, returns the next entity to be rendered
        /// </summary>
        /// <returns>The next entity to be rendered, or null if all Entities have been iterated over</returns>
        internal static Entity GetNextRenderEntity()
        {
            //In case entities have been removed
            if (EntityHandler.ActiveMap.entities.Count < entityToRenderCount)
                entityToRenderCount = EntityHandler.ActiveMap.entities.Count;

            if (entityToRender < entityToRenderCount)
                return EntityHandler.ActiveMap.entities[entityToRender++];
            else
                return null;
        }

        internal static void RenderToWorld(Entity entity, RenderMode mode)
        {
            if (entity == null)
                return;

            if (entity is ParticleEmitter)
            {
                ParticleEmitter emitter = (ParticleEmitter)entity;

                for (int i = 0; i < emitter.particles.Length; i++)
                {
                    RenderToWorld((Entity)emitter.particles[i], mode);
                }
            }

            if (entity.children != null)
            {
                foreach (Entity child in entity.children)
                {
                    RenderToWorld(child, mode);
                }
            }

            if (entity.hidden == false && entity.material != null)
            {
                Texture2D texture;
                if (mode == RenderMode.DepthMap && entity.material.HasTexture((uint)3))
                    texture = entity.material.GetTexture((uint)3);
                else if (mode == RenderMode.NormalMap && entity.material.HasTexture((uint)4))
                    texture = entity.material.GetTexture((uint)4);
                else if (mode == RenderMode.ColorMap && entity.material.HasTexture((uint)1))
                    texture = entity.material.GetTexture((uint)1);
                else
                    return;

                if (entity == Cursor.tempEntity && mode == RenderMode.ColorMap)
                    selectedEntityEffect.CurrentTechnique.Passes[0].Apply();

                Renderer.SpriteBatch.Draw(
                    texture,
                    entity.body.Position * 64.0f,
                    null, Color.White, entity.body.Rotation,
                    new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),
                    entity.material.scale, SpriteEffects.None, 0.0f
                    );

                //This has to be done to avoid applying the selected entity effect to the rest of the rendering sequence
                //TODO: Get rid of duplicate code (SpriteBatch.Begin())
                if (entity == Cursor.tempEntity && mode == RenderMode.ColorMap)
                {
                    Renderer.SpriteBatch.End();
                    Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, ViewHandling.ViewMatrix);
                }
            }
        }
    }
}

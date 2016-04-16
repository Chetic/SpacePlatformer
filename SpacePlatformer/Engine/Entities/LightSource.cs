using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.Rendering;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;

namespace SpacePlatformer.Engine.Entities
{
    public class LightSource : Entity
    {
        public Color lightColor = Color.White;
        public float radius = 4096.0f, power = 16.0f;
        internal RenderTarget2D RenderTarget { get; private set; }
        public Vector2 LightAreaSize { get; set; }

        public LightSource()
            : base()
        {
        }

        public LightSource(Vector2 position, Material lightMaskMaterial)
            : base(position, new Vector2(1.0f, 1.0f), lightMaskMaterial, true)
        {
            startPosition = position;
            startStatic = true;
            startRotation = 0.0f;
            Init();
        }

        public void Init()
        {
            if (body != null)
                body.Dispose();
            body = BodyFactory.CreateCircle(EntityHandler.ActiveMap.world, size.X, 1.0f, startPosition);
            body.IsStatic = startStatic;
            body.Rotation = startRotation;
            body.CollidesWith = FarseerPhysics.Dynamics.Category.Cat2;
            int baseSize = 2 << (int)ShadowmapSize.Size256;
            LightAreaSize = new Vector2(baseSize);
            RenderTarget = new RenderTarget2D(Renderer.Device, baseSize, baseSize);
        }
    }
}

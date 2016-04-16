using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.GUI;
using SpacePlatformer.Engine.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

namespace SpacePlatformer.Engine.Rendering
{
    static class PrimitiveRendering
    {
        private static BasicEffect wireFrameEffect;

        internal static void Initialize()
        {
            wireFrameEffect = new BasicEffect(Renderer.Device);
            wireFrameEffect.VertexColorEnabled = true;
            wireFrameEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0, Renderer.Device.Viewport.Width,
                Renderer.Device.Viewport.Height, 0,
                0, 1);
        }

        internal static void DrawBoundingBoxOf(Entity entity)
        {
            wireFrameEffect.CurrentTechnique.Passes[0].Apply();
            Vector2 halvedSize = entity.size / 2.0f;
            Vector2 position = entity.body.Position / 64.0f;
            Vector2[] vec2vertices = new Vector2[]
            {
                position / 64.0f - halvedSize,
                position / 64.0f + new Vector2(halvedSize.X, -halvedSize.Y),
                position / 64.0f + halvedSize,
                position / 64.0f + new Vector2(-halvedSize.X, halvedSize.Y),
                position / 64.0f - halvedSize
            };

            VertexPositionColor[] vertices = new VertexPositionColor[vec2vertices.Length];
            for (int i = 0; i < vec2vertices.Length; i++)
            {
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateRotationZ(entity.body.Rotation));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateTranslation(new Vector3(entity.body.Position, 0.0f)));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateScale(64.0f));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], ViewHandling.ViewMatrix);
                vertices[i].Color = Color.White;
                vertices[i].Position = new Vector3(vec2vertices[i], 0.0f);
            }

            Renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
        }

        internal static void DrawWireFrameOf(Entity entity)
        {
            ConcaveMesh concaveMesh;
            if ((concaveMesh = entity as ConcaveMesh) != null)
            {
                DrawMesh(concaveMesh);
                return;
            }
            foreach (Fixture fixture in entity.body.FixtureList)
            {
                if (fixture.ShapeType == ShapeType.Circle)
                {
                    DrawCircle(entity.body.Position, entity.size.X, 16);
                }
                else
                {
                    DrawRectangle(entity.body.Position, entity.size, entity.body.Rotation);
                }
            }
        }

        private static void DrawRectangle(Vector2 position, Vector2 size, float rotation)
        {
            wireFrameEffect.CurrentTechnique.Passes[0].Apply();
            Vector2 halvedSize = size / 2.0f;
            Vector2[] vec2vertices = new Vector2[]
            {
                position / 64.0f - halvedSize,
                position / 64.0f + new Vector2(halvedSize.X, -halvedSize.Y),
                position / 64.0f + halvedSize,
                position / 64.0f + new Vector2(-halvedSize.X, halvedSize.Y),
                position / 64.0f - halvedSize
            };

            VertexPositionColor[] vertices = new VertexPositionColor[vec2vertices.Length];
            for (int i = 0; i < vec2vertices.Length; i++)
            {
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateRotationZ(rotation));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateTranslation(new Vector3(position, 0.0f)));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], Matrix.CreateScale(64.0f));
                vec2vertices[i] = Vector2.Transform(vec2vertices[i], ViewHandling.ViewMatrix);
                vertices[i].Color = Color.White;
                vertices[i].Position = new Vector3(vec2vertices[i], 0.0f);
            }

            Renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
        }

        private static void DrawCircle(Vector2 position, float radius, int points)
        {
            double angle = MathHelper.TwoPi / points;

            VertexPositionColorTexture[] pointList = new VertexPositionColorTexture[points + 1];
            // Initialize an array of indices of type short.
            short[] lineListIndices = new short[(points * 2)];

            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < points; i++)
            {
                lineListIndices[i * 2] = (short)(i + 1);
                lineListIndices[(i * 2) + 1] = (short)(i + 2);
            }

            lineListIndices[(points * 2) - 1] = 1;
            
            for (int i = 0; i <= points; i++)
            {
                Vector2 localPosition = new Vector2(
                                            (float)Math.Round(Math.Sin(angle * i), 4),
                                            (float)Math.Round(Math.Cos(angle * i), 4));
                localPosition += position;
                //localPosition = Vector2.Transform(localPosition, Matrix.CreateRotationZ(rotation));
                localPosition = Vector2.Transform(localPosition, Matrix.CreateTranslation(new Vector3(position/64.0f, 0.0f)));
                localPosition = Vector2.Transform(localPosition, Matrix.CreateScale(64.0f));
                localPosition = Vector2.Transform(localPosition, ViewHandling.ViewMatrix);
                pointList[i] = new VertexPositionColorTexture(
                    new Vector3(localPosition, 0.0f),
                    Color.White,
                    new Vector2());
            }

            Renderer.Device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                PrimitiveType.LineList,
                pointList,
                0,  // vertex buffer offset to add to each element of the index buffer
                pointList.Length,  // number of vertices in pointList
                lineListIndices,  // the index buffer
                0,  // first index element to read
                points   // number of primitives to draw
            );
        }

        private static void DrawMesh(ConcaveMesh meshEntity)
        {
            wireFrameEffect.CurrentTechnique.Passes[0].Apply();
            foreach (Vertices verts in meshEntity.GetVerticesList())
            {
                for (int i = 0; i < verts.Count - 1; i++)
                {
                    Vector2 from = verts[i];
                    from = Vector2.Transform(from, Matrix.CreateRotationZ(meshEntity.body.Rotation));
                    from = Vector2.Transform(from, Matrix.CreateTranslation(new Vector3(meshEntity.body.Position, 0.0f)));
                    from = Vector2.Transform(from, Matrix.CreateScale(64.0f));
                    from = Vector2.Transform(from, ViewHandling.ViewMatrix);

                    Vector2 to = verts[i + 1];
                    to = Vector2.Transform(to, Matrix.CreateRotationZ(meshEntity.body.Rotation));
                    to = Vector2.Transform(to, Matrix.CreateTranslation(new Vector3(meshEntity.body.Position, 0.0f)));
                    to = Vector2.Transform(to, Matrix.CreateScale(64.0f));
                    to = Vector2.Transform(to, ViewHandling.ViewMatrix);

                    DrawLine(from, to, Color.White);
                }
            }
        }

        internal static void DrawLine(Vector2 from, Vector2 to, Color lineColor)
        {
            VertexPositionColor[] currentLine = new VertexPositionColor[2];
            currentLine[0].Color = lineColor;
            currentLine[1].Color = lineColor;
            currentLine[0].Position = new Vector3(from, 0);
            currentLine[1].Position = new Vector3(to, 0);
            Renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, currentLine, 0, 1);
        }

        internal static void DrawRectangle(Vector2 screenPosition, Vector2 size)
        {
            float x = screenPosition.X;
            float y = screenPosition.Y;
            float w = size.X;
            float h = size.Y;

            VertexPositionColor[] currentLine = new VertexPositionColor[4];
            currentLine[0].Color = Color.White;
            currentLine[1].Color = Color.White;
            currentLine[2].Color = Color.White;
            currentLine[3].Color = Color.White;
            currentLine[0].Position = new Vector3(new Vector2(x, y), 0);
            currentLine[1].Position = new Vector3(new Vector2(x + w, y), 0);
            currentLine[2].Position = new Vector3(new Vector2(x, y + h), 0);
            currentLine[3].Position = new Vector3(new Vector2(x + w, y + h), 0);
            Renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, currentLine, 0, 2);
        }
    }
}

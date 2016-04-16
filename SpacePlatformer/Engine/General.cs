using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Entities;

namespace SpacePlatformer.Engine
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    static class GeneralHelper
    {
        public static Vector2 RotateVector(Vector2 vec, float radians)
        {
            return new Vector2(
                vec.X * (float)Math.Cos((float)radians) - vec.Y * (float)Math.Sin((float)radians),
                vec.X * (float)Math.Sin((float)radians) + vec.Y * (float)Math.Cos((float)radians));
        }

        public static void MoveByGrid(Vector2 position, float gridSize)
        {
            float x = position.X, y = position.Y;

            if (x < 0)
                x -= x % gridSize;
            else
                x -= x % gridSize - gridSize;
            if (y < 0)
                y -= y % gridSize;
            else
                y -= y % gridSize - gridSize;

            x -= gridSize / 2.0f;
            y -= gridSize / 2.0f;

            position = new Vector2(x, y);
        }

        public static void RotateByGrid(Entity tempEntity, float rotationGrid)
        {
            int multiplesOfGrid = (int)(tempEntity.body.Rotation / rotationGrid);
            float newAngle = (float)multiplesOfGrid * rotationGrid;
            newAngle += rotationGrid;
            float lastRotation = tempEntity.body.Rotation - newAngle;
            if (Math.Abs(lastRotation) == 0.0f)
            {
                newAngle += rotationGrid;
            }
            tempEntity.body.Rotation = newAngle;
        }
    }
}

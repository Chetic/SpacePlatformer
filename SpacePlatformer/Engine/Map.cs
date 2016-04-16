using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SpacePlatformer.Engine.Entities;
using GameStateManagement;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.Procedural;
using System.Xml;
using GameStateManagement.Screens;
using SpacePlatformer.Engine.GUI;
using System.Xml.Serialization;
using System.IO;

namespace SpacePlatformer.Engine
{
    public class Map
    {
        [XmlIgnore]
        public World world;
        public Player player { get { return (Player)entities[0]; } }
        public List<Entity> entities;
        public List<LightSource> lightSources;
        public float timescale = 1.0f;
        public bool Pause = false;

        public Map()
        {
            entities = new List<Entity>();
            lightSources = new List<LightSource>();
            world = new World(new Vector2(0.0f, 5.82f));
        }

        static public Map LoadMap(string filename)
        {
            XmlSerializer xmlSerializer;
            xmlSerializer = new XmlSerializer(typeof(Map));
            StreamReader streamReader = new StreamReader(filename);
            Map map = (Map)xmlSerializer.Deserialize(streamReader);
            streamReader.Close();

            return map;
        }

        public void SaveMap(string filename)
        {
            XmlSerializer xmlSerializer;
            xmlSerializer = new XmlSerializer(this.GetType());
            StreamWriter streamWriter = new StreamWriter(filename, false);
            xmlSerializer.Serialize(streamWriter, this);
            streamWriter.Close();
        }
         
        public void Update(GameTime gameTime)
        {
            if (Pause)
                timescale = 0.0f;
            float stepTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * timescale;

            EntityHandler.Update(stepTime);

            ViewHandling.UpdateCamera(player.body.Position);
        }
    }
}

﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.GameWorld
{
    public class World
    {
        public bool GameOver = false; // Corner cut: should this be part of the world?

        public double XpMultiplier = 1.0;
        public double SpeedMultiplier = 1.0;
        public double RarityMultiplier = 1.0;
        public double QuantityMultiplier = 1.0;

        public AreaKingdomDescription AreaKingdom { get; private set; } = new();

        public WorldMap Map = new();

        public Entity GlobalEntity = new(); // Stores character-independent data, that needs to be saved regularly (e.g. achievement status)

        public World()
        {
            Map.Dungeons = AreaKingdom.Dungeons;
        }

        public ZoneDescription? GetZone(Location location)
        {
            return Map.GetZone(location);
        }

        public ZoneDescription? GetDungeonZone(string dungeonId, int dungeonLevel, int floor)
        {
            return Map.GetDungeonZone(dungeonId, dungeonLevel, floor);
        }
    }
}

using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.DataAccess
{
    public class BasicPlayerEntityConverter : IEntityConverter
    {
        public Entity? DeserializeEntity(string serialized)
        {
            Entity result = new();
            var success = DeserializeEntity(serialized, out result);
            return success ? result : null;
        }

        public bool DeserializeEntity(string serialized, out Entity entity)
        {
            entity = new();
            try
            {                
                var description = JsonSerializer.Deserialize<PlayerDescription>(serialized);
                if (description == null)
                    return false;

                Entity player = PlayerFactory.MakeNewPlayer(description.Name);
                player.GetComponent<NameComponent>()!.Name = description.Name;
                player.GetComponent<LevelComponent>()!.Level = description.Level;
                player.GetComponent<XpGainerComponent>()!.Current = description.XP;

                entity = player;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string SerializeEntity(Entity entity)
        {
            PlayerDescription description = new(entity);
            return JsonSerializer.Serialize(description);            
        }

        internal class PlayerDescription
        {
            public string Name { get; set; }

            public int Level { get; set; }

            public int XP { get; set; }

            public PlayerDescription()
            {
                Name = "";
                Level = 1;
                XP = 0;
            }

            public PlayerDescription(Entity player)
            {
                Name = player.GetComponent<NameComponent>()!.Name;
                Level = player.GetComponent<LevelComponent>()!.Level;
                XP = player.GetComponent<XpGainerComponent>()!.Current;
            }
        }
    }
}

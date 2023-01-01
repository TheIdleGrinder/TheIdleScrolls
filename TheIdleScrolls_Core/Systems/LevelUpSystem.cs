using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class LevelUpSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var entity in coordinator.GetEntities<LevelComponent, XpGainerComponent>())
            {
                var lvlComp = entity.GetComponent<LevelComponent>() ?? throw new Exception("Missing Level component");
                var xpComp = entity.GetComponent<XpGainerComponent>() ?? throw new Exception("Missing XP Gainer component");

                var lvl = lvlComp.Level;
                var target = xpComp.TargetFunction(lvl);
                while (xpComp.Current >= target)
                {
                    lvlComp.IncreaseLevel();
                    xpComp.Current -= target;
                    target = xpComp.TargetFunction(++lvl);
                    coordinator.PostMessage(this, new LevelUpMessage(entity));
                }
            }
        }

        public class LevelUpMessage : IMessage
        {
            public Entity Character { get; set; }

            public LevelUpMessage(Entity character)
            {
                Character = character;
            }

            string IMessage.BuildMessage()
            {
                var lvl = Character.GetComponent<LevelComponent>()?.Level ?? 0;
                return $"{Character.GetName()} reached level {lvl}";
            }
        }
    }
}

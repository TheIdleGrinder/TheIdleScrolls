using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.DataAccess
{
    public interface IEntityConverter
    {
        public string SerializeEntity(Entity entity);

        public Entity? DeserializeEntity(string serialized);

        public bool DeserializeEntity(string serialized, out Entity entity);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.DataAccess
{
    public record CharacterMetaData(string Name, int Level, string Class);

    public static class CharacterMetaDataReader
    {
        public static async Task<CharacterMetaData?> GetCharacterMetaData(this DataAccessHandler accessHandler, string characterId)
        {
            var entity = await accessHandler.LoadEntity(characterId);
            if (entity == null)
                return null;
            
            return new(
                entity.GetName(), 
                entity.GetComponent<LevelComponent>()?.Level ?? 0, 
                PlayerFactory.GetCharacterClass(entity).Localize()
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.DataAccess
{
    public record CharacterMetaData(string Name, string TitledName, string TitlePrefix, int Level, string Class);

    public static class CharacterMetaDataReader
    {
        public static async Task<CharacterMetaData?> GetCharacterMetaData(this DataAccessHandler accessHandler, string characterId)
        {
            try
            {
                var encrypted = await accessHandler.StorageHandler.LoadData(characterId);
                var decrypted = accessHandler.Decrypt(encrypted);

                var metaDataComp = accessHandler.EntityConverter
                                    .DeserializeComponentFromSerializedEntity<MetaDataComponent>(decrypted);

                if (metaDataComp is not null)
                {
                    return new(
                        metaDataComp.Name,
                        metaDataComp.NameWithSuffixTitle,
                        metaDataComp.PrefixTitle,
                        metaDataComp.Level,
                        metaDataComp.DisplayClass.Localize()
                    );
                }
                else
                {
                    var entity = accessHandler.EntityConverter.DeserializeEntity(decrypted);
                    if (entity is not null)
                    {
                        return new(
                            entity.GetName(),
                            entity.GetTitledName(false, true),
                            entity.GetComponent<TitleBearerComponent>()?.GetPrefixTitle() ?? "",
                            entity.GetLevel(),
                            PlayerFactory.GetCharacterClass(entity).Localize()
						);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to load {characterId}");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to get character meta data: {e.Message}");
                return null;
            }
        }
    }
}

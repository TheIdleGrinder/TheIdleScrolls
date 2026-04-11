using MiniECS;

namespace TheIdleScrolls_Core.Modifiers
{
    public interface IPerkCondition
    {
        string Description => GetDescription();

        bool IsSatisfied(Entity entity);

        string GetDescription();
    }
}

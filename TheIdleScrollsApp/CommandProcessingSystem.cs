using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class CommandProcessingSystem : AbstractSystem, IUserInputHandler
    {
        MainWindow m_mainWindow;
        GameRunner m_runner;

        List<ItemMove> m_commands = new();
        List<IMessage> m_requests = new();

        public CommandProcessingSystem(MainWindow mainWindow, GameRunner runner)
        {
            m_mainWindow = mainWindow;
            m_runner = runner;

            runner.AddSystem(this);
            runner.AddSystem(new UpdateMainWindowSystem(mainWindow));
        }

        public void EquipItem(uint playerId, uint itemId)
        {
            m_commands.Add(new ItemMove(playerId, itemId, true));
        }

        public void SetAutoProceed(bool autoProceed)
        {
            m_requests.Add(new AutoProceedRequest(autoProceed));
        }

        public void TravelToArea(int areaLevel)
        {
            m_requests.Add(new TravelRequest(areaLevel));
        }

        public void UnequipItem(uint playerId, uint itemId)
        {
            m_commands.Add(new ItemMove(playerId, itemId, false));
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            List<ItemMove> processed = new();
            foreach (var move in m_commands)
            {
                coordinator.PostMessage(this, new ItemMoveRequest(move.PlayerId, move.ItemId, move.Equip));
                processed.Add(move);
            }
            processed.ForEach(m => m_commands.Remove(m));

            foreach (var request in m_requests)
            {
                coordinator.PostMessage(this, request as dynamic);
            }
            m_requests.Clear();
        }

        class ItemMove
        {
            public uint PlayerId { get; set; }
            public uint ItemId { get; set; }
            public bool Equip { get; set; }

            public ItemMove(uint player, uint item, bool equip)
            {
                PlayerId = player;
                ItemId = item;
                Equip = equip;
            }
        }
    }
}

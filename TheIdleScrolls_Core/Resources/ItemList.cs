using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Resources
{
    internal static class ItemList
    {
        static readonly List<ItemFamilyDescription> s_items = GenerateItems();

        public static List<ItemFamilyDescription> Items { get { return s_items; } }

        static List<ItemFamilyDescription> GenerateItems()
        {
            return new()
            {
                new("SBL",
                    new()
                    {
                        new(new("Hand"), new( 4.0, 1.0 ), null, 0, new() { "MAT_SIMPLE" }),
                        new(new("Hand"), new( 5.0, 0.75), null, 10, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand"), new( 8.0, 1.05), null, 20, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand"), new( 8.0, 0.95), null, 30, new() { "MAT_M1", "MAT_M2" }),
                    }                    
                ),
                new("LBL",
                    new()
                    {
                        new(new("Hand Hand"),   new( 7.0, 1.4 ), null,  0, new() { "MAT_SIMPLE" }),
                        new(new("Hand"),        new(11.0, 1.5 ), null, 10, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand Hand"),   new(18.0, 1.85), null, 20, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand"),        new(18.0, 1.65), null, 30, new() { "MAT_M1", "MAT_M2" }),
                    }
                ),
                new("AXE",
                    new()
                    {
                        new(new("Hand"),        new( 5.0, 1.25), null,  0, new() { "MAT_SIMPLE" }),
                        new(new("Hand"),        new( 9.0, 1.25), null, 10, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand Hand"),   new(18.0, 1.85), null, 20, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand"),        new(12.0, 1.35), null, 30, new() { "MAT_M1", "MAT_M2" }),
                    }
                ),
                new("BLN",
                    new()
                    {
                        new(new("Hand"),        new( 7.0, 1.65), null,  0, new() { "MAT_SIMPLE" }),
                        new(new("Hand"),        new(10.0, 1.4 ), null, 10, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand Hand"),   new(19.0, 1.95), null, 20, new() { "MAT_M0", "MAT_M1", "MAT_M2" }),
                        new(new("Hand"),        new(15.0, 1.65), null, 30, new() { "MAT_M1", "MAT_M2" }),
                    }
                ),
                new("POL",
                    new()
                    {
                        new(new("Hand Hand"),   new(11.0, 2.1 ), null,  0, new() { "MAT_SIMPLE" }),
                        new(new("Hand Hand"),   new(20.0, 2.15), null, 10, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                        new(new("Hand Hand"),   new(13.0, 1.4 ), null, 20, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                        new(new("Hand Hand"),   new(26.0, 2.3 ), null, 30, new() { "MAT_W1", "MAT_W2" }),
                    }
                ),
                new("LAR",
                    new()
                    {
                        new(new("Chest", 6.0), null, new(10.0, 0.0),  0, new() { "MAT_SIMPLE" }),
                        new(new("Chest", 6.0), null, new(14.0, 0.0), 15, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Head",  4.0), null, new(10.0, 0.0), 16, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Arms",  2.5), null, new( 8.0, 0.0), 13, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Legs",  2.5), null, new( 8.0, 0.0), 14, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Hand",  3.0), null, new(10.0, 0.0), 17, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                        new(new("Chest", 6.0), null, new(16.0, 0.0), 25, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Head",  4.0), null, new(12.0, 0.0), 26, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Arms",  2.5), null, new(10.0, 0.0), 23, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Legs",  2.5), null, new(10.0, 0.0), 24, new() { "MAT_L0", "MAT_L1", "MAT_L2" }),
                        new(new("Hand",  3.0), null, new(12.0, 0.0), 27, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                    }
                ),
                new("HAR",
                    new()
                    {
                        new(new("Chest",12.0), null, new(16.0, 0.0),  0, new() { "MAT_SIMPLE" }),
                        new(new("Chest",12.0), null, new(22.0, 0.0), 15, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Head",  8.0), null, new(16.0, 0.0), 16, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Arms",  5.0), null, new(13.0, 0.0), 13, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Legs",  5.0), null, new(13.0, 0.0), 14, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Hand",  6.0), null, new(16.0, 0.0), 17, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                        new(new("Chest",12.0), null, new(26.0, 0.0), 25, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Head",  8.0), null, new(19.0, 0.0), 26, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Arms",  5.0), null, new(16.0, 0.0), 23, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Legs",  5.0), null, new(16.0, 0.0), 24, new() { "MAT_H0", "MAT_H1", "MAT_H2" }),
                        new(new("Hand",  6.0), null, new(19.0, 0.0), 27, new() { "MAT_W0", "MAT_W1", "MAT_W2" }),
                    }
                )
            };
        }
    }
}

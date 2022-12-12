﻿// See https://aka.ms/new-console-template for more information
using ConsoleRunner;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Storage;

const int TimePerTick = 250;

Console.Write("Player Name: ");
string? playerName = Console.ReadLine();
if (playerName == null || playerName == "")
    playerName = "Leeroy";

var dataHandler = new DataAccessHandler(new EntityJsonConverter(new ItemFactory()), new BasicFileStorageHandler());
var runner = new GameRunner(dataHandler);
runner.Initialize(playerName);
runner.SetAppInterface(new ConsoleUpdater());

var startTime = DateTime.Now;
var tickStart = startTime;
var tickEnd = startTime;

long runTime = 0;

while (true)
{
    var lastTickDuration = (tickEnd - tickStart).TotalMilliseconds;
    tickStart = DateTime.Now;

    runner.ExecuteTick(lastTickDuration / 1000);

    var tickDuration = (DateTime.Now - tickStart).Milliseconds;
    var sleepTime = Math.Max(TimePerTick - tickDuration, 0);
    runTime += (tickDuration + sleepTime);
    //Console.WriteLine($"Last tick: {(DateTime.Now - tickStart).TotalMilliseconds}");

    System.Threading.Thread.Sleep(sleepTime);
    tickEnd = DateTime.Now;
}
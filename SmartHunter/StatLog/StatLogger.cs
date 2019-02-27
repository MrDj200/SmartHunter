﻿using System.Collections.Generic;
using SmartHunter.Game.Data;
using SmartHunter.Core;
using Newtonsoft.Json;
using System;
using System.IO;
using SmartHunter.Game.Helpers;
using System.Linq;

namespace SmartHunter.StatLog
{
    static class StatLogger
    {
        public static List<Monster> MonsterList { get; set; } = new List<Monster>();
        public static bool MissionInProgress { get; set; } = false;

        public static long LastStamp { get; private set; } = 0;
        public static DataObject LastData { get; private set; } = null;
        public static void InitLogger()
        {
            if (StatObject.Exists)
            {
                return;
            }
            StatObject.Init();

            // Subscribing to events
            MhwHelper.OnMissionStart += OnMissionStart;
            MhwHelper.OnMissionEnd += OnMissionEnd;

            StatObject.Exists = true;
            StatObject.Clear();
            StatObject.Instance.Location = "WIP"; // TODO: Somehow get the Location into this :thinkingEmoji:
            LastStamp = 0;            
        }

        private static void OnMissionStart(List<Player> updatedPlayers)
        {
            long stamp = Utils.GetUnixTimeStamp(); // Get Current Time in unix format
            if (!StatObject.Exists || stamp <= LastStamp) // If it's not logging, or the last log is younger than a second, do nothing
            {
                return; // Our exit Strategy
            }

            if (MonsterList.Count <= 0)
            {
                return;
            }

            if (!MissionInProgress)
            {
                MissionInProgress = true;
                Log.WriteLine("Stat Logging Started!");
            }

            long timeDif = stamp - LastStamp;

            List<StatMonster> _monsters = new List<StatMonster>();
            #region MonsterList
            _monsters.Clear(); // Safety measure
            foreach (Monster mnst in MonsterList)
            {
                StatMonster _temp = new StatMonster
                {
                    MonsterName = mnst.Name,
                    MonsterHP = mnst.Health.Current,
                    MonsterHPMax = mnst.Health.Max,
                    MonsterCrown = mnst.Crown
                };

                _monsters.Add(_temp);
            }
            #endregion
            List<StatPlayer> _players = new List<StatPlayer>();
            #region PlayerList
            _players.Clear(); // Safety measure
            foreach (Player ply in updatedPlayers)
            {
                int lastTotalDmg = 0;
                if ( LastData != null )
                {
                    StatPlayer shit = LastData.Players.Where(p => p.PlayerName == ply.Name).ToList<StatPlayer>().FirstOrDefault();
                    if ( shit != null )
                    {
                        // TODO: rename
                        lastTotalDmg = shit.PlayerTotalDmg;
                    }
                    
                }
                int _tempDps = (int)(((ply.Damage - lastTotalDmg) / timeDif));
                if(_tempDps < 0)
                {
                    _tempDps = 0;
                }
                StatPlayer _temp = new StatPlayer
                {
                    PlayerName = ply.Name,
                    PlayerTotalDmg = ply.Damage,
                    PlayerDps = _tempDps // TODO: Confirm this 
                };
                _players.Add(_temp);
            }
            #endregion

            DataObject _tempObj = new DataObject()
            {
                TimeStamp = stamp,
                Monsters = _monsters,
                Players = _players
            };

            StatObject.Instance.DataObject.Add(_tempObj);

            LastData = _tempObj;
            LastStamp = stamp; // Update the old stamp to be ready for the next one
            return;
        }

        private static void OnMissionEnd()
        {
            if (!StatObject.Exists)
            {
                return; // More Safety measures
            }
            MissionInProgress = false;

            string dir = "data";
            Directory.CreateDirectory(dir); // Create the dir if it does not exist yet

            if(LastStamp != 0)
            {
                JsonSerialization.WriteToJsonFile<StatObject>($"{dir}\\{LastStamp}.json", StatObject.Instance, false);
                Log.WriteLine("Stat Logging Stopped!");
            }
            else
            {
                Log.WriteLine("Stat Logging Aborted!");
            }
            
            StatObject.Clear(); // Clearing the object so it'll be empty when the next mission starts
            
        }
    }

}

﻿using SmartHunter.Core.Config;
using System.Text.RegularExpressions;

namespace SmartHunter.Game.Config
{
    public class MonsterWidgetConfig : WidgetConfig
    {
        // em[0-9]|ems[0-9]|gm[0-9]
        public string IncludeMonsterIdRegex = "em[0-9]";
        public bool ShowUnchangedMonsters = true;
        public float HideMonstersAfterSeconds = 9999;
        public bool ShowUnchangedParts = false;
        public float HidePartsAfterSeconds = 20f;
        public bool ShowUnchangedStatusEffects = false;
        public float HideStatusEffectsAfterSeconds = 20f;

        public bool ShowHealthBar = true;
        public bool ShowHealth = true;
        public bool ShowCrown = true;
        public bool ShowRemovableParts = true;
        public bool ShowParts = true;
        public bool ShowStatusEffects = true;

        public MonsterWidgetConfig(float x, float y) : base(x, y)
        {
        }

        public bool MatchIncludeMonsterIdRegex(string monsterId)
        {
            return new Regex(IncludeMonsterIdRegex).IsMatch(monsterId);
        }
    }
}

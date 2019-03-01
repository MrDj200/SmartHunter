﻿using SmartHunter.Core.Data;
using SmartHunter.Game.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartHunter.Game.Data.WidgetContexts
{
    public class MonsterWidgetContext : WidgetContext
    {
        public ObservableCollection<Monster> Monsters { get; private set; }

        bool m_ShowHealthBar = true;
        public bool ShowHealthBar
        {
            get { return m_ShowHealthBar; }
            set { SetProperty(ref m_ShowHealthBar, value); }
        }

        bool m_ShowHealth = true;
        public bool ShowHealth
        {
            get { return m_ShowHealth; }
            set { SetProperty(ref m_ShowHealth, value); }
        }

        bool m_ShowCrown = true;
        public bool ShowCrown
        {
            get { return m_ShowCrown; }
            set { SetProperty(ref m_ShowCrown, value); }
        }

        bool m_ShowRemovableParts = true;
        public bool ShowRemovableParts
        {
            get { return m_ShowRemovableParts; }
            set { SetProperty(ref m_ShowRemovableParts, value); }
        }

        bool m_ShowParts = true;
        public bool ShowParts
        {
            get { return m_ShowParts; }
            set { SetProperty(ref m_ShowParts, value); }
        }

        bool m_ShowStatusEffects = true;
        public bool ShowStatusEffects
        {
            get { return m_ShowStatusEffects; }
            set { SetProperty(ref m_ShowStatusEffects, value); }
        }

        public MonsterWidgetContext()
        {
            Monsters = new ObservableCollection<Monster>();

            UpdateFromConfig();
        }

        public Monster UpdateAndGetMonster(ulong address, string id, float maxHealth, float currentHealth, float sizeScale)
        {
            Monster monster = null;

            monster = Monsters.FirstOrDefault(existingMonster => existingMonster.Address == address);
            if (monster != null)
            {
                monster.Id = id;
                monster.Health.Max = maxHealth;
                monster.Health.Current = currentHealth;
                monster.SizeScale = sizeScale;
            }
            else
            {
                monster = new Monster(address, id, maxHealth, currentHealth, sizeScale);
                Monsters.Add(monster);
            }

            monster.UpdateVisibility();

            return monster;
        }

        public override void UpdateFromConfig()
        {
            base.UpdateFromConfig();

            ShowHealthBar = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowHealthBar;
            ShowHealth = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowHealth;
            ShowCrown = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowCrown;
            ShowRemovableParts = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowRemovableParts;
            ShowParts = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowParts;
            ShowStatusEffects = ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowStatusEffects;

            foreach (var monster in Monsters)
            {
                monster.NotifyPropertyChanged(nameof(Monster.IsVisible));
            }
        }
    }
}

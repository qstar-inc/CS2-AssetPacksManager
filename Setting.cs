﻿using System;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace AssetImporter
{
    [FileLocation(nameof(AssetImporter))]
    [SettingsUIGroupOrder(kSettingsGroup, kActionsGroup)]
    [SettingsUIShowGroupName(kSettingsGroup, kActionsGroup)]
    public class Setting : ModSetting
    {

        public const string kSection = "Main";
        public const string kSettingsGroup = "Settings";
        public const string kActionsGroup = "Actions";
        public static Setting instance;
        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUIHidden]
        public bool HiddenSetting { get; set; }

        [SettingsUISection(kSection, kSettingsGroup)]
        public bool DisableAssetUpdates { get; set; }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kActionsGroup)]
        public bool DeleteImportedAssets
        {
            set { Mod.DeleteImportedAssets(); }
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kActionsGroup)]
        public bool ReimportAssets
        {
            set { Mod.CopyFromMods(true); }
        }

        public override void SetDefaults()
        {
            HiddenSetting = true;
            DisableAssetUpdates = false;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                {m_Setting.GetSettingsLocaleID(), nameof(AssetImporter)},
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSettingsGroup), "Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGroup), "Actions" },

                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableAssetUpdates)), "Disable Asset Updates"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableAssetUpdates)),
                    $"Disables overwriting of assets if the same asset is importing again. This will prevent the asset from being updated/patched and should be used for compatibility only."
                },

                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteImportedAssets)), "Delete all imported Assets"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteImportedAssets)),
                    $"Deletes all locally imported assets. This action cannot be undone and WILL break your save game if used improperly."
                },
                {
                    m_Setting.GetOptionWarningLocaleID(nameof(Setting.DeleteImportedAssets)),
                    $"Are you sure to delete the CustomAssets folder? This action is irreversible and will break your save game if used improperly. Please make sure to backup your game before proceeding.\n\nYour active asset packs will be reinstalled after restarting the game."
                },

                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.ReimportAssets)), "Re-Import subscribed assets"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ReimportAssets)),
                    $"Copies all the downloaded asset packs to your game directory again. This will overwrite any existing assets"
                },
                {
                    m_Setting.GetOptionWarningLocaleID(nameof(Setting.ReimportAssets)),
                    $"Are you sure you want to re-import all asset packs?"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
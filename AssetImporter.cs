﻿using System;
using System.IO;
using Colossal;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
using JetBrains.Annotations;

namespace AssetsLoader
{
    public class AssetsLoader : IMod
    {
        public static readonly ILog Logger = LogManager.GetLogger($"{nameof(global::AssetsLoader)}.{nameof(AssetsLoader)}")
            .SetShowsErrorsInUI(false);

        [CanBeNull] public string ModPath { get; set; }

        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Logger.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                Logger.Info($"Current mod asset at {asset.path}");
                ModPath = Path.GetDirectoryName(asset.path);
            }

            //var dir = "C:/Users/" + Environment.UserName + "/AppData/LocalLow/Colossal Order/Cities Skylines II/StreamingAssets~";
            var dir = "C:/Users/" + Environment.UserName + "/Desktop/assets";
            LoadFromDirectory(dir);
            //LoadFromInstalledMods();
            
            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(global::AssetsLoader), m_Setting, new Setting(this));
        }

        private void LoadFromDirectory(string assetsDir)
        {
            foreach (var file in new DirectoryInfo(assetsDir).GetFiles("*.Prefab"))
            {
                var hash = Hash128.Parse(File.ReadAllText(file.FullName + ".cid"));
                AssetDatabase.game.AddAsset<PrefabAsset>(AssetDataPath.Create(file.FullName), hash);
                Logger.Info("Loaded " + file.FullName + " with hash " + hash);
            }
        }

        private void LoadFromInstalledMods()
        {
            foreach (var modInfo in GameManager.instance.modManager)
            {
                if (modInfo.asset.isEnabled)
                {
                    var modDir = Path.GetDirectoryName(modInfo.asset.path);
                    if (modDir == null)
                    {
                        continue;
                    }

                    var assetsDir = Path.Combine(modDir, "assets");
                    if (Directory.Exists(assetsDir))
                    {
                        Logger.Info($"Load \"{modInfo.name}\"'s assets.");
                        LoadFromDirectory(assetsDir);
                    }
                }
            }
        }

        public void OnDispose()
        {
            Logger.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
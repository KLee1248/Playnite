﻿using EpicLibrary.Models;
using EpicLibrary.Services;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EpicLibrary
{
    public class EpicLibrary : LibraryPlugin
    {        
        private ILogger logger = LogManager.GetLogger();
        private readonly IPlayniteAPI playniteApi;
        private const string dbImportMessageId = "epiclibImportError";
        internal readonly string TokensPath;
        internal readonly EpicLibrarySettings LibrarySettings;

        public EpicLibrary(IPlayniteAPI api) : base(api)
        {
            playniteApi = api;
            LibrarySettings = new EpicLibrarySettings(this, api);
            TokensPath = Path.Combine(GetPluginUserDataPath(), "tokens.json");
        }

        internal Dictionary<string, GameInfo> GetInstalledGames()
        {
            var games = new Dictionary<string, GameInfo>();
            foreach (var app in EpicLauncher.GetInstalledAppList())
            {
                if (app.AppName.StartsWith("UE_"))
                {
                    continue;
                }            

                var game = new GameInfo()
                {
                    Source = "Epic",
                    GameId = app.AppName,
                    Name = Path.GetFileName(app.InstallLocation),
                    InstallDirectory = app.InstallLocation,
                    IsInstalled = true,
                    PlayAction = new GameAction()
                    {
                        Type = GameActionType.URL,
                        Path = string.Format(EpicLauncher.GameLaunchUrlMask, app.AppName),
                        IsHandledByPlugin = true
                    }
                };

                games.Add(game.GameId, game);
            }

            return games;
        }

        internal List<GameInfo> GetLibraryGames()
        {
            var games = new List<GameInfo>();
            var accountApi = new EpicAccountClient(playniteApi, TokensPath);
            var assets = accountApi.GetAssets();
            if (!assets?.Any() == true)
            {
                logger.Warn("Found no assets on Epic accounts.");
            }
            
            foreach (var gameAsset in assets.Where(a => a.@namespace != "ue"))
            {
                var catalogItem = accountApi.GetCatalogItem(gameAsset.@namespace, gameAsset.catalogItemId);
                if (catalogItem?.categories?.Where(a => a.path == "applications").Any() != true)
                {
                    continue;
                }

                games.Add(new GameInfo()
                {
                    Source = "Epic",
                    GameId = gameAsset.appName,
                    Name = catalogItem.title,
                });                
            }

            return games;
        }

        #region ILibraryPlugin

        public override LibraryClient Client => new EpicClient();

        public override string Name => "Epic";

        public override string LibraryIcon => EpicLauncher.Icon;

        public override Guid Id => Guid.Parse("00000002-DBD1-46C6-B5D0-B1BA559D10E4");

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return LibrarySettings;
        }

        public override UserControl GetSettingsView(bool firstRunView)
        {
            return new EpicLibrarySettingsView();
        }

        public override IGameController GetGameController(Game game)
        {
            return new EpicGameController(game, playniteApi);
        }

        public override IEnumerable<GameInfo> GetGames()
        {
            var allGames = new List<GameInfo>();
            var installedGames = new Dictionary<string, GameInfo>();
            Exception importError = null;

            if (LibrarySettings.ImportInstalledGames)
            {
                try
                {
                    installedGames = GetInstalledGames();
                    logger.Debug($"Found {installedGames.Count} installed Epic games.");
                    allGames.AddRange(installedGames.Values.ToList());
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to import installed Epic games.");
                    importError = e;
                }
            }

            if (LibrarySettings.ImportUninstalledGames)
            {
                try
                {
                    var uninstalled = GetLibraryGames();
                    logger.Debug($"Found {uninstalled.Count} library Epic games.");

                    foreach (var game in uninstalled)
                    {
                        if (installedGames.TryGetValue(game.GameId, out var installed))
                        {
                            installed.Playtime = game.Playtime;
                            installed.LastActivity = game.LastActivity;
                            installed.Name = game.Name;
                        }
                        else
                        {
                            allGames.Add(game);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to import uninstalled Epic games.");
                    importError = e;
                }
            }

            if (importError != null)
            {
                playniteApi.Notifications.Add(
                    dbImportMessageId,
                    string.Format(playniteApi.Resources.GetString("LOCLibraryImportError"), Name) +
                    System.Environment.NewLine + importError.Message,
                    NotificationType.Error);
            }
            else
            {
                playniteApi.Notifications.Remove(dbImportMessageId);
            }

            return allGames;
        }

        #endregion ILibraryPlugin
    }
}

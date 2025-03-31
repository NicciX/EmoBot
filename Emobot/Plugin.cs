using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;

using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;

using Lumina.Excel.Sheets;

using FFXIVClientStructs;
using System.Runtime.InteropServices;

using XivCommon;

using Emobot.Windows;
using Emobot.Game;
using Emobot.Constants;
using Emobot.Chat;
using Emobot.Ui;
using Lumina.Data.Parsing;

namespace Emobot;

public class Plugin : IDalamudPlugin
{
    public const string PluginName = "Emobot";
    public const string Prefix = "Emobot";

    private bool disposed = false;


    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CmdManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] public static ISigScanner Scanner { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; } = null!;

    [PluginService] internal static IGameInteropProvider Interop { get; private set; } = null!;

    //public static ServerChat ServerChat { get; private set; } = null!;
    internal static ServerChat ServerChat { get; private set; } = null!;
    internal static PluginCommandManager CommandManager { get; private set; } = null!;

    private readonly WindowSystem windowSystem;

    public const string Name = "EmoBot";
    private const string CommandName = "/emo";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Emobot");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public XivChatType GeneralChatType { get; set; } = XivChatType.Debug;



    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var emoImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "lips.png");

        ServerChat = new(Scanner);

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, emoImagePath);



        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager = new(this)
        {
            ErrorHandler = ChatUtil.ShowPrefixedError
        };
        CommandManager.AddCommandHandlers();

        this.windowSystem = new(this.GetType().Namespace!);
        this.helpWindows = CommandManager.Commands.ToDictionary(cmd => cmd.CommandComparable, cmd => new HelpWindow(cmd) as Window);
        this.helpWindows.Add("<PLUGIN>", new HelpWindow(
            "Basics",
            PluginName,
            "the plugin itself",
            "Basic information about how commands work",
            $"{PluginName} uses a custom command parser that accepts single-character boolean flags starting with a hyphen."
            + "These flags can be bundled into one argument, such that \"-va\" will set both the \"v\" and \"a\" flags, just like \"-av\" will.\n"
            + "\n"
            + "All commands accept \"-h\" to display their built-in help.\n"
            + "\n"
            + "To list all commands, you can use \"/tinycmds\", optionally with \"-a\" to also list their aliases."
            + " Be aware that this list may be a little long. It's also not really sorted."
        ));
        this.helpWindows.Add("<LIST>", new CommandListWindow(this));

        foreach (Window wnd in this.helpWindows.Values)
            this.windowSystem.AddWindow(wnd);

        PluginInterface.UiBuilder.Draw += this.windowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"{PluginInterface.Manifest.Name} is loaded and ready to jack!!");
    }

    internal static Vector2 WorldToMap(Vector3 pos, Map zone) => WorldToMap(new Vector2(pos.X, pos.Z), zone);
    internal static Vector2 WorldToMap(Vector2 pos, Map zone)
    {
        Vector2 raw = MapUtil.WorldToMap(pos, zone);
        return new((int)MathF.Round(raw.X * 10, 1) / 10f, (int)MathF.Round(raw.Y * 10, 1) / 10f);
    }
    internal static Vector2 MapToWorld(Vector2 pos, Map zone)
    {
        MapLinkPayload maplink = new(zone.TerritoryType.Value.RowId, zone.RowId, pos.X, pos.Y);
        return new(maplink.RawX / 1000f, maplink.RawY / 1000f);
    }

    #region IDisposable Support
    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
            return;
        this.disposed = true;

        if (disposing)
        {
            //Common?.Dispose();
            CommandManager.Dispose();
            PluginInterface.UiBuilder.Draw -= this.windowSystem.Draw;
            this.windowSystem.RemoveAllWindows();
        }

    }
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~Plugin()
    {
        this.Dispose(false);
    }
    #endregion
    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        string msg = ServerChat.SanitiseText(args);
        ServerChat.SendMessage(msg);
        //SendChat("/fc " + args);
        
        //PrintChat(args);
        //ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}

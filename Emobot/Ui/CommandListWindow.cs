using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using ImGuiNET;

namespace Emobot.Ui;

internal class CommandListWindow: HelpWindow {
	private readonly Plugin plugin;

	public CommandListWindow(Plugin core) : base("Command List", Plugin.PluginName, "command listing", "A list of all commands in the plugin", "") {
		this.SizeConstraints = new() {
			MinimumSize = new(300, 100),
			MaximumSize = new(450, 550),
		};
		this.plugin = core;
	}

	public override void Draw() {
		this.DrawHeader();

		foreach (PluginCommand cmd in Plugin.CommandManager.Commands) {
			if (!cmd.ShowInListing)
				continue;

			
		}
	}

}

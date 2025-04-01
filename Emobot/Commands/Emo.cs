using Emobot.Attributes;
using Emobot.Chat;
using Emobot.Utils;

namespace Emobot.Commands;

[Command("/emotetext", "/emo")]
[Summary("Perform an emote without text, and simultaneously perform a custom-text emote to go with it")]
[HelpText(
	"This command condenses the given emote command (with the \"motion\" argument to silence it) and the \"/em\" command to simulate a custom emote macro in one command.",
	"For example, you can do \"/cem pat gives <t> some headpats\" to play the /pat animation with the custom text \"<Your Character> gives <Target Name> some headpats\" without needing a whole macro."
)]
public class Emo: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (string.IsNullOrWhiteSpace(rawArguments)) {
			showHelp = true;
			return;
		}
        string chatString = "";
        if (rawArguments.Length > 0)
        {
            chatString = rawArguments;
            ChatUtil.SendChatlineToServer(chatString, verbose, dryRun);
            //rawArguments = rawArguments[rawArguments.IndexOf(' ')..].Trim();
        }
        
	}
}

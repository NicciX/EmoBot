using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using org.mariuszgromada.math.mxparser;

using Emobot.Attributes;
using Emobot.Chat;
using Emobot.Utils;

namespace Emobot.Commands;

[Command("/calc", "/eval")]
[Summary("Evaluates the input string as a mathmatical expression and returns the result.")]
[HelpText(
    "This command condenses the given emote command (with the \"motion\" argument to silence it) and the \"/em\" command to simulate a custom emote macro in one command.",
    "For example, you can do \"/cem pat gives <t> some headpats\" to play the /pat animation with the custom text \"<Your Character> gives <Target Name> some headpats\" without needing a whole macro."
)]
public class Calc : PluginCommand
{
    protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp)
    {
        if (string.IsNullOrWhiteSpace(rawArguments))
        {
            showHelp = true;
            return;
        }
        //var result = rawArguments.Execute<string>();
        Expression e = new Expression(rawArguments);
        var v = e.calculate();
        string result = v.ToString();

        if (!string.IsNullOrWhiteSpace(result))
        {
            
            ChatUtil.SendChatlineToServer($"/fc " + rawArguments + " = " + result, verbose, dryRun);
            //rawArguments = rawArguments[rawArguments.IndexOf(' ')..].Trim();
        }
        
        //ChatUtil.SendChatlineToServer($"/{emoteName} motion", verbose, dryRun);
        //ChatUtil.SendChatlineToServer($"/em {actionText}", verbose, dryRun);
    }
}

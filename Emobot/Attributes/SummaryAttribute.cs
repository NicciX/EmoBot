using System;

namespace Emobot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class SummaryAttribute(string helpMessage): Attribute {
	public string Summary { get; } = helpMessage;
}

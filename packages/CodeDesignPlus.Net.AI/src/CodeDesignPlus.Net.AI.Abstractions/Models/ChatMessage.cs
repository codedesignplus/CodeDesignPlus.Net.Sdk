namespace CodeDesignPlus.Net.AI.Abstractions.Models;

/// <summary>
/// Represents a single message in a chat conversation.
/// </summary>
/// <param name="Role">The role of the message sender (System, User, or Assistant).</param>
/// <param name="Content">The text content of the message.</param>
public record ChatMessage(ChatRole Role, string Content);

/// <summary>
/// Defines the role of a participant in a chat conversation.
/// </summary>
public enum ChatRole
{
    /// <summary>System instructions that guide the AI behavior.</summary>
    System,
    /// <summary>Messages from the human user.</summary>
    User,
    /// <summary>Responses from the AI assistant.</summary>
    Assistant
}

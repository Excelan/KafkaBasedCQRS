namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class AddCommentCommand : BaseCommand
{
    public required string Comment { get; init; }
    public required string Username { get; init; }
}
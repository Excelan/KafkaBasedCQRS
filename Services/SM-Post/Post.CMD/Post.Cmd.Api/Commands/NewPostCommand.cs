namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class NewPostCommand : BaseCommand
{
    public required string Author { get; init; }
    public required string Message { get; init; }
}
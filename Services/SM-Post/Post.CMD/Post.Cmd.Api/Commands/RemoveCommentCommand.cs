namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public required string Username { get; init; }
}
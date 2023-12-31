namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class EditCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public required string Comment { get; init; }
    public required string Username { get; init; }
}
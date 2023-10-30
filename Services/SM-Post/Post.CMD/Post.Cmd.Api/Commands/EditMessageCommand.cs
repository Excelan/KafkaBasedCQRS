namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class EditMessageCommand : BaseCommand
{
    public required string Message { get; init; }

}
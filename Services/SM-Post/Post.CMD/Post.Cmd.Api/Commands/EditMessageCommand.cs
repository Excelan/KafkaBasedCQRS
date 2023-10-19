namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class EditMessageCommand: BaseCommand
{
    public string? Message { get; set; }
    
}
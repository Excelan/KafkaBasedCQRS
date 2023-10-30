namespace Post.Cmd.Api.Controllers;

using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

[Route("api/v1/[controller]")]
[ApiController]
public class NewPostController : ControllerBase
{
    const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";

    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher) {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command) {
        var id = Guid.NewGuid();
        try {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse {
                Message = "New post request is completed successfully"
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            _logger.LogError(e, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = SAFE_ERROR_MESSAGE,
                Id = id
            });
        }
    }
}

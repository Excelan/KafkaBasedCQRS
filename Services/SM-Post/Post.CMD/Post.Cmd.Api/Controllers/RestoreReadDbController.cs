using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class RestoreReadDbController: ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreReadDbController(ILogger<PostsController> logger, ICommandDispatcher commandDispatcher) {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }


    [HttpPost]
    public async Task<ActionResult> RestoreDbAsync() {
        try {
            await _commandDispatcher.SendAsync(new RestoreReadDbCommand());
            return StatusCode(
                StatusCodes.Status201Created,
                new NewPostResponse {
                    Message = "Read database restore command is accepted."
                }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Read Database resore error.";
            _logger.LogError(e, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse {
                Message = SAFE_ERROR_MESSAGE,
            });
        }
    }
}

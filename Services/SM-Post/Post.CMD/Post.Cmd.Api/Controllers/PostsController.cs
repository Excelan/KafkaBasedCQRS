namespace Post.Cmd.Api.Controllers;

using System.ComponentModel.Design;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

[Route("api/v1/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";

    private readonly ILogger<PostsController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public PostsController(ILogger<PostsController> logger, ICommandDispatcher commandDispatcher) {
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

    [HttpPut("{id}/EditMessage")]
    public async Task<ActionResult> EditPostMessageAsync(Guid id, EditMessageCommand command) {
        try {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Post message change request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to create a new post!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse {
                Message = EDIT_ERROR_MESSAGE,
            });
        }
    }

    [HttpPut("{id}/Like")]
    public async Task<ActionResult> LikeAsync(Guid id) {
        var command = new LikePostCommand { Id = id };
        try {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Post like request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to like a post!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = EDIT_ERROR_MESSAGE,
                Id = id
            });
        }
    }


    [HttpPut("{id}/AddComment")]
    public async Task<ActionResult> AddCommentAsync(Guid id, AddCommentCommand command) {
        try {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Add comment request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to add a comment to a post!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = EDIT_ERROR_MESSAGE,
                Id = id
            });
        }
    }

    [HttpPut("{id}/EditComment/{commentId}")]
    public async Task<ActionResult> EditCommentAsync(Guid id, Guid commentId, EditCommentCommand command) {
        try {
            command.Id = id;
            command.CommentId = commentId;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Edit comment request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to edit a comment!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = EDIT_ERROR_MESSAGE,
                Id = id
            });
        }
    }

    [HttpDelete("{id}/DeleteComment/{commentId}")]
    public async Task<ActionResult> DeleteCommentAsync(Guid id, Guid commentId, RemoveCommentCommand command) {
        try {
            command.Id = id;
            command.CommentId = commentId;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Delete comment request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to delete a comment!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = EDIT_ERROR_MESSAGE,
                Id = id
            });
        }
    }

    [HttpDelete("{id}/Delete")]
    public async Task<ActionResult> DeletePostMessageAsync(Guid id, DeletePostCommand command) {
        try {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new BaseResponse {
                Message = "Post message delete request is completed succesfully."
            }
            );
        } catch (InvalidOperationException e) {
            _logger.LogWarning(e, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (AggregateNotFoundException e) {
            _logger.LogWarning(e, $"Cannot find and aggregate with id [{id}]");
            return BadRequest(new BaseResponse { Message = e.Message });
        } catch (Exception e) {
            const string EDIT_ERROR_MESSAGE = "Error while processing request to delete a new post!";
            _logger.LogError(e, EDIT_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse {
                Message = EDIT_ERROR_MESSAGE,
                Id = id
            });
        }
    }
}

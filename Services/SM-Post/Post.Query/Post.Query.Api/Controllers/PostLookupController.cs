using System.Runtime.Intrinsics;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public class PostLookupController : Controller
{
    private ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _dispatcher;

    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> dispatcher) {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    private ActionResult<PostEntity> NormalResponse(List<PostEntity>? posts) {
        if (posts is null || !posts.Any()) {
            return NoContent();
        }
        return Ok(new PostLookupResponse {
            Posts = posts,
            Message = $"Successfully returned {posts.Count} record{(posts.Count > 0 ? "s" : string.Empty)}"
        });
    }

    private ActionResult<PostEntity> ErorrResponse(Exception e, string safe_error_message) {
        _logger.LogError(e, safe_error_message);
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse {
            Message = safe_error_message
        });
    }


    [HttpGet]
    public async Task<ActionResult<PostEntity>> GetAllPosts() {
        try {
            var posts = await _dispatcher.SendAsync(new FindAllPostsQuery());
            return NormalResponse(posts);
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Error while processing a query to retrieve all posts.";
            return ErorrResponse(e, SAFE_ERROR_MESSAGE);
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<PostEntity>> GetById(Guid id) {
        try {
            var posts = await _dispatcher.SendAsync(new FindPostByIdQuery { Id = id});
            return NormalResponse(posts);
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Error while processing a query to retrieve a post by Id.";
            return ErorrResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult<PostEntity>> GetByAuthor(string author) {
        try {
            var posts = await _dispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });
            return NormalResponse(posts);
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Error while processing a query to retrieve a post by author.";
            return ErorrResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult<PostEntity>> GetAllPostsWithComments() {
        try {
            var posts = await _dispatcher.SendAsync(new FindPostsWithCommentsQuery());
            return NormalResponse(posts);
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Error while processing a query to retrieve posts with comments.";
            return ErorrResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withLikes/{minNumberOfLikes}")]
    public async Task<ActionResult<PostEntity>> GetAllPostsWithLikes(int minNumberOfLikes) {
        try {
            var posts = await _dispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = minNumberOfLikes });
            return NormalResponse(posts);
        } catch (Exception e) {
            const string SAFE_ERROR_MESSAGE = "Error while processing a query to retrieve posts with likes.";
            return ErorrResponse(e, SAFE_ERROR_MESSAGE);
        }
    }
}

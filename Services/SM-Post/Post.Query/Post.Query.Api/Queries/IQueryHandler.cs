namespace Post.Query.Api.Queries;

using Post.Query.Domain.Entities;

public interface IQueryHandler
{
    Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query);
}

namespace Post.Query.Api.Queries;

using CQRS.Core.Queries;

public class FindPostsWithLikesQuery: BaseQuery
{
    public int NumberOfLikes { get; set; }
}

namespace Post.Query.Api.Queries;

using CQRS.Core.Queries;

public class FindPostsByAuthorQuery: BaseQuery
{
    public string Author { get; set; }
}

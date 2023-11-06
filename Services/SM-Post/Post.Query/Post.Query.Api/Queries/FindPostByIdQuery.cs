namespace Post.Query.Api.Queries;

using CQRS.Core.Queries;

public class FindPostByIdQuery : BaseQuery
{
    public Guid Id { get; set; }
}

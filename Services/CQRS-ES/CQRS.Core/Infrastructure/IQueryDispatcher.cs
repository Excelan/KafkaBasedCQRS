using CQRS.Core.Queries;

namespace CQRS.Core.Infrastructure;

// A Mediator
public interface IQueryDispatcher<TEntity>
{
    void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> queryHandler) where TQuery : BaseQuery;
    Task<List<TEntity>> SendAsync(BaseQuery query);

}

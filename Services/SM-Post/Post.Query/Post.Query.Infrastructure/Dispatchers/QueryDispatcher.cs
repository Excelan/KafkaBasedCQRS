namespace Post.Query.Infrastructure.Dispatchers;

using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> queryHandler) where TQuery : BaseQuery {
        ArgumentNullException.ThrowIfNull(queryHandler);
        if (_handlers.ContainsKey(typeof(TQuery))) {
            throw new ArgumentException($"You can only register one handler per query type. A handler for [{typeof(TQuery)}] is already registered.");
        }
        _handlers[typeof(TQuery)] = x => queryHandler((TQuery)x);
    }

    public async Task<List<PostEntity>> SendAsync(BaseQuery query) {
        if (_handlers.TryGetValue(query.GetType(), out var handler)) {
            return await handler(query);
        }
        throw new ArgumentException($"Query of type {query.GetType().Name} is curremtly not supported.");
    }
}

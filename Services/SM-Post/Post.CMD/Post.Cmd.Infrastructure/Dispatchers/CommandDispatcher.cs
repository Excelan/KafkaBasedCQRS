namespace Post.Cmd.Infrastructure.Dispatchers;

using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand {
        if (_handlers.ContainsKey(typeof(T))) {
            throw new ArgumentException($"You cannot register the handler for the type {typeof(T).Name} twice");
        }
        _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command) {
        var commandType = command.GetType();
        if (_handlers.TryGetValue(commandType, out Func<BaseCommand, Task>? handler)) {
            if (handler is null) {
                throw new ApplicationException($"Registered handler for {commandType.Name} is null.");
            }
            await handler(command);
        } else {
            throw new ArgumentException($"No handlers is registered for {commandType.Name}.");
        }
    }
}

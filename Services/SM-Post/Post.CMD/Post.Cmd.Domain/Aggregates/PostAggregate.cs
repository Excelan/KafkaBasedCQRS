namespace Post.Cmd.Domain.Aggregates;

using CQRS.Core.Domain;
using Post.Common.Events;

public class PostAggregate : AggregateRoot
{
    private bool _isActive;
    private string? _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active { get => _isActive; set => _isActive = value; }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message) {
        if (string.IsNullOrEmpty(author)) {
            throw new ArgumentException($"'{nameof(author)}' cannot be null or empty.", nameof(author));
        }

        RaiseEvent(new PostCreatedEvent {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    public void Apply(PostCreatedEvent @event) {
        if (string.IsNullOrEmpty(@event.Author)) {
            throw new ArgumentException($"'{nameof(@event.Author)}' cannot contain null or empty.", nameof(@event));
        }

        _id = @event.Id;
        _author = @event.Author;
        _isActive = true;
        // who processes @event.Mesage and @event.DatePosted ?
    }

    public void EditMessage(string message) {
        if (!_isActive) {
            throw new InvalidOperationException("You can not edit the message of an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(message)) {
            throw new InvalidOperationException($"You can not post an empty {nameof(message)}.");
        }

        RaiseEvent(new MessageUpdatedEvent {
            Id = _id,
            Message = message,
        });
    }

    public void Apply(MessageUpdatedEvent @event) {
        if (string.IsNullOrWhiteSpace(@event.Message)) {
            throw new InvalidOperationException($"You can not post an empty message.");
        }
        _id = @event.Id;
    }

    public void LikePost() {
        if (!_isActive) {
            throw new InvalidOperationException("You can not like an inactive post.");
        }

        RaiseEvent(new PostLikeEvent {
            Id = _id
        });
    }

    public void Apply(PostLikeEvent @event) {
        _id = @event.Id;
    }

    public void AddComment(string comment, string userName) {
        if (!_isActive) {
            throw new InvalidOperationException("You can add a comment to an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(comment)) {
            throw new InvalidOperationException($"You can not post an empty comment.");
        }

        if (string.IsNullOrWhiteSpace(userName)) {
            throw new ArgumentException($"'{nameof(userName)}' cannot be null or whitespace.", nameof(userName));
        }

        RaiseEvent(new CommentAddedEvent {
            Id = _id,
            Comment = comment,
            CommentId = Guid.NewGuid(),
            Username = userName,
            CommentDate = DateTime.UtcNow
        });
    }

    public void Apply(CommentAddedEvent @event) {
        if (string.IsNullOrWhiteSpace(@event.Comment)) {
            throw new InvalidOperationException($"You can not post an empty comment.");
        }

        if (string.IsNullOrWhiteSpace(@event.Username)) {
            throw new ArgumentException($"A userName cannot be null or whitespace.");
        }

        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string userName) {
        if (!_isActive) {
            throw new InvalidOperationException("You can change a comment of an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(comment)) {
            throw new InvalidOperationException($"You can not post an empty comment.");
        }

        if (string.IsNullOrWhiteSpace(userName)) {
            throw new ArgumentException($"'{nameof(userName)}' cannot be null or whitespace.", nameof(userName));
        }

        if (!_comments[commentId].Item2.Equals(userName, StringComparison.InvariantCultureIgnoreCase)) {
            throw new InvalidOperationException("You are not allowed to change a comment of another user.");
        }

        RaiseEvent(new CommentUpdatedEvent {
            Id = _id,
            Comment = comment,
            CommentId = commentId,
            Username = userName,
            EditDate = DateTime.UtcNow
        });
    }

    public void Apply(CommentUpdatedEvent @event) {
        if (string.IsNullOrWhiteSpace(@event.Comment)) {
            throw new InvalidOperationException($"You can not post an empty comment.");
        }

        if (string.IsNullOrWhiteSpace(@event.Username)) {
            throw new ArgumentException($"A userName cannot be null or whitespace.");
        }

        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username) {
        if (!_isActive) {
            throw new InvalidOperationException("You can not remove a comment of an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(username)) {
            throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace.", nameof(username));
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.InvariantCultureIgnoreCase)) {
            throw new InvalidOperationException("You are not allowed to change a comment of another user.");
        }

        RaiseEvent(new CommentRemovedEvent {
            Id = _id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event) {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username) {
        if (!_isActive) {
            throw new InvalidOperationException("The post is already removed.");
        }

        if (string.IsNullOrWhiteSpace(username)) {
            throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace.", nameof(username));
        }

        if (!username.Equals(_author, StringComparison.InvariantCultureIgnoreCase)) {
            throw new InvalidOperationException("You are not allowed to delete a post of another user.");
        }

        RaiseEvent(new PostRemovedEvent {
            Id = _id
        });
    }

    public void Apply(PostRemovedEvent @event) { 
        _id = @event.Id;
        _isActive = false;
    }

}

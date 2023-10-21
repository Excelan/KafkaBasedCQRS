using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;
internal interface ICommentRepository
{
    Task CreateAsync(CommentEntity entity);
    Task UpdateAsync(CommentEntity entity);
    Task<CommentEntity> GetByIdAsync(Guid commentId);
    Task DeleteAsync (Guid commentId);
}

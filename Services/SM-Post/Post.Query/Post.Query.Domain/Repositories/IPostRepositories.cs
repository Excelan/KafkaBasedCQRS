using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;
internal interface IPostRepositories
{
    Task CreateAsync(PostEntity post);
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid postId);
    Task<PostEntity> GetByIdAsync(Guid postId);
    
    Task<List<PostEntity>> ListAllAsync();
    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListWithLikesAsync(int likes);
    Task<List<PostEntity>> ListWithCommentsAsync();



}

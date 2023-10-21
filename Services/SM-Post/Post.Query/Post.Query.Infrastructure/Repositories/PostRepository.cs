using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;
internal class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity post) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post != null) {
            // why not just simply
            // context.Remove(post);
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var postEntity = await context.Posts
            .Include(p => p.Comments) // this comes Lazy because of UseLazyLoadingProxies
            .FirstOrDefaultAsync(x => x.PostId == postId);
        return postEntity ?? throw new Exception($"No Post found for id: [{postId}]");
    }

    public async Task<List<PostEntity>> ListAllAsync() {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking() // while this is used only in read-only scenario.
            .Include(p => p.Comments) // this comes from LazyProxies
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking() // while this is used only in read-only scenario.
            .Where(x => x.Author.Equals(author, StringComparison.InvariantCultureIgnoreCase))
            .Include(p => p.Comments) // this comes from LazyProxies
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync() {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking() // while this is used only in read-only scenario.
            // .Where(x => x.Comments != null || x.Comments.Any())
            .Where(x => x.Comments.Any())
            .Include(p => p.Comments) // this comes from LazyProxies
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking() // while this is used only in read-only scenario.
            .Where(x => x.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }
}

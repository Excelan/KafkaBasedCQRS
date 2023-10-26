namespace Post.Query.Infrastructure.Repositories;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

internal class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory) {
        _contextFactory = contextFactory;
    }

    // So the Rep responsible for the whole CRUD.
    // I suppose the read op. is the only one needed for the Post.Quert.API
    // And CUD is what handlers will use.

    public async Task CreateAsync(CommentEntity comment) {
        using var context = _contextFactory.CreateDbContext();
        context.Comments.Add(comment);
        _ = await context.SaveChangesAsync();

    }

    public async Task<CommentEntity?> GetByIdAsync(Guid commentId) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Comments.SingleOrDefaultAsync(x => x.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity comment) {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Update(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId) {
        var comment = await GetByIdAsync(commentId);
        if (comment == null) {
            return;
        }
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Remove(comment);
        _ = await context.SaveChangesAsync();
    }

}

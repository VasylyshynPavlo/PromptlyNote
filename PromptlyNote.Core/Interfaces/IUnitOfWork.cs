using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }
}

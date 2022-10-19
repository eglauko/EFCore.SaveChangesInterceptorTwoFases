using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.SaveChangesInterceptorTwoFases;

public class MyInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        var transactionManager = eventData.Context.GetService<TransactionManager>();

        if (transactionManager.Stage == 0)
        { 
            var transaction = eventData.Context.Database.CurrentTransaction;
            if (transaction is null)
            {
                transaction = eventData.Context.Database.BeginTransaction();
                transactionManager.ManagedTransaction = transaction;
                transactionManager.Stage = 1;
            }
        }
        else if (transactionManager.Stage == 1)
        {
            transactionManager.Stage = 2;
        }

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is null)
            return result;

        var transactionManager = eventData.Context.GetService<TransactionManager>();
        if (transactionManager.ManagedTransaction is not null)
        {
            if (transactionManager.Stage == 1)
            { 
                // executar segundo estágio
                // todo
                
                // salvar pela segunda vez.
                result += eventData.Context.SaveChanges();

                // commit
                transactionManager.ManagedTransaction.Commit();

                // reset
                transactionManager.Stage = 0;
                transactionManager.ManagedTransaction.Dispose();
                transactionManager.ManagedTransaction = null;

                // return
                return result;
            }
        }

        return result;
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, 
        int result, 
        CancellationToken cancellationToken = default)
    {
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}

public class TransactionManager
{
    public IDbContextTransaction? ManagedTransaction { get; set; }

    public int Stage { get; set; }
}
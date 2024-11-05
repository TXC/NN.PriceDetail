using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public class Seeder
    {
        /// <see cref="DbContextOptionsBuilder.UseSeeding(Action{DbContext, bool})"/>
        public static void Run<T>(DbContext ctx, bool stored, Func<T[]> GetRows)
            where T : class => RunAsync<T>(ctx, stored, GetRows).GetAwaiter().GetResult();

        /// <see cref="DbContextOptionsBuilder.UseAsyncSeeding(Func{DbContext, bool, CancellationToken, Task})"/>
        async public static Task RunAsync<T>(DbContext ctx, bool stored, Func<T[]> GetRows, CancellationToken token = default)
            where T : class
        {
            var res = await ctx.Set<T>()
                                 .FirstOrDefaultAsync(token);
            if (res is not null)
            {
                return;
            }

            try
            {
                var entityType = ctx.Model.FindEntityType(typeof(T));

                var result = GetRows.Invoke();

                await ctx.Set<T>().AddRangeAsync(result, token);
                await ctx.EnableIdentityInsert<T>();

                await ctx.SaveChangesAsync(token);

                await ctx.DisableIdentityInsert<T>();
            }
            catch (Exception)
            {
                // If a failure occurred, we rollback to the savepoint and can continue the transaction
                //transaction.RollbackToSavepoint("BeforeSeeding");
            }
        }
    }
}

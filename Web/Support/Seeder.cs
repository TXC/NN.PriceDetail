namespace Web.Support
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Web.Extensions;

    public class Seeder
    {
        /// <see cref="DbContextOptionsBuilder.UseSeeding(Action{DbContext, bool})"/>
        public static void Run<T>(DbContext ctx, bool stored, Func<T[]> GetRows)
            where T : class => RunAsync<T>(ctx, stored, GetRows).GetAwaiter().GetResult();

        /// <see cref="DbContextOptionsBuilder.UseAsyncSeeding(Func{DbContext, bool, CancellationToken, Task})"/>
        async public static Task RunAsync<T>(DbContext ctx, bool _, Func<T[]> GetRows, CancellationToken token = default)
            where T : class
        {
            if (await ctx.Set<T>().AnyAsync(token))
            {
                return;
            }

            var result = GetRows.Invoke();

            await ctx.Set<T>().AddRangeAsync(result, token);
            await ctx.EnableIdentityInsert<T>();

            await ctx.SaveChangesAsync(token);

            await ctx.DisableIdentityInsert<T>();
        }
    }
}

namespace Web.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    public static class IdentifyExtension
    {
        public static Task EnableIdentityInsert<T>(this DbContext context)
            => SetIdentityInsert<T>(context, enable: true);

        public static Task DisableIdentityInsert<T>(this DbContext context)
            => SetIdentityInsert<T>(context, enable: false);

        /// <summary>
        /// Enable/Disable Identity Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="enable"></param>
        /// <returns />
        static Task SetIdentityInsert<T>(DbContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";
            return context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
        }

        public static void SaveChangesWithIdentityInsert<T>(this DbContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            context.EnableIdentityInsert<T>();
            context.SaveChanges();
            context.DisableIdentityInsert<T>();
            transaction.Commit();
        }

    }
}

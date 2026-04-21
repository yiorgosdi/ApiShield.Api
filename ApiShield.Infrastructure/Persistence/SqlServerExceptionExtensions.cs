using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiShield.Infrastructure.Persistence;

internal static class SqlServerExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this Exception exception)
    {
        if (exception is DbUpdateException dbUpdateException &&
            dbUpdateException.InnerException is SqlException sqlException)
        {
            return sqlException.Number is 2601 or 2627;
        }

        return false;
    }
}
using System;
using System.Data;
using System.Reflection;
using Dapper;
using FS.Commons;
using FS.IdentityFramework;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FS.DAL.Base;

/// <summary>
/// Class này dùng chung cho một DB, sau nếu có thay đổi db thì chỉ cần thay đổi class này
/// </summary>
public class BaseRepository : IDisposable
{
    // Flag: Has Dispose already been called?
    bool disposed = false;
    private string _connectionString;
    private IDbConnection _connection;
    public string LanguageCode { get; set; }

    public FSDbContext _dbContext;
    public BaseRepository(IConfiguration configuration, FSDbContext context)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _dbContext = context;
    }
    protected virtual IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }
            return _connection;
        }
    }

    public DynamicParameters AddDynamicParams<T>(T obj)
    {
        var param = new DynamicParameters();
        Type type = typeof(T);
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute(typeof(OutputParamAttribute));
            if (attr == null)
            {
                param.Add(string.Format("@{0}", prop.Name), prop.GetValue(obj), MappingDBType(prop.PropertyType), ParameterDirection.Input);
            }
            else
            {
                param.Add(string.Format("@{0}", prop.Name), prop.GetValue(obj), MappingDBType(prop.PropertyType), ParameterDirection.Output);
            }
        }
        return param;
    }

    /// <summary>
    /// Mappings the type of the database.
    /// </summary>
    /// <param name="propType">Type of the property.</param>
    /// <returns></returns>
    private DbType MappingDBType(Type propType)
    {
        switch (Type.GetTypeCode(propType))
        {
            case TypeCode.Byte:
                return DbType.Byte;
            case TypeCode.SByte:
                return DbType.SByte;
            case TypeCode.Int16:
                return DbType.Int16;
            case TypeCode.Int32:
                return DbType.Int32;
            case TypeCode.Int64:
                return DbType.Int64;
            case TypeCode.Decimal:
                return DbType.Decimal;
            case TypeCode.Double:
                return DbType.Double;
            case TypeCode.Single:
                return DbType.Single;
            default:
                return DbType.String;
        }
    }

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            _dbContext?.Dispose();
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
            _connection?.Dispose();
            // Free any other managed objects here.
            //
        }

        // Free any unmanaged objects here.
        //
        disposed = true;
    }

    ~BaseRepository()
    {
        Dispose(false);
    }
}

using Core.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly string _connectionString;

        public GenericRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Add(TEntity entity)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var tableName = typeof(TEntity).Name;
                    var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id" && p.Name != "File");

                    var columnNames = string.Join(",", properties.Select(p => p.Name));
                    var parameterNames = string.Join(",", properties.Select(p => "@" + p.Name));

                    var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames});";

                    await connection.ExecuteAsync(query, entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex);
                throw;
            }
        }
        public async Task<TEntity> GetById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var tableName = typeof(TEntity).Name;
                    var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "File");
                    var columnNames = string.Join(",", properties.Select(p => p.Name));
                    var primaryKey = "Id";

                    var query = $"SELECT {columnNames} FROM {tableName} WHERE {primaryKey} = @Id;";

                    return await connection.QuerySingleOrDefaultAsync<TEntity>(query, new { Id = id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex);
                throw;
            }
        }
        public async Task<List<TEntity>> GetAll()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var tableName = typeof(TEntity).Name;
                    var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "File");
                    var columnNames = string.Join(",", properties.Select(p => p.Name));

                    var query = $"SELECT {columnNames} FROM {tableName};";
                    var entities = await connection.QueryAsync<TEntity>(query);
                    return entities.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex);
                throw;
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var tableName = typeof(TEntity).Name;
                    var primaryKey = "Id";

                    var query = $"DELETE FROM {tableName} WHERE {primaryKey} = @Id;";
                    await connection.ExecuteAsync(query, new { Id = id }); // new { Id = id } is an anonymous object; Id property should match the Id parameter present in the query
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex);
                throw;
            }
        }

        public async Task Update(TEntity entity)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var tableName = typeof(TEntity).Name;
                    var primaryKey = "Id";

                    var properties = typeof(TEntity).GetProperties().Where(p => p.Name != primaryKey && p.Name != "Image");

                    var setClause = string.Join(",", properties.Select(p => $"{p.Name} = @{p.Name}"));
                    var query = $"UPDATE {tableName} SET {setClause} WHERE {primaryKey} = @{primaryKey};";
                    await connection.ExecuteAsync(query, entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex);
                throw;
            }
        }

    }
}

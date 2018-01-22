using CatiLyfe.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatiLyfe.DataLayer.Sql
{
    internal abstract class SqlDataLayerBase
    {
        private readonly string connectionString;

        protected SqlDataLayerBase(string connectionString)
        {
            this.connectionString = connectionString;
        }


        protected async Task<IEnumerable<T1>> ExecuteReader<T1>(string sproc, Action<SqlParameterCollection> parameters, Func<SqlDataReader, T1> readerset1)
            where T1 : class
        {
            var results = await this.ExecuteReader(sproc, parameters, new Func<SqlDataReader, object>[] { readerset1 });

            return results[0].Cast<T1>();
        }

        protected async Task<(IEnumerable<T1>, IEnumerable<T2>)> ExecuteReader<T1, T2>(
            string sproc,
            Action<SqlParameterCollection> parameters,
            Func<SqlDataReader, T1> readerset1,
            Func<SqlDataReader, T2> readerset2) where T1 : class where T2 : class
        {
            var results = await this.ExecuteReader(sproc, parameters, new Func<SqlDataReader, object>[] { readerset1, readerset2 });

            return (results[0].Cast<T1>(), results[1].Cast<T2>());
        }

        protected async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>)> ExecuteReader<T1, T2, T3>(
            string sproc,
            Action<SqlParameterCollection> parameters,
            Func<SqlDataReader, T1> readerset1,
            Func<SqlDataReader, T2> readerset2,
            Func<SqlDataReader, T3> readerset3) where T1 : class where T2 : class where T3 : class
        {
            var results = await this.ExecuteReader(
                              sproc,
                              parameters,
                              new Func<SqlDataReader, object>[] { readerset1, readerset2, readerset3 });

            return (results[0].Cast<T1>(), results[1].Cast<T2>(), results[2].Cast<T3>());
        }

        protected async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>)> ExecuteReader<T1, T2, T3, T4>(
            string sproc,
            Action<SqlParameterCollection> parameters,
            Func<SqlDataReader, T1> readerset1,
            Func<SqlDataReader, T2> readerset2,
            Func<SqlDataReader, T3> readerset3,
            Func<SqlDataReader, T4> readerset4) where T1 : class where T2 : class where T3 : class where T4 : class
        {
            var results = await this.ExecuteReader(
                              sproc,
                              parameters,
                              new Func<SqlDataReader, object>[] { readerset1, readerset2, readerset3, readerset4 });

            return (results[0].Cast<T1>(), results[1].Cast<T2>(), results[2].Cast<T3>(), results[3].Cast<T4>());
        }

        /// <summary>
        /// A genius function which allows for the parsing of SQL result sets.
        /// </summary>
        /// <param name="sproc">The stored procedure name.</param>
        /// <param name="parameters">The parameters modifier.</param>
        /// <param name="readersets">The reader functions.</param>
        /// <returns>All of the data.</returns>
        private async Task<List<object>[]> ExecuteReader(
            string sproc,
            Action<SqlParameterCollection> parameters,
            IList<Func<SqlDataReader, object>> readersets)
        {
            using (var connection = await this.GetConnection())
            {
                var command = SetupCommand(sproc, parameters, connection);

                var results = new List<object>[readersets.Count];

                await SqlDataLayerBase.ExecuteSqlReader(
                    async () =>
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // If we returned an error, exit immediatly.
                            this.HandleSoftError(command);

                            for (var i = 0; i < readersets.Count; i++)
                            {
                                results[i] = new List<object>();

                                while (true == await reader.ReadAsync())
                                {
                                    results[i].Add(readersets[i](reader));
                                }

                                if (i < readersets.Count - 1)
                                {
                                    if (false == await reader.NextResultAsync())
                                    {
                                        // Check to see if we got something nice.
                                        this.HandleSoftError(command);

                                        // Otherwise throw normally
                                        throw new InvalidOperationException($"Expecting another result set in sproc {sproc}. Current {i + 1} out of {readersets.Count}.");
                                    }
                                }
                            }
                        }
                    });

                return results;
            }
        }

        protected async Task ExecuteNonQuery(
            string sproc,
            Action<SqlParameterCollection> parameters)
        {
            using (var connection = await this.GetConnection())
            {
                var command = SetupCommand(sproc, parameters, connection);

                await SqlDataLayerBase.ExecuteSqlReader(
                    async () =>
                    {
                        await command.ExecuteNonQueryAsync();
                        this.HandleSoftError(command);
                    });
            }
        }

        private void HandleSoftError(SqlCommand command)
        {
            var retvalue = command.Parameters["ReturnValue"]?.Value as int? ?? 0;
            var message = command.Parameters["error_message"]?.Value as string;

            switch (retvalue)
            {
                case 0:
                    return;
                case 50001:
                    throw new ItemNotFoundException(message ?? "Item not found");
                case 50002:
                    throw new InvalidArgumentException(message ?? "Invalid arguments provided.");
                case 50003:
                    throw new DuplicateItemException(message ?? "Duplicate item");
                case 50004:
                    throw new RevisionMismatchException(message ?? "Revision id mismatch");
                default:
                    if (retvalue > 50000)
                    {
                        throw new DeveloperIsAnIdiotException();
                    }
                    else
                    {
                        throw new Exception("SQL failure. Generic. No error. Peter will fix.");
                    }
            }
        }

        private async Task<SqlConnection> GetConnection()
        {
            var connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private static SqlCommand SetupCommand(string sproc, Action<SqlParameterCollection> parameters, SqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = sproc;
            command.CommandType = CommandType.StoredProcedure;

            var error = new SqlParameter("error_message", SqlDbType.NVarChar, 2048) { Direction = ParameterDirection.Output };
            var retVal = new SqlParameter("ReturnValue", SqlDbType.Int, 4) { Direction = ParameterDirection.ReturnValue };

            command.Parameters.Add(error);
            command.Parameters.Add(retVal);
            parameters(command.Parameters);
            return command;
        }

        private static async Task ExecuteSqlReader(Func<Task> function)
        {
            try
            {
                await function();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

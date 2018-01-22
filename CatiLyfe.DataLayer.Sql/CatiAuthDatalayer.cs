using CatiLyfe.Common.Exceptions;
using CatiLyfe.DataLayer.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatiLyfe.DataLayer.Sql
{
    internal class CatiAuthDatalayer : SqlDataLayerBase, ICatiAuthDataLayer
    {
        public CatiAuthDatalayer(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Gets all user information.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="email">The email.</param>
        /// <param name="token">The token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IEnumerable<User>> GetUser(IEnumerable<int> ids, IEnumerable<string> emails, IEnumerable<string> names, byte[] token)
        {
            var result = await this.ExecuteReader(
                "auth.getuserinfo2",
                parameters =>
                {
                    var idParam = parameters.AddWithValue(
                                          "ids",
                                          CatiAuthDatalayer.CreateIdList(ids));
                    idParam.SqlDbType = SqlDbType.Structured;
                    idParam.TypeName = "auth.idlist";

                    var emailParam = parameters.AddWithValue(
                                          "emails",
                                          CatiAuthDatalayer.CreateStringList(emails));
                    emailParam.SqlDbType = SqlDbType.Structured;
                    emailParam.TypeName = "auth.stringlist";

                    var nameParam = parameters.AddWithValue(
                                          "names",
                                          CatiAuthDatalayer.CreateStringList(names));
                    nameParam.SqlDbType = SqlDbType.Structured;
                    nameParam.TypeName = "auth.stringlist";

                    parameters.AddWithValue("token", token);
                },
                SqlParsers.ParseRole,
                SqlParsers.ParseUser);

            var users = result.Item2.ToList();

            if (false == users.Any())
            {
                throw new ItemNotFoundException("The user could not be found.");
            }

            var roles = result.Item1.ToLookup(role => (int?)role.UserId, role => role.Role);

            foreach (var user in users)
            {
                user.Roles = new HashSet<string>((roles.Contains(user.Id) ? roles[user.Id] : Enumerable.Empty<string>()));
            }

            return users;
        }

        /// <summary>
        /// Creates a token for a user.
        /// </summary>
        /// <param name="user">The user id.</param>
        /// <param name="token">The token.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns>An async task.</returns>
        public Task CreateToken(int user, byte[] token, DateTime expiry)
        {
            return this.ExecuteNonQuery(
                "auth.settoken",
                parameters =>
                {
                    parameters.AddWithValue("userid", user);
                    parameters.AddWithValue("token", token);
                    parameters.AddWithValue("expiry", expiry);
                });
        }

        /// <summary>
        /// Sets a user based on the user model.
        /// </summary>
        /// <param name="usermodel">The user model.</param>
        /// <returns>An async task..</returns>
        public Task SetUser(User usermodel)
        {
            return this.ExecuteNonQuery(
                "auth.setuserinfo",
                parameters =>
                {
                    parameters.AddWithValue("id", usermodel.Id);
                    parameters.AddWithValue("name", usermodel.Name);
                    parameters.AddWithValue("email", usermodel.Email);
                    parameters.AddWithValue("salt", usermodel.Salt);
                    parameters.AddWithValue("password", usermodel.Password);
                    var rolelist = parameters.AddWithValue(
                        "rolelist",
                        CatiAuthDatalayer.GetRoleRecord(usermodel.Roles));
                    rolelist.SqlDbType = SqlDbType.Structured;
                    rolelist.TypeName = "auth.rolelist";
                });
        }

        /// <summary>
        /// Gets the user roles.
        /// </summary>
        /// <returns>The list of roles.</returns>
        public Task<IEnumerable<UserRoleDescription>> GetRoles()
        {
            return this.ExecuteReader(
                "auth.getroles",
                parameters: parameters =>
                {
                },
                readerset1: SqlParsers.ParseUserRoleDescription);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="name">The name of role.</param>
        /// <param name="commit">To really delete it. Or just soft delete.</param>
        /// <returns>The task.</returns>
        public Task DeleteRole(string name, bool commit)
        {
            return this.ExecuteNonQuery(
                "auth.deleterole",
                parameters: parameters =>
                {
                    parameters.AddWithValue("name", name);
                    parameters.AddWithValue("commit", commit);
                });
        }

        /// <summary>
        /// Creates or edits a role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task SetRole(UserRoleDescription role)
        {
            return this.ExecuteNonQuery(
                "auth.setrole",
                parameters: paramters =>
                {
                    paramters.AddWithValue("name", role.RoleName);
                    paramters.AddWithValue("description", role.Description);
                    paramters.AddWithValue("revive", true);
                });
        }

        /// <summary>
        /// Deauthorize a token.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The token.</param>
        /// <returns>An async task.</returns>
        public Task DeauthorizeToken(int user, byte[] token)
        {
            return this.ExecuteNonQuery("auth.settoken", parameters => {
                parameters.AddWithValue("userid", user);
                parameters.AddWithValue("token", token);
                parameters.AddWithValue("expiry", DateTime.UtcNow);
            });
        }

        /// <summary>
        /// Gets the role records.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <returns>The records.</returns>
        private static IEnumerable<SqlDataRecord> GetRoleRecord(IEnumerable<string> roles)
        {
            return roles.ToDataTable(
                () => new[] { new SqlMetaData("role", SqlDbType.NVarChar, 64) },
                (record, role) =>
                {
                    record.SetValue(0, role);
                });
        }

        /// <summary>
        /// Create a string list.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns>A string list.</returns>
        private static IEnumerable<SqlDataRecord> CreateStringList(IEnumerable<string> strings)
        {
            return strings.ToDataTable(
                () => new[] { new SqlMetaData("string", SqlDbType.NVarChar, 256) },
                (record, tag) =>
                {
                    record.SetValue(0, tag);
                });
        }

        /// <summary>
        /// Create an ID list.
        /// </summary>
        /// <param name="ids">The ids to add.</param>
        /// <returns>An id list.</returns>
        private static IEnumerable<SqlDataRecord> CreateIdList(IEnumerable<int> ids)
        {
            return ids.ToDataTable(
                () => new[] { new SqlMetaData("id", SqlDbType.Int) },
                (record, tag) =>
                {
                    record.SetValue(0, tag);
                });
        }
    }
}

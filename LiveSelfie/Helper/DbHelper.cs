using Microsoft.Data.SqlClient;
using System.Data;

namespace LiveSelfie.Helper
{
    public static class DbHelper
    {
        public static async Task<List<T>> ExecuteQuery<T>(string constring, string query, Dictionary<string, object> parameters) where T : new()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(
                                param.Key,
                                param.Value ?? DBNull.Value);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            await Task.Run(() => da.Fill(dt));
                        }
                    }
                }

                List<T> data = new List<T>();

                foreach (DataRow row in dt.Rows)
                {
                    T item = new T();

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        try
                        {
                            if (dt.Columns.Contains(prop.Name) &&
                                row[prop.Name] != DBNull.Value)
                            {
                                Type propertyType =
                                    Nullable.GetUnderlyingType(prop.PropertyType)
                                    ?? prop.PropertyType;

                                object safeValue = Convert.ChangeType(
                                    row[prop.Name],
                                    propertyType);

                                prop.SetValue(item, safeValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Property Mapping Error: {prop.Name} - {ex.Message}");
                        }
                    }

                    data.Add(item);
                }

                return data;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");

                // Optional logging
                // Logger.LogError(sqlEx);

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Optional logging
                // Logger.LogError(ex);

                throw;
            }
        }

        public static async Task<int> ExecuteNonQuery(string constring, string query, Dictionary<string, object> parameters)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(
                                param.Key,
                                param.Value ?? DBNull.Value);
                        }

                        await con.OpenAsync();

                        object result = await cmd.ExecuteScalarAsync();

                        return result != null
                            ? Convert.ToInt32(result)
                            : 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Database related exception
                Console.WriteLine($"SQL Error: {sqlEx.Message}");

                // Optional: log error
                // Logger.LogError(sqlEx);

                throw;
            }
            catch (Exception ex)
            {
                // General exception
                Console.WriteLine($"Error: {ex.Message}");

                // Optional: log error
                // Logger.LogError(ex);

                throw;
            }
        }

        public static async Task<DataSet> ExecuteQueryDataSet(string constring, string query, Dictionary<string, object> parameters)
        {
            try
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add Parameters
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(
                                param.Key,
                                param.Value ?? DBNull.Value);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            await Task.Run(() => da.Fill(ds));
                        }
                    }
                }

                return ds;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");

                // Optional Logging
                // Logger.LogError(sqlEx);

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Optional Logging
                // Logger.LogError(ex);

                throw;
            }
        }
    }
}

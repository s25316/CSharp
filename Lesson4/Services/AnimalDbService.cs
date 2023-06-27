using Lesson4.Models;
using Lesson4.Models.DTO;
using System.Collections.Generic;
using System.Data.SqlClient;
using static System.Reflection.Metadata.BlobBuilder;

namespace Lesson4.Services
{
    public class AnimalDbService : IAnimalDbService
    {
        private const string _connString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";


        public async Task<IList<Animal>> GetAllAnimalsAsync(string orderBy)
        {
            List<Animal> animals = new();

            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            string sql;
            if (string.IsNullOrWhiteSpace(orderBy) ||
                !orderBy.ToUpper().Equals("idAnimal") ||
                !orderBy.ToUpper().Equals("name") ||
                !orderBy.ToUpper().Equals("description") ||
                !orderBy.ToUpper().Equals("category") ||
                !orderBy.ToUpper().Equals("area"))
            {
                sql = "SELECT * FROM Animal ORDER BY Name ASC";
            }
            else
            {
                sql = $"SELECT * FROM Animal ORDER BY {orderBy.ToLower()} ASC";
            }

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            await using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            while (await sqlDataReader.ReadAsync())
            {
                Animal animal = new()
                {
                    IdAnimal = int.Parse(sqlDataReader["IdAnimal"].ToString()),
                    Name = sqlDataReader["Name"].ToString(),
                    Description = sqlDataReader["Description"].ToString(),
                    Category = sqlDataReader["Category"].ToString(),
                    Area = sqlDataReader["Area"].ToString()
                };
                animals.Add(animal);
            }

            await sqlConnection.CloseAsync();


            return animals;
        }

        public async Task<bool> DeleteAnimal(int idAnimal)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            string sql = $"DELETE FROM Animal WHERE IdAnimal = {idAnimal} ;";


            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            int countDeletedAnimals = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();


            return countDeletedAnimals > 0;
        }


        public async Task<bool> PostAnimal(AnimalDTO animal)
        {
            
            string sql = $"INSERT INTO Animal VALUES('{animal.Name}', '{animal.Description}', '{animal.Category}', '{animal.Area}');";

            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            int countPostedAnimals = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();


            return countPostedAnimals > 0;
            /*
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new(
                $"INSERT INTO Animal VALUES('{animal.Name}', '{animal.Description}', '{animal.Category}', '{animal.Area}')"
                , sqlConnection);

            await sqlConnection.OpenAsync();

            int rows = await sqlCommand.ExecuteNonQueryAsync();

            return rows > 0;
            */
        }

        public async Task<bool> PutAnimal(int idAnimal, AnimalDTO animal)
        {
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();
           

            string sql = $"UPDATE Animal SET Name = '{animal.Name}', Description = '{animal.Description}', Category = '{animal.Category}', Area = '{animal.Area}' WHERE IdAnimal = {idAnimal}";
            

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            int countPutedAnimals = await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();

            return countPutedAnimals > 0;
        }
    }
}

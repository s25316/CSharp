using Lesson5.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lesson5.Servises
{
    public class ProductWarehouseDBService : IProductWarehouseDBService
    {
        private const string _connString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";


        public async Task PostProductByProcedureAsync(ProductWarehouseDTO product)
        {
            DateTime time = DateTime.Now;              // Use current time
            string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in database 
            string sql = $"EXEC AddProductToWarehouse @IdProduct = {product.IdProduct}, @IdWarehouse = {product.IdWarehouse}, @Amount = {product.Amount}, @CreatedAt = {time.ToString(format)};";
            await IsRealizedByQueryAsync(sql);
        }

        public async Task<RequestDTO> PostProductWarehouseAsync(ProductWarehouseDTO product)
        {
            if (!await IsProductIdExistAsync (product))
            {
                return new RequestDTO
                {
                    RequestCode = 404,
                    RequestMessage = $"Product z podanym id {product.IdProduct} nie istnieje w bazie"
                };
            }
            if (!await IsWarehouseExistAsync(product))
            {
                return new RequestDTO
                {
                    RequestCode = 404,
                    RequestMessage = $"Warehouse z podanym id{product.IdWarehouse}  nie istnieje w bazie"
                };
            }
            if (product.Amount <= 0)
            {
                return new RequestDTO
                {
                    RequestCode = 400,
                    RequestMessage = $"Amount musi być powyżej Zera. Wartość podana w Amount: {product.Amount}"
                };
            }
            if (!await IsOrderExistAsync(product))
            {
                return new RequestDTO
                {
                    RequestCode = 404,
                    RequestMessage = "Order z podanym Product i Amount nie istnieje"
                };
            }
            if (await IsProductHaveAddAsync(product))
            {
                return new RequestDTO
                {
                    RequestCode = 400,
                    RequestMessage = "Order został wczesniej wykonany"
                };
            }


            await IsUpdatedOrderAsync(product);
            Console.WriteLine("wd");
            await InsertIntoProductWarehouseAsync(product);

            Console.WriteLine("wd");

            string sql = "SELECT MAX(idproductwarehosue) FROM product_warehosue";
            RequestDTO request = new RequestDTO()
            {
                RequestCode = 200
            };


            Console.WriteLine("wd");

            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();           

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            
            await sqlConnection.OpenAsync();

            await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                request.RequestMessage = reader["idproductwarehosue"].ToString();
            }

            await sqlConnection.CloseAsync();
            return request;
        }


        private async Task<bool> IsExistByQueryAsync(string sql)
        {
            
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            
            await sqlConnection.CloseAsync();

            
            return Convert.ToDecimal(count) > 0 ;
        }

        private async Task IsRealizedByQueryAsync(string sql) 
        {
            
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();

            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            await sqlCommand.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();

        }


        private async Task<bool> IsProductIdExistAsync(ProductWarehouseDTO product) 
        {
            string sql = $"SELECT Count (idproduct) FROM product WHERE idproduct = {product.IdProduct}";           

            return await IsExistByQueryAsync(sql);
        }

        private async Task<bool> IsWarehouseExistAsync(ProductWarehouseDTO product)
        {
            string sql = $"SELECT Count (idproduct) FROM product WHERE idproduct = {product.IdProduct}";

            return await IsExistByQueryAsync(sql);
        }

        private async Task<bool> IsOrderExistAsync(ProductWarehouseDTO product)
        {
            string sql = $"SELECT Count (1) FROM \"order\" WHERE idproduct = {product.IdProduct} AND amount = {product.Amount}";

            bool exsist = await IsExistByQueryAsync(sql);
            if (!exsist) 
            {
                return false ;
            }
            sql = $"SELECT CreatedAt FROM \"order\" WHERE idproduct = {product.IdProduct} AND amount = {product.Amount}";
            await using SqlConnection sqlConnection = new(_connString);
            await using SqlCommand sqlCommand = new();
            
            sqlCommand.CommandText = sql;
            sqlCommand.Connection = sqlConnection;

            await sqlConnection.OpenAsync();

            await using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            while (await sqlDataReader.ReadAsync())
            {
                
                if (DateTime.Parse(product.CreatedAt.ToString()) < DateTime.Parse(sqlDataReader["CreatedAt"].ToString()))
                {
                    
                    await sqlConnection.CloseAsync();
                    return false;
                }

            }

            await sqlConnection.CloseAsync();

            return true;
        }

        private async Task<bool> IsProductHaveAddAsync(ProductWarehouseDTO product)
        {
            string sql = $"SELECT Count (1) FROM product_warehouse WHERE idproduct = {product.IdProduct}";

            return await IsExistByQueryAsync(sql);
        }

        private async Task IsUpdatedOrderAsync(ProductWarehouseDTO product)
        {
            DateTime time = DateTime.Now;              // Use current time
            string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in database 


            string sql = $"UPDATE \"order\" SET FulfilledAt = '{time.ToString(format)}' WHERE idproduct = {product.IdProduct} AND amount = {product.Amount};";
            
            await IsRealizedByQueryAsync(sql);
        }

        private async Task InsertIntoProductWarehouseAsync(ProductWarehouseDTO product) 
        {
            DateTime time = DateTime.Now;              // Use current time
            string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in database 
            /*
            string sql = $"INSERT INTO product_warehouse VALUES ((SELECT MAX(IdProductWarehouse) FROM product_warehouse) + 1, {product.IdWarehouse}, {product.IdProduct}, (SELECT idorder FROM \"order\" WHERE idproduct = {product.IdProduct} AND amount = {product.Amount})," +
                $"(SELECT price FROM product WHERE idproduct = {product.IdProduct}) * {product.Amount}, convert(datetime, (CAST('{time.ToString(format)}' AS DateTime)) ,5) )";
            */
            
            string idOrder = $"( SELECT Max (a.IdOrder) FROM \"order\" a WHERE a.IdProduct = {product.IdProduct} AND a.Amount = {product.Amount})";
            string amount = $"( SELECT Amount FROM \"order\" b WHERE b.IdOrder = {idOrder} )";

            string sql = $"""INSERT INTO Product_Warehouse(IdProductWarehouse, IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES """
             +$"( (SELECT MAX(IdProductWarehouse) FROM product_warehouse) + 1, {product.IdWarehouse}, {product.IdProduct}, {idOrder} , {product.Amount}, {int.Parse(amount) * product.Amount}, (CAST('{time.ToString(format)}' AS DateTime)) ,5) ));";
            Console.WriteLine(sql);
            await IsRealizedByQueryAsync(sql);
        }

    }
}

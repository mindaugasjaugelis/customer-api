using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Dtos;
using Customer.Application.Services.Dtos;

namespace Customer.Application.Services.DataReader
{
    public class FileDataReader : IDataReader
    {
        public async Task<ReadCustomersFileResult> Read(string path)
        {
            var readFileContentResult = await ReadFileContent(path);
            if (readFileContentResult.Success)
            {
                var deserializeFileContentToCustomersListResult =
                    DeserializeFileContentToCustomerImportList(readFileContentResult.Content);

                return new ReadCustomersFileResult(deserializeFileContentToCustomersListResult.Success)
                {
                    Customers = deserializeFileContentToCustomersListResult.Customers,
                    ErrorMessage = deserializeFileContentToCustomersListResult.ErrorMessage
                };
            }

            return new ReadCustomersFileResult(false)
            {
                ErrorMessage = readFileContentResult.ErrorMessage,
                Customers = new List<CustomerImportDto>()
            };
        }

        private static DeserializeFileContentToCustomersListResult DeserializeFileContentToCustomerImportList(string fileContent)
        {
            try
            {
                var deserializedCustomers = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<CustomerImportDto>>(fileContent)
                    ?? new List<CustomerImportDto>();

                return new DeserializeFileContentToCustomersListResult(true)
                {
                    Customers = deserializedCustomers
                };
            }
            catch (Exception ex)
            {
                return new DeserializeFileContentToCustomersListResult(false)
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        private static async Task<ReadFileContentResult> ReadFileContent(string path)
        {
            try
            {
                var fileContent = await File.ReadAllTextAsync(path);
                return new ReadFileContentResult(true)
                {
                    Content = fileContent
                };
            }
            catch (Exception ex)
            {
                return new ReadFileContentResult(false)
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}

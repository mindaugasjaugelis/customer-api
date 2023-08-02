using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Dtos;
using Customer.Application.Services.Dtos;

namespace Customer.Application.Services.DataReader
{
    public class FileDataReader : IDataReader
    {
        public async Task<ReadCustomersFileResult> Read(string path)
        {
            ReadFileContentResult readFileContentResult = await ReadFileContent(path);
            if (readFileContentResult.Success)
            {
                DeserializeFileContentToCustomersListResult deserializeFileContentToCustomersListResult =
                    DeserializeFileContentToCustomerImportList(readFileContentResult.Content);

                return new ReadCustomersFileResult(deserializeFileContentToCustomersListResult.Success)
                {
                    Customers = deserializeFileContentToCustomersListResult.Customers,
                    ErrorMessage = deserializeFileContentToCustomersListResult.ErrorMessage
                };
            }

            return new ReadCustomersFileResult(false)
            {
                Success = false,
                ErrorMessage = readFileContentResult.ErrorMessage
            };
        }

        private static DeserializeFileContentToCustomersListResult DeserializeFileContentToCustomerImportList(string fileContent)
        {
            try
            {
                List<CustomerImportDto> deserializedCustomers = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<CustomerImportDto>>(fileContent)
                    ?? new List<CustomerImportDto>();

                return new DeserializeFileContentToCustomersListResult
                {
                    Customers = deserializedCustomers
                };
            }
            catch (Exception ex)
            {
                return new DeserializeFileContentToCustomersListResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static async Task<ReadFileContentResult> ReadFileContent(string path)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(path);
                return new ReadFileContentResult
                {
                    Content = fileContent
                };
            }
            catch (Exception ex)
            {
                return new ReadFileContentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}

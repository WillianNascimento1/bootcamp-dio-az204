using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fnPostDataStorage
{
    public static class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("dataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Subindo imagem para o hd!");


            if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
            {
                return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório");
            }

            var fileType = fileTypeHeader.ToString();
            var form = await req.ReadFormAsync();
            var file = form.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("O arquivo não foi enviado");
            }

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = fileType;
            BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
            BlobContainerClient containerClient new BlobContainerClient(connectionString, containerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            string blobName = file.FileName;
            var blob = containerClient.GetBlobClient(blobName);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }

            _logger.LogInformation($"Arquivo {file.FileName} armazenado com sucesso");

            return new OkObjectResult(new
            {
                Message "Arquivo armazęnado com sucesso",
                BlobUri = blob.Uri
            });
        }
    }
}

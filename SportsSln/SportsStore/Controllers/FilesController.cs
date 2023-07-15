using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System.Numerics;
using SportsStore.Services;
using static SportsStore.Services.FileUploadService;

namespace SportsStore.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {

        private readonly IAmazonS3 _s3Client;
        private StoreDbContext _dbContext;
        private string bucketName = "fish-b11";


        public FilesController(IAmazonS3 s3Client, StoreDbContext ctx)
        {
            _s3Client = s3Client;
            _dbContext = ctx;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, long? productId)

            
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            

            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            // Get the file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            // Determine the file type based on the extension
            var fileType = GetFileType(extension);
            if (fileType == FileType.Unknown)
            {

                return NotFound($"File with {fileType} not allowed");
            }
            var key = GenerateUniqueKey(file.FileName);

            // Check exist product before upload image to S3
            var existingProduct = await _dbContext.Products.FindAsync(productId);
            if (existingProduct == null)
            {
                // Product not found
                return NotFound();
            }

            string Key = $"{fileType}/{key}";

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = Key,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            var result = await _s3Client.PutObjectAsync(request);
            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {

                // Delete file exist on S3
                await _s3Client.DeleteObjectAsync(bucketName, existingProduct.Image);

                // Update the product properties
                existingProduct.Image = Key;
                existingProduct.PresignedUrl = $"https://{bucketName}.s3.amazonaws.com/{fileType}/{key}";

                // Save the changes to the database
                await _dbContext.SaveChangesAsync();

                // Return the updated product
                return Ok(existingProduct);
            }
            else
            {
                return BadRequest("Upload Failed!");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilesAsync(string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new S3ObjectDto()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });
            return Ok(s3Objects);
        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetFileByKeyAsync(string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFileAsync(string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
            await _s3Client.DeleteObjectAsync(bucketName, key);
            return NoContent();
        }

        private FileType GetFileType(string extension)
        {
            if (IsImageExtension(extension))
            {
                return FileType.Image;
            }
            else if (IsDocumentExtension(extension))
            {
                return FileType.Document;
            }
            else if (IsVideoExtension(extension))
            {
                return FileType.Video;
            }

            return FileType.Unknown;
        }

        private bool IsImageExtension(string extension)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            return allowedExtensions.Contains(extension);
        }

        private bool IsDocumentExtension(string extension)
        {
            string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".txt" };
            return allowedExtensions.Contains(extension);
        }

        private bool IsVideoExtension(string extension)
        {
            string[] allowedExtensions = { ".mp4", ".avi", ".mov", ".wmv" };
            return allowedExtensions.Contains(extension);
        }

        private string GenerateUniqueKey(string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var key = $"{guid}-{fileName}";
            return key;
        }

        public enum FileType
        {
            Unknown,
            Image,
            Document,
            Video
        }
    }
}

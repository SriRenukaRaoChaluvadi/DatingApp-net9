using System;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}

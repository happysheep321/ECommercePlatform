using ECommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Identity.API.Application.Services
{
    public interface IAvatarService
    {
        Task<string> UploadAvatarAsync(IFormFile file, Guid userId);
        Task<byte[]?> GetAvatarAsync(string avatarUrl);
        Task<byte[]?> GetDefaultAvatarAsync();
        Task DeleteAvatarAsync(string avatarUrl);
    }

    public class AvatarService : IAvatarService
    {
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<AvatarService> logger;

        public AvatarService(IWebHostEnvironment environment, ILogger<AvatarService> logger)
        {
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, Guid userId)
        {
            try
            {
                // 创建上传目录
                var uploadsDir = Path.Combine(environment.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsDir);

                // 删除旧头像
                await DeleteOldAvatarAsync(userId, uploadsDir);

                // 生成唯一文件名
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                logger.LogInformation("用户 {UserId} 上传头像成功: {FileName}", userId, fileName);

                // 返回相对路径
                return $"/uploads/avatars/{fileName}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "用户 {UserId} 上传头像失败", userId);
                throw;
            }
        }

        public async Task<byte[]?> GetAvatarAsync(string avatarUrl)
        {
            if (string.IsNullOrEmpty(avatarUrl))
                return null;

            try
            {
                // 如果是本地文件路径
                if (avatarUrl.StartsWith("/uploads/"))
                {
                    var filePath = Path.Combine(environment.WebRootPath, avatarUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        return await File.ReadAllBytesAsync(filePath);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取头像失败: {AvatarUrl}", avatarUrl);
                return null;
            }
        }

        public async Task<byte[]?> GetDefaultAvatarAsync()
        {
            try
            {
                var defaultAvatarPath = Path.Combine(environment.WebRootPath, "images", "default-avatar.png");
                if (File.Exists(defaultAvatarPath))
                {
                    return await File.ReadAllBytesAsync(defaultAvatarPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取默认头像失败");
                return null;
            }
        }

        public Task DeleteAvatarAsync(string avatarUrl)
        {
            if (string.IsNullOrEmpty(avatarUrl) || !avatarUrl.StartsWith("/uploads/"))
                return Task.CompletedTask;

            try
            {
                var filePath = Path.Combine(environment.WebRootPath, avatarUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    logger.LogInformation("删除头像文件: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除头像文件失败: {AvatarUrl}", avatarUrl);
            }

            return Task.CompletedTask;
        }

        private Task DeleteOldAvatarAsync(Guid userId, string uploadsDir)
        {
            try
            {
                var oldFiles = Directory.GetFiles(uploadsDir, $"{userId}_*");
                foreach (var oldFile in oldFiles)
                {
                    File.Delete(oldFile);
                    logger.LogInformation("删除旧头像文件: {OldFile}", oldFile);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除用户 {UserId} 的旧头像失败", userId);
            }

            return Task.CompletedTask;
        }
    }
}

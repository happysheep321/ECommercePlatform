using ECommerce.BuildingBolcks.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Identity.API.Controllers
{
    /// <summary>
    /// EF Core 迁移管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 只允许管理员访问
    public class MigrationController : ControllerBase
    {
        private readonly IMigrationService _migrationService;
        private readonly ILogger<MigrationController> _logger;

        public MigrationController(IMigrationService migrationService, ILogger<MigrationController> logger)
        {
            _migrationService = migrationService;
            _logger = logger;
        }

        /// <summary>
        /// 创建新的迁移
        /// </summary>
        /// <param name="request">创建迁移请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateMigration([FromBody] CreateMigrationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.MigrationName))
                {
                    return BadRequest(new { message = "迁移名称不能为空" });
                }

                // 获取当前项目路径
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await _migrationService.CreateMigrationAsync(
                    request.MigrationName, 
                    projectPath);

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = result.Message,
                        output = result.Output 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message,
                        error = result.Error 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "创建迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("update-database")]
        public async Task<IActionResult> UpdateDatabase()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await _migrationService.UpdateDatabaseAsync(projectPath);

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = result.Message,
                        output = result.Output 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message,
                        error = result.Error 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新数据库时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "更新数据库时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 移除最后一个迁移
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("remove-last")]
        public async Task<IActionResult> RemoveLastMigration()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await _migrationService.RemoveLastMigrationAsync(projectPath);

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = result.Message,
                        output = result.Output 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message,
                        error = result.Error 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "移除迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 生成SQL脚本
        /// </summary>
        /// <param name="request">生成脚本请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("generate-script")]
        public async Task<IActionResult> GenerateScript([FromBody] GenerateScriptRequest request)
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await _migrationService.GenerateScriptAsync(
                    projectPath, 
                    null,
                    request.OutputPath);

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = result.Message,
                        output = result.Output 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message,
                        error = result.Error 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成SQL脚本时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "生成SQL脚本时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 列出所有迁移
        /// </summary>
        /// <returns>迁移列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> ListMigrations()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await _migrationService.ListMigrationsAsync(projectPath);

                if (result.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = result.Message,
                        migrations = result.Migrations,
                        output = result.Output 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message,
                        error = result.Error 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "列出迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 获取迁移状态信息
        /// </summary>
        /// <returns>迁移状态</returns>
        [HttpGet("status")]
        public async Task<IActionResult> GetMigrationStatus()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                // 获取迁移列表
                var listResult = await _migrationService.ListMigrationsAsync(projectPath);
                
                var status = new
                {
                    projectPath = projectPath,
                    totalMigrations = listResult.Success ? listResult.Migrations.Count : 0,
                    migrations = listResult.Success ? listResult.Migrations : new List<string>(),
                    lastUpdate = DateTime.UtcNow
                };

                return Ok(new { 
                    success = true, 
                    message = "获取迁移状态成功",
                    data = status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取迁移状态时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "获取迁移状态时发生异常",
                    error = ex.Message 
                });
            }
        }
    }

    /// <summary>
    /// 创建迁移请求
    /// </summary>
    public class CreateMigrationRequest
    {
        /// <summary>
        /// 迁移名称
        /// </summary>
        public string MigrationName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 生成脚本请求
    /// </summary>
    public class GenerateScriptRequest
    {
        /// <summary>
        /// 输出文件路径（可选）
        /// </summary>
        public string? OutputPath { get; set; }
    }
}

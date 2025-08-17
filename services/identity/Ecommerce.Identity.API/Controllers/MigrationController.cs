using ECommerce.BuildingBolcks.EFCore;
using ECommerce.Identity.API.Application.Commands;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace ECommerce.Identity.API.Controllers
{
    /// <summary>
    /// EF Core 迁移管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 只允许管理员访问
    public class MigrationController : ControllerBase
    {
        private readonly IMigrationService migrationService;
        private readonly ILogger<MigrationController> logger;
        private readonly IMediator mediator;

        public MigrationController(IMigrationService migrationService, ILogger<MigrationController> logger, IMediator mediator)
        {
            this.migrationService = migrationService;
            this.logger = logger;
            this.mediator = mediator;
        }

        /// <summary>
        /// 创建新的EF Core迁移
        /// </summary>
        /// <param name="command">创建迁移命令</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 创建新的EF Core数据库迁移文件
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "migrationName": "AddUserTable"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> CreateMigration([FromBody] CreateMigrationCommand command)
        {
            try
            {
                await mediator.Send(command);
                
                // 获取当前项目路径
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await migrationService.CreateMigrationAsync(
                    command.MigrationName, 
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
                logger.LogError(ex, "创建迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "创建迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 更新数据库到最新迁移
        /// </summary>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 将数据库更新到最新的EF Core迁移版本
        /// </remarks>
        [HttpPost("update-database")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> UpdateDatabase()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await migrationService.UpdateDatabaseAsync(projectPath);

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
                logger.LogError(ex, "更新数据库时发生异常");
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
        /// <remarks>
        /// 移除最后一个EF Core迁移文件（如果尚未应用到数据库）
        /// </remarks>
        [HttpPost("remove-last")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> RemoveLastMigration()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await migrationService.RemoveLastMigrationAsync(projectPath);

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
                logger.LogError(ex, "移除迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "移除迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 生成EF Core迁移SQL脚本
        /// </summary>
        /// <param name="command">生成脚本命令</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 生成EF Core迁移的SQL脚本，可选择输出到文件
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "outputPath": "migration-script.sql"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("generate-script")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> GenerateScript([FromBody] GenerateScriptCommand command)
        {
            try
            {
                await mediator.Send(command);
                
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await migrationService.GenerateScriptAsync(
                    projectPath, 
                    null,
                    command.OutputPath);

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
                logger.LogError(ex, "生成SQL脚本时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "生成SQL脚本时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 列出所有EF Core迁移
        /// </summary>
        /// <returns>迁移列表</returns>
        /// <remarks>
        /// 获取当前项目的所有EF Core迁移文件列表
        /// </remarks>
        [HttpGet("list")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> ListMigrations()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                var result = await migrationService.ListMigrationsAsync(projectPath);

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
                logger.LogError(ex, "列出迁移时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "列出迁移时发生异常",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// 获取EF Core迁移状态信息
        /// </summary>
        /// <returns>迁移状态</returns>
        /// <remarks>
        /// 获取当前项目的EF Core迁移状态，包括迁移总数和列表
        /// </remarks>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> GetMigrationStatus()
        {
            try
            {
                var projectPath = Directory.GetCurrentDirectory();
                
                // 获取迁移列表
                var listResult = await migrationService.ListMigrationsAsync(projectPath);
                
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
                logger.LogError(ex, "获取迁移状态时发生异常");
                return StatusCode(500, new { 
                    success = false, 
                    message = "获取迁移状态时发生异常",
                    error = ex.Message 
                });
            }
        }
    }
}

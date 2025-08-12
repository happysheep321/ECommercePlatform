using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace ECommerce.BuildingBolcks.EFCore
{
    /// <summary>
    /// EF Core 迁移服务实现
    /// </summary>
    public class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;

        public MigrationService(ILogger<MigrationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建新的迁移
        /// </summary>
        public async Task<MigrationResult> CreateMigrationAsync(string migrationName, string projectPath, string? startupProjectPath = null)
        {
            try
            {
                _logger.LogInformation("开始创建迁移: {MigrationName} 在项目: {ProjectPath}", migrationName, projectPath);

                var arguments = $"ef migrations add \"{migrationName}\" --project \"{projectPath}\"";
                if (!string.IsNullOrEmpty(startupProjectPath))
                {
                    arguments += $" --startup-project \"{startupProjectPath}\"";
                }

                var result = await ExecuteDotNetCommandAsync(arguments, projectPath);

                if (result.Success)
                {
                    _logger.LogInformation("迁移创建成功: {MigrationName}", migrationName);
                    result.Message = $"迁移 '{migrationName}' 创建成功";
                }
                else
                {
                    _logger.LogError("迁移创建失败: {MigrationName}, 错误: {Error}", migrationName, result.Error);
                    result.Message = $"迁移 '{migrationName}' 创建失败";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建迁移时发生异常: {MigrationName}", migrationName);
                return new MigrationResult
                {
                    Success = false,
                    Message = $"创建迁移时发生异常: {ex.Message}",
                    Error = ex.ToString()
                };
            }
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        public async Task<MigrationResult> UpdateDatabaseAsync(string projectPath, string? startupProjectPath = null)
        {
            try
            {
                _logger.LogInformation("开始更新数据库，项目路径: {ProjectPath}", projectPath);

                var arguments = "ef database update --project \"" + projectPath + "\"";
                if (!string.IsNullOrEmpty(startupProjectPath))
                {
                    arguments += $" --startup-project \"{startupProjectPath}\"";
                }

                var result = await ExecuteDotNetCommandAsync(arguments, projectPath);

                if (result.Success)
                {
                    _logger.LogInformation("数据库更新成功");
                    result.Message = "数据库更新成功";
                }
                else
                {
                    _logger.LogError("数据库更新失败，错误: {Error}", result.Error);
                    result.Message = "数据库更新失败";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新数据库时发生异常");
                return new MigrationResult
                {
                    Success = false,
                    Message = $"更新数据库时发生异常: {ex.Message}",
                    Error = ex.ToString()
                };
            }
        }

        /// <summary>
        /// 移除最后一个迁移
        /// </summary>
        public async Task<MigrationResult> RemoveLastMigrationAsync(string projectPath, string? startupProjectPath = null)
        {
            try
            {
                _logger.LogInformation("开始移除最后一个迁移，项目路径: {ProjectPath}", projectPath);

                var arguments = "ef migrations remove --project \"" + projectPath + "\"";
                if (!string.IsNullOrEmpty(startupProjectPath))
                {
                    arguments += $" --startup-project \"{startupProjectPath}\"";
                }

                var result = await ExecuteDotNetCommandAsync(arguments, projectPath);

                if (result.Success)
                {
                    _logger.LogInformation("最后一个迁移移除成功");
                    result.Message = "最后一个迁移移除成功";
                }
                else
                {
                    _logger.LogError("移除迁移失败，错误: {Error}", result.Error);
                    result.Message = "移除迁移失败";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除迁移时发生异常");
                return new MigrationResult
                {
                    Success = false,
                    Message = $"移除迁移时发生异常: {ex.Message}",
                    Error = ex.ToString()
                };
            }
        }

        /// <summary>
        /// 生成SQL脚本
        /// </summary>
        public async Task<MigrationResult> GenerateScriptAsync(string projectPath, string? startupProjectPath = null, string? outputPath = null)
        {
            try
            {
                _logger.LogInformation("开始生成SQL脚本，项目路径: {ProjectPath}", projectPath);

                var arguments = "ef migrations script --project \"" + projectPath + "\"";
                if (!string.IsNullOrEmpty(startupProjectPath))
                {
                    arguments += $" --startup-project \"{startupProjectPath}\"";
                }
                if (!string.IsNullOrEmpty(outputPath))
                {
                    arguments += $" --output \"{outputPath}\"";
                }

                var result = await ExecuteDotNetCommandAsync(arguments, projectPath);

                if (result.Success)
                {
                    _logger.LogInformation("SQL脚本生成成功");
                    result.Message = "SQL脚本生成成功";
                }
                else
                {
                    _logger.LogError("SQL脚本生成失败，错误: {Error}", result.Error);
                    result.Message = "SQL脚本生成失败";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成SQL脚本时发生异常");
                return new MigrationResult
                {
                    Success = false,
                    Message = $"生成SQL脚本时发生异常: {ex.Message}",
                    Error = ex.ToString()
                };
            }
        }

        /// <summary>
        /// 列出所有迁移
        /// </summary>
        public async Task<MigrationListResult> ListMigrationsAsync(string projectPath, string? startupProjectPath = null)
        {
            try
            {
                _logger.LogInformation("开始列出迁移，项目路径: {ProjectPath}", projectPath);

                var arguments = "ef migrations list --project \"" + projectPath + "\"";
                if (!string.IsNullOrEmpty(startupProjectPath))
                {
                    arguments += $" --startup-project \"{startupProjectPath}\"";
                }

                var result = await ExecuteDotNetCommandAsync(arguments, projectPath);
                var listResult = new MigrationListResult
                {
                    Success = result.Success,
                    Message = result.Message,
                    Output = result.Output,
                    Error = result.Error
                };

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    // 解析迁移列表输出
                    var lines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("Build"))
                        {
                            listResult.Migrations.Add(trimmedLine);
                        }
                    }
                }

                return listResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出迁移时发生异常");
                return new MigrationListResult
                {
                    Success = false,
                    Message = $"列出迁移时发生异常: {ex.Message}",
                    Error = ex.ToString()
                };
            }
        }

        /// <summary>
        /// 执行dotnet命令
        /// </summary>
        private async Task<MigrationResult> ExecuteDotNetCommandAsync(string arguments, string workingDirectory)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                return new MigrationResult
                {
                    Success = process.ExitCode == 0,
                    Output = output,
                    Error = error,
                    Message = process.ExitCode == 0 ? "命令执行成功" : "命令执行失败"
                };
            }
            catch (Exception ex)
            {
                return new MigrationResult
                {
                    Success = false,
                    Error = ex.ToString(),
                    Message = $"执行命令时发生异常: {ex.Message}"
                };
            }
        }
    }
}

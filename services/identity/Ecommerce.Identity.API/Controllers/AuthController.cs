using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace ECommerce.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IUserService userService;
        private readonly IVerificationCodeService emailVerificationService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IUserService userService, IVerificationCodeService emailVerificationService , ILogger<AuthController> logger)
        {
            this.userService = userService;
            this.emailVerificationService = emailVerificationService;
            this.logger = logger;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="command">注册指令</param>
        /// <returns>用户Id</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Guid),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> RegisterAsync([FromBody]RegisterUserCommand command)
        {
            try
            {
                var userId = await userService.RegisterAsync(command);
                return Ok(userId);
            }
            catch (ArgumentException ex) // 参数问题
            {
                logger.LogWarning(ex, "注册参数无效");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "用户注册失败");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="command">登录信息</param>
        /// <returns>登录结果</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResultDto>> LoginAsync([FromBody]LoginUserCommand command)
        {
            try
            {
                var result = await userService.LoginAsync(command);
                if (result == null)
                {
                    return Unauthorized("用户名或密码错误");
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "未授权的登录尝试");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "用户登录失败");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        [HttpPost("send-code")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmailCode([FromBody] EmailCodeCommand command)
        {
            try
            {
                var success = await emailVerificationService.SendCodeAsync(command.Email);
                if (!success)
                {
                    var msg = $"验证码发送失败，目标邮箱：{command.Email}";
                    logger.LogWarning(msg);
                    return StatusCode(500, msg);
                }

                return Ok("验证码已发送");
            }
            catch (SmtpException ex)
            {
                logger.LogError(ex, "SMTP 邮件服务异常");
                return StatusCode(500, "邮件服务器异常，请稍后再试");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "发送验证码失败：{Message}", ex.Message);
                return StatusCode(500, $"服务器内部错误：{ex.Message}");
            }
        }
    }
}

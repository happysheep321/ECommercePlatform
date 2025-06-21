using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IUserService userService;
        private readonly ISmsCodeService smsCodeService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IUserService userService,ISmsCodeService smsCodeService, ILogger<AuthController> logger)
        {
            this.userService = userService;
            this.smsCodeService = smsCodeService;
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

        /// <summary>
        /// 发送注册验证码
        /// </summary>
        /// <param name="command">包含手机号的命令</param>
        /// <returns>发送结果</returns>
        [HttpPost("send-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendCodeAsync([FromBody] SendSmsCodeCommand command)
        {
            try
            {
                await smsCodeService.SendRegisterCodeAsync(command.Phone);
                return Ok(new { Message = "验证码已发送" });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "验证码发送失败");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "发送验证码过程中发生错误");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }
    }
}

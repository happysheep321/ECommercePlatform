using ECommerce.Identity.API.Application.Commands;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using MediatR;

namespace ECommerce.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IUserService userService;
        private readonly IVerificationCodeService emailVerificationService;
        private readonly ILogger<AuthController> logger;
        private readonly IMediator mediator;

        public AuthController(IUserService userService, IVerificationCodeService emailVerificationService, ILogger<AuthController> logger, IMediator mediator)
        {
            this.userService = userService;
            this.emailVerificationService = emailVerificationService;
            this.logger = logger;
            this.mediator = mediator;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="command">注册指令</param>
        /// <returns>用户Id</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Guid>>> RegisterAsync([FromBody]RegisterUserCommand command)
        {
            var userId = await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.Ok(userId, "Created"));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="command">登录信息</param>
        /// <returns>登录结果</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LoginResultDto>>> LoginAsync([FromBody]LoginUserCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(ApiResponse<LoginResultDto>.Ok(result));
        }

        /// <summary>
        /// 忘记密码 - 通过邮箱重置密码
        /// </summary>
        /// <param name="command">忘记密码指令，包含邮箱地址</param>
        /// <returns>重置密码操作结果</returns>
        /// <remarks>
        /// 此接口会向指定邮箱发送密码重置链接，用户可以通过邮件中的链接重置密码
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "email": "user@example.com"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "密码重置成功"));
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="command">邮箱验证码指令</param>
        /// <returns>验证码发送结果</returns>
        /// <remarks>
        /// 向指定邮箱发送验证码，用于用户注册、登录或其他需要邮箱验证的场景
        /// 验证码有效期为5分钟
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "email": "user@example.com"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("send-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> SendEmailCode([FromBody] EmailCodeCommand command)
        {
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "验证码已发送"));
        }
    }
}

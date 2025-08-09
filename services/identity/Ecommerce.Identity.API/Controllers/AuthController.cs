using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
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
        [ProducesResponseType(typeof(Guid),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<Guid>>> RegisterAsync([FromBody]RegisterUserCommand command)
        {
            var userId = await userService.RegisterAsync(command);
            return StatusCode(StatusCodes.Status201Created, ECommerce.SharedKernel.DTOs.ApiResponse<Guid>.Ok(userId, "Created"));
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
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<LoginResultDto>>> LoginAsync([FromBody]LoginUserCommand command)
        {
            var result = await userService.LoginAsync(command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<LoginResultDto>.Ok(result));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await userService.ResetPasswordByEmailAsync(command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "密码重置成功"));
        }

        [HttpPost("send-code")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmailCode([FromBody] EmailCodeCommand command)
        {
            var success = await emailVerificationService.SendCodeAsync(command.Email);
            if (!success)
            {
                return StatusCode(500, ECommerce.SharedKernel.DTOs.ApiResponse<string>.Fail("MAIL_SEND_FAILED", $"验证码发送失败，{command.Email}"));
            }
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "验证码已发送"));
        }
    }
}

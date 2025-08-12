using ECommerce.Identity.API.Application.Commands;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using ECommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using ECommerce.Identity.API.Domain.Repositories;
using ECommerce.SharedKernel.Interfaces;

namespace ECommerce.Identity.API.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RoleService> logger;

        public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork, ILogger<RoleService> logger)
        {
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Guid> CreateRoleAsync(CreateRoleCommand command)
        {
            if (await roleRepository.ExistsByNameAsync(command.Name))
                throw new InvalidOperationException("角色名称已存在");

            var role=new Role(Guid.NewGuid(),command.Name,command.Description);

            // 自动赋予基础权限
            var defaultPermissions = await permissionRepository.GetDefaultPermissionsAsync();
            role.ReplacePermissions(defaultPermissions);

            await roleRepository.AddAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("创建角色成功：{RoleName} ({RoleId})", role.Name, role.Id);
            return role.Id;
        }

        public async Task UpdateRoleAsync(UpdateRoleCommand command)
        {
            var role=await roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                throw new InvalidOperationException("角色不存在");

            // 名称冲突校验（除自身）
            if (!string.Equals(role.Name, command.Name, StringComparison.OrdinalIgnoreCase)
                && await roleRepository.ExistsByNameAsync(command.Name))
                throw new InvalidOperationException("角色名称已存在");

            role.UpdateRole(command.Name, command.Description);
            roleRepository.Update(role);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("更新角色成功：{RoleName} ({RoleId})", role.Name, role.Id);
        }

        public async Task DeleteRoleAsync(DeleteRoleCommand command)
        {
            var role = await roleRepository.GetByIdAsync(command.RoleId);
            if (role == null) throw new InvalidOperationException("角色不存在");
            if (role.IsSystemRole) throw new InvalidOperationException("系统内置角色不可删除");

            roleRepository.Delete(role);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("删除角色成功：{RoleName} ({RoleId})", role.Name, role.Id);
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await roleRepository.GetAllAsync();
            return roles.Select(r => new RoleDto
            {
                RoleId = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsEnabled = r.Enabled,
                IsSystem = r.IsSystemRole
            }).ToList();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(GetRoleByIdCommand command)
        {
            var role = await roleRepository.GetByIdAsync(command.RoleId);
            if (role == null) return null;
            return new RoleDto
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsEnabled = role.Enabled,
                IsSystem = role.IsSystemRole
            };
        }

        public async Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            var perms = await roleRepository.GetPermissionsByRoleIdAsync(roleId);
            return perms.Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                Name = p.Name ?? string.Empty,
                Code = p.Name ?? string.Empty,
                Description = p.Description,
                IsEnabled = p.Enabled,
                IsSystemPermission = false
            }).ToList();
        }

        public async Task AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds)
        {
            var role = await roleRepository.GetByIdAsync(roleId) ?? throw new InvalidOperationException("角色不存在");
            var permissions = new List<Permission>();
            foreach (var pid in permissionIds.Distinct())
            {
                var perm = await permissionRepository.GetByIdAsync(pid);
                if (perm != null) permissions.Add(perm);
            }
            role.ReplacePermissions(permissions);
            roleRepository.Update(role);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
        {
            var role = await roleRepository.GetByIdAsync(roleId) ?? throw new InvalidOperationException("角色不存在");
            var removed = role.RemovePermission(permissionId);
            if (removed)
            {
                roleRepository.Update(role);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task EnableAsync(EnableRoleCommand command)
        {
            var role = await roleRepository.GetByIdAsync(command.RoleId) ?? throw new InvalidOperationException("角色不存在");
            role.Enable();
            roleRepository.Update(role);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DisableAsync(DisableRoleCommand command)
        {
            var role = await roleRepository.GetByIdAsync(command.RoleId) ?? throw new InvalidOperationException("角色不存在");
            if (role.IsSystemRole) throw new InvalidOperationException("系统内置角色不可禁用");
            role.Disable();
            roleRepository.Update(role);
            await unitOfWork.SaveChangesAsync();
        }
    }
}

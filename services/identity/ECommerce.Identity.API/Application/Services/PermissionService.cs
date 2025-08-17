using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using ECommerce.Identity.API.Domain.Repositories;
using ECommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Identity.API.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository permissionRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            this.permissionRepository = permissionRepository;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await permissionRepository.GetAllAsync();
            return permissions.Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                Name = p.DisplayName ?? p.Name ?? string.Empty,
                Code = p.Name ?? string.Empty,
                Description = p.Description,
                IsEnabled = p.Enabled,
                IsSystemPermission = false
            }).ToList();
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission == null) return null;

            return new PermissionDto
            {
                PermissionId = permission.Id,
                Name = permission.DisplayName ?? permission.Name ?? string.Empty,
                Code = permission.Name ?? string.Empty,
                Description = permission.Description,
                IsEnabled = permission.Enabled,
                IsSystemPermission = false
            };
        }

        public async Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            var permissions = await roleRepository.GetPermissionsByRoleIdAsync(roleId);
            return permissions.Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                Name = p.DisplayName ?? p.Name ?? string.Empty,
                Code = p.Name ?? string.Empty,
                Description = p.Description,
                IsEnabled = p.Enabled,
                IsSystemPermission = false
            }).ToList();
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // 获取用户的所有角色ID
            var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            if (!roleIds.Any()) return false;

            // 一次性获取所有角色的权限
            var allRolePermissions = new List<Permission>();
            foreach (var roleId in roleIds)
            {
                var rolePermissions = await roleRepository.GetPermissionsByRoleIdAsync(roleId);
                allRolePermissions.AddRange(rolePermissions);
            }

            // 检查是否有匹配的权限
            return allRolePermissions.Any(p => p.Name == permissionCode && p.Enabled);
        }
    }
}

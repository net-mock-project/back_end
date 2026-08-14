using System;

namespace RescueHub.Domain.Entities
{
    /// <summary>
    /// Vai trò (Role) của người dùng trong hệ thống RescueHub.
    /// Đây là phía "1" trong quan hệ 1 - N với User.
    /// Một Role có thể được gán cho nhiều User.
    /// </summary>
    public class Role
    {
        public Guid RoleId { get; private set; }

        public string Name { get; private set; } = null!;

        public string? Description { get; private set; }


        // Constructor rỗng phục vụ EF Core / mapping ngược từ database.
        private Role() { }


        // Tạo mới một Role.
        public Role(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Role name cannot be empty.",
                    nameof(name));

            Name = name;
            Description = description;
        }


        // Dựng lại Role từ dữ liệu database.
        public Role(
            Guid roleId,
            string name,
            string? description)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Role name cannot be empty.",
                    nameof(name));

            RoleId = roleId;
            Name = name;
            Description = description;
        }


        /// Cập nhật thông tin của Role.
        public void UpdateDetails(
            string name,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Role name cannot be empty.",
                    nameof(name));

            Name = name;
            Description = description;
        }
    }
}
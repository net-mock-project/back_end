namespace RescueHub.Domain.Common.Constants
{
    public static class RoleConstants
    {
        public static readonly Guid RequesterId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid VolunteerId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid CoordinatorId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid AdminId = Guid.Parse("10000000-0000-0000-0000-000000000004");

        public const string Requester = "Requester";
        public const string Volunteer = "Volunteer";
        public const string Coordinator = "Coordinator";
        public const string Admin = "Admin";
    }
}
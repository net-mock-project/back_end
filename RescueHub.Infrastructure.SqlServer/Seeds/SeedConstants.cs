using NetTopologySuite.Geometries;

namespace RescueHub.Infrastructure.SqlServer.Seeds
{
    public static class SeedLocation
    {
        public static Point Create(double longitude, double latitude)
        {
            return new Point(longitude, latitude)
            {
                SRID = 4326
            };
        }
    }

    public static class SeedConstants
    {
        // Roles
        public static readonly Guid RequesterRole =
            Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid VolunteerRole =
            Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid CoordinatorRole =
            Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid AdminRole =
            Guid.Parse("10000000-0000-0000-0000-000000000004");

        // Users
        public static readonly Guid RequesterUser =
            Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid VolunteerUser1 =
            Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid VolunteerUser2 =
            Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid VolunteerUser3 =
            Guid.Parse("20000000-0000-0000-0000-000000000004");
        public static readonly Guid VolunteerUser4 =
            Guid.Parse("20000000-0000-0000-0000-000000000005");
        public static readonly Guid VolunteerUser5 = Guid.Parse("20000000-0000-0000-0000-000000000009");
        public static readonly Guid VolunteerUser6 = Guid.Parse("20000000-0000-0000-0000-000000000010");
        public static readonly Guid VolunteerUser7 = Guid.Parse("20000000-0000-0000-0000-000000000011");
        public static readonly Guid VolunteerUser8 = Guid.Parse("20000000-0000-0000-0000-000000000012");
        public static readonly Guid VolunteerUser9 = Guid.Parse("20000000-0000-0000-0000-000000000013");
        public static readonly Guid VolunteerUser10 = Guid.Parse("20000000-0000-0000-0000-000000000014");
        public static readonly Guid VolunteerUser11 = Guid.Parse("20000000-0000-0000-0000-000000000015");
        public static readonly Guid VolunteerUser12 = Guid.Parse("20000000-0000-0000-0000-000000000016");
        public static readonly Guid VolunteerUser13 = Guid.Parse("20000000-0000-0000-0000-000000000017");
        public static readonly Guid VolunteerUser14 = Guid.Parse("20000000-0000-0000-0000-000000000018");
        public static readonly Guid CoordinatorUser =
            Guid.Parse("20000000-0000-0000-0000-000000000006");
        public static readonly Guid AdminUser =
            Guid.Parse("20000000-0000-0000-0000-000000000007");
        public static readonly Guid InactiveUser =
            Guid.Parse("20000000-0000-0000-0000-000000000008");

        // Skills
        public static readonly Guid FirstAidSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid MedicalSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid DrivingSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid SwimmingSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid RescueSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000005");
        public static readonly Guid LogisticsSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000006");
        public static readonly Guid CommunicationSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000007");
        public static readonly Guid CookingSkill =
            Guid.Parse("30000000-0000-0000-0000-000000000008");

        // Volunteers
        public static readonly Guid Volunteer1 =
            VolunteerUser1;
        public static readonly Guid Volunteer2 =
            VolunteerUser2;
        public static readonly Guid Volunteer3 =
            VolunteerUser3;
        public static readonly Guid Volunteer4 =
            VolunteerUser4;
        public static readonly Guid Volunteer5 = VolunteerUser5;
        public static readonly Guid Volunteer6 = VolunteerUser6;
        public static readonly Guid Volunteer7 = VolunteerUser7;
        public static readonly Guid Volunteer8 = VolunteerUser8;
        public static readonly Guid Volunteer9 = VolunteerUser9;
        public static readonly Guid Volunteer10 = VolunteerUser10;
        public static readonly Guid Volunteer11 = VolunteerUser11;
        public static readonly Guid Volunteer12 = VolunteerUser12;
        public static readonly Guid Volunteer13 = VolunteerUser13;
        public static readonly Guid Volunteer14 = VolunteerUser14;

        // Relief Requests
        public static readonly Guid Request1 =
            Guid.Parse("40000000-0000-0000-0000-000000000001");
        public static readonly Guid Request2 =
            Guid.Parse("40000000-0000-0000-0000-000000000002");
        public static readonly Guid Request3 =
            Guid.Parse("40000000-0000-0000-0000-000000000003");
        public static readonly Guid Request4 =
            Guid.Parse("40000000-0000-0000-0000-000000000004");
        public static readonly Guid Request5 =
            Guid.Parse("40000000-0000-0000-0000-000000000005");

        // Relief Tasks
        public static readonly Guid Task1 =
            Guid.Parse("50000000-0000-0000-0000-000000000001");
        public static readonly Guid Task2 =
            Guid.Parse("50000000-0000-0000-0000-000000000002");
        public static readonly Guid Task3 =
            Guid.Parse("50000000-0000-0000-0000-000000000003");
        public static readonly Guid Task4 =
            Guid.Parse("50000000-0000-0000-0000-000000000004");
        public static readonly Guid Task5 =
            Guid.Parse("50000000-0000-0000-0000-000000000005");
        public static readonly Guid Task6 =
            Guid.Parse("50000000-0000-0000-0000-000000000006");
        public static readonly Guid Task7 =
            Guid.Parse("50000000-0000-0000-0000-000000000007");

        // Warehouses
        public static readonly Guid Warehouse1 =
            Guid.Parse("60000000-0000-0000-0000-000000000001");
        public static readonly Guid Warehouse2 =
            Guid.Parse("60000000-0000-0000-0000-000000000002");
        public static readonly Guid Warehouse3 =
            Guid.Parse("60000000-0000-0000-0000-000000000003");

        // Supplies
        public static readonly Guid Rice =
            Guid.Parse("70000000-0000-0000-0000-000000000001");
        public static readonly Guid DrinkingWater =
            Guid.Parse("70000000-0000-0000-0000-000000000002");
        public static readonly Guid Blanket =
            Guid.Parse("70000000-0000-0000-0000-000000000003");
        public static readonly Guid Medicine =
            Guid.Parse("70000000-0000-0000-0000-000000000004");
        public static readonly Guid FirstAidKit =
            Guid.Parse("70000000-0000-0000-0000-000000000005");
        public static readonly Guid CannedFood =
            Guid.Parse("70000000-0000-0000-0000-000000000006");
        public static readonly Guid Flashlight =
            Guid.Parse("70000000-0000-0000-0000-000000000007");
        public static readonly Guid LifeJacket =
            Guid.Parse("70000000-0000-0000-0000-000000000008");
        public static readonly Guid HygieneKit =
            Guid.Parse("70000000-0000-0000-0000-000000000009");
        public static readonly Guid BabyFood =
            Guid.Parse("70000000-0000-0000-0000-000000000010");

        // Donations
        public static readonly Guid Donation1 =
            Guid.Parse("80000000-0000-0000-0000-000000000001");
        public static readonly Guid Donation2 =
            Guid.Parse("80000000-0000-0000-0000-000000000002");
        public static readonly Guid Donation3 =
            Guid.Parse("80000000-0000-0000-0000-000000000003");

        // Notifications
        public static readonly Guid Notification1 =
            Guid.Parse("90000000-0000-0000-0000-000000000001");
        public static readonly Guid Notification2 =
            Guid.Parse("90000000-0000-0000-0000-000000000002");
        public static readonly Guid Notification3 =
            Guid.Parse("90000000-0000-0000-0000-000000000003");
        public static readonly Guid Notification4 =
            Guid.Parse("90000000-0000-0000-0000-000000000004");
        public static readonly Guid Notification5 =
            Guid.Parse("90000000-0000-0000-0000-000000000005");
        public static readonly Guid Notification6 =
            Guid.Parse("90000000-0000-0000-0000-000000000006");
        public static readonly Guid Notification7 =
            Guid.Parse("90000000-0000-0000-0000-000000000007");
        public static readonly Guid Notification8 =
            Guid.Parse("90000000-0000-0000-0000-000000000008");

        // VolunteerSkill
        public static readonly Guid VolunteerSkill1 =
            Guid.Parse("31000000-0000-0000-0000-000000000001");
        public static readonly Guid VolunteerSkill2 =
            Guid.Parse("31000000-0000-0000-0000-000000000002");
        public static readonly Guid VolunteerSkill3 =
            Guid.Parse("31000000-0000-0000-0000-000000000003");
        public static readonly Guid VolunteerSkill4 =
            Guid.Parse("31000000-0000-0000-0000-000000000004");
        public static readonly Guid VolunteerSkill5 =
            Guid.Parse("31000000-0000-0000-0000-000000000005");
        public static readonly Guid VolunteerSkill6 =
            Guid.Parse("31000000-0000-0000-0000-000000000006");
        public static readonly Guid VolunteerSkill7 =
            Guid.Parse("31000000-0000-0000-0000-000000000007");
        public static readonly Guid VolunteerSkill8 =
            Guid.Parse("31000000-0000-0000-0000-000000000008");
        public static readonly Guid VolunteerSkill9 =
            Guid.Parse("31000000-0000-0000-0000-000000000009");
        public static readonly Guid VolunteerSkill10 =
            Guid.Parse("31000000-0000-0000-0000-000000000010");
        public static readonly Guid VolunteerSkill11 =
            Guid.Parse("31000000-0000-0000-0000-000000000011");
        public static readonly Guid VolunteerSkill12 =
            Guid.Parse("31000000-0000-0000-0000-000000000012");

        // WarehouseInventory
        public static readonly Guid WarehouseInventory1 =
            Guid.Parse("61000000-0000-0000-0000-000000000001");
        public static readonly Guid WarehouseInventory2 =
            Guid.Parse("61000000-0000-0000-0000-000000000002");
        public static readonly Guid WarehouseInventory3 =
            Guid.Parse("61000000-0000-0000-0000-000000000003");
        public static readonly Guid WarehouseInventory4 =
            Guid.Parse("61000000-0000-0000-0000-000000000004");
        public static readonly Guid WarehouseInventory5 =
            Guid.Parse("61000000-0000-0000-0000-000000000005");
        public static readonly Guid WarehouseInventory6 =
            Guid.Parse("61000000-0000-0000-0000-000000000006");
        public static readonly Guid WarehouseInventory7 =
            Guid.Parse("61000000-0000-0000-0000-000000000007");
        public static readonly Guid WarehouseInventory8 =
            Guid.Parse("61000000-0000-0000-0000-000000000008");
        public static readonly Guid WarehouseInventory9 =
            Guid.Parse("61000000-0000-0000-0000-000000000009");

        // Warehouse transactions
        public static readonly Guid WarehouseTransaction1 =
            Guid.Parse("62000000-0000-0000-0000-000000000001");
        public static readonly Guid WarehouseTransaction2 =
            Guid.Parse("62000000-0000-0000-0000-000000000002");
        public static readonly Guid WarehouseTransaction3 =
            Guid.Parse("62000000-0000-0000-0000-000000000003");
        public static readonly Guid WarehouseTransaction4 =
            Guid.Parse("62000000-0000-0000-0000-000000000004");
        public static readonly Guid WarehouseTransaction5 =
            Guid.Parse("62000000-0000-0000-0000-000000000005");
        public static readonly Guid WarehouseTransaction6 =
            Guid.Parse("62000000-0000-0000-0000-000000000006");

        // Volunteer Engagements
        public static readonly Guid Engagement1 =
            Guid.Parse("51000000-0000-0000-0000-000000000001");
        public static readonly Guid Engagement2 =
            Guid.Parse("51000000-0000-0000-0000-000000000002");
        public static readonly Guid Engagement3 =
            Guid.Parse("51000000-0000-0000-0000-000000000003");
        public static readonly Guid Engagement4 =
            Guid.Parse("51000000-0000-0000-0000-000000000004");
        public static readonly Guid Engagement5 =
            Guid.Parse("51000000-0000-0000-0000-000000000005");

        // Task Assignments
        public static readonly Guid Assignment1 =
            Guid.Parse("52000000-0000-0000-0000-000000000001");
        public static readonly Guid Assignment2 =
            Guid.Parse("52000000-0000-0000-0000-000000000002");
        public static readonly Guid Assignment3 =
            Guid.Parse("52000000-0000-0000-0000-000000000003");
        public static readonly Guid Assignment4 =
            Guid.Parse("52000000-0000-0000-0000-000000000004");
        public static readonly Guid Assignment5 =
            Guid.Parse("52000000-0000-0000-0000-000000000005");
        public static readonly Guid Assignment6 =
            Guid.Parse("52000000-0000-0000-0000-000000000006");
        public static readonly Guid Assignment7 =
            Guid.Parse("52000000-0000-0000-0000-000000000007");

        // Donation Transactions
        public static readonly Guid DonationTransaction1 =
            Guid.Parse("81000000-0000-0000-0000-000000000001");
        public static readonly Guid DonationTransaction2 =
            Guid.Parse("81000000-0000-0000-0000-000000000002");
        public static readonly Guid DonationTransaction3 =
            Guid.Parse("81000000-0000-0000-0000-000000000003");

        // Inventory Transactions
        public static readonly Guid InventoryTransaction1 =
            Guid.Parse("72000000-0000-0000-0000-000000000001");
        public static readonly Guid InventoryTransaction2 =
            Guid.Parse("72000000-0000-0000-0000-000000000002");
        public static readonly Guid InventoryTransaction3 =
            Guid.Parse("72000000-0000-0000-0000-000000000003");

        // Audit Logs
        public static readonly Guid AuditLog1 =
            Guid.Parse("A0000000-0000-0000-0000-000000000001");
        public static readonly Guid AuditLog2 =
            Guid.Parse("A0000000-0000-0000-0000-000000000002");
        public static readonly Guid AuditLog3 =
            Guid.Parse("A0000000-0000-0000-0000-000000000003");
        public static readonly Guid AuditLog4 =
            Guid.Parse("A0000000-0000-0000-0000-000000000004");
        public static readonly Guid AuditLog5 =
            Guid.Parse("A0000000-0000-0000-0000-000000000005");
        public static readonly Guid AuditLog6 =
            Guid.Parse("A0000000-0000-0000-0000-000000000006");
        public static readonly Guid AuditLog7 =
            Guid.Parse("A0000000-0000-0000-0000-000000000007");
        public static readonly Guid AuditLog8 =
            Guid.Parse("A0000000-0000-0000-0000-000000000008");
    }
}
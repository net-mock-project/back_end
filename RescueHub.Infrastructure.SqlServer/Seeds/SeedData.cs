using RescueHub.Domain.Common.Enums;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Seeds
{
    public static class SeedData
    {
        private static readonly DateTime SeedDate =
            new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const string SeedPasswordHash = "SEED_PASSWORD_HASH";

        public static List<RoleDataModel> Roles()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.RequesterRole,
                    Name = "Requester",
                    Description = "User who creates relief requests.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.VolunteerRole,
                    Name = "Volunteer",
                    Description = "User who participates in relief activities.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.CoordinatorRole,
                    Name = "Coordinator",
                    Description = "User who coordinates relief operations.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.AdminRole,
                    Name = "Admin",
                    Description = "System administrator.",
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<SkillDataModel> Skills()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.FirstAidSkill,
                    Name = "First Aid",
                    Description = "Basic first aid and emergency response.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.MedicalSkill,
                    Name = "Medical",
                    Description = "Medical support and healthcare.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.DrivingSkill,
                    Name = "Driving",
                    Description = "Vehicle and emergency transport operation.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.SwimmingSkill,
                    Name = "Swimming",
                    Description = "Swimming and water rescue capability.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.RescueSkill,
                    Name = "Rescue",
                    Description = "Search and rescue operations.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.LogisticsSkill,
                    Name = "Logistics",
                    Description = "Relief logistics and supply management.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.CommunicationSkill,
                    Name = "Communication",
                    Description = "Communication and coordination.",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.CookingSkill,
                    Name = "Cooking",
                    Description = "Food preparation for affected communities.",
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<SupplyDataModel> Supplies()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Rice,
                    Name = "Rice",
                    Category = "Food",
                    Unit = "kg",
                    MinimumStock = 500,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.DrinkingWater,
                    Name = "Drinking Water",
                    Category = "Water",
                    Unit = "bottle",
                    MinimumStock = 1000,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Blanket,
                    Name = "Blanket",
                    Category = "Shelter",
                    Unit = "piece",
                    MinimumStock = 100,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Medicine,
                    Name = "Medicine",
                    Category = "Medical",
                    Unit = "box",
                    MinimumStock = 100,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.FirstAidKit,
                    Name = "First Aid Kit",
                    Category = "Medical",
                    Unit = "kit",
                    MinimumStock = 50,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.CannedFood,
                    Name = "Canned Food",
                    Category = "Food",
                    Unit = "can",
                    MinimumStock = 500,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Flashlight,
                    Name = "Flashlight",
                    Category = "Equipment",
                    Unit = "piece",
                    MinimumStock = 100,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.LifeJacket,
                    Name = "Life Jacket",
                    Category = "Rescue Equipment",
                    Unit = "piece",
                    MinimumStock = 50,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.HygieneKit,
                    Name = "Hygiene Kit",
                    Category = "Hygiene",
                    Unit = "kit",
                    MinimumStock = 100,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.BabyFood,
                    Name = "Baby Food",
                    Category = "Food",
                    Unit = "box",
                    MinimumStock = 100,
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<UserDataModel> Users()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.RequesterUser,
                    RoleId = SeedConstants.RequesterRole,
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1995, 5, 12),
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Province = "Đà Nẵng",
                    FullName = "Nguyễn Văn An",
                    Email = "requester@rescuehub.test",
                    Phone = "0900000001",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.VolunteerUser1,
                    RoleId = SeedConstants.VolunteerRole,
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1998, 3, 20),
                    Location = SeedLocation.Create(108.1760, 16.0730),
                    Province = "Đà Nẵng",
                    FullName = "Trần Minh Quân",
                    Email = "volunteer1@rescuehub.test",
                    Phone = "0900000002",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.VolunteerUser2,
                    RoleId = SeedConstants.VolunteerRole,
                    Gender = Gender.Female,
                    DateOfBirth = new DateOnly(1997, 8, 15),
                    Location = SeedLocation.Create(108.2120, 16.0470),
                    Province = "Đà Nẵng",
                    FullName = "Lê Thu Hà",
                    Email = "volunteer2@rescuehub.test",
                    Phone = "0900000003",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.VolunteerUser3,
                    RoleId = SeedConstants.VolunteerRole,
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1992, 11, 2),
                    Location = SeedLocation.Create(108.1540, 16.0390),
                    Province = "Đà Nẵng",
                    FullName = "Phạm Đức Long",
                    Email = "volunteer3@rescuehub.test",
                    Phone = "0900000004",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.VolunteerUser4,
                    RoleId = SeedConstants.VolunteerRole,
                    Gender = Gender.Female,
                    DateOfBirth = new DateOnly(2000, 1, 25),
                    Location = SeedLocation.Create(108.1900, 16.0610),
                    Province = "Đà Nẵng",
                    FullName = "Ngô Mai Anh",
                    Email = "volunteer4@rescuehub.test",
                    Phone = "0900000005",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.CoordinatorUser,
                    RoleId = SeedConstants.CoordinatorRole,
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1988, 6, 10),
                    Location = SeedLocation.Create(108.2200, 16.0670),
                    Province = "Đà Nẵng",
                    FullName = "Nguyễn Văn Bình",
                    Email = "coordinator@rescuehub.test",
                    Phone = "0900000006",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.AdminUser,
                    RoleId = SeedConstants.AdminRole,
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1985, 2, 18),
                    Location = null,
                    Province = "Đà Nẵng",
                    FullName = "Trần Quốc Minh",
                    Email = "admin@rescuehub.test",
                    Phone = "0900000007",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Active,
                    IsVerified = true,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.InactiveUser,
                    RoleId = SeedConstants.RequesterRole,
                    Gender = Gender.Female,
                    DateOfBirth = new DateOnly(1996, 9, 30),
                    Location = SeedLocation.Create(108.2100, 16.0500),
                    Province = "Đà Nẵng",
                    FullName = "Đỗ Ngọc Lan",
                    Email = "inactive@rescuehub.test",
                    Phone = "0900000008",
                    PasswordHash = SeedPasswordHash,
                    Status = UserStatus.Inactive,
                    IsVerified = true,
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<VolunteerDataModel> Volunteers()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Volunteer1,
                    ExperienceYears = 5,
                    ApprovalStatus = VolunteerApprovalStatus.Approved,
                    CVUrl = "https://example.com/cv/volunteer-1",
                    ApprovedBy = SeedConstants.CoordinatorUser,
                    ApprovedAt = SeedDate.AddDays(2),
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Volunteer2,
                    ExperienceYears = 3,
                    ApprovalStatus = VolunteerApprovalStatus.Approved,
                    CVUrl = "https://example.com/cv/volunteer-2",
                    ApprovedBy = SeedConstants.CoordinatorUser,
                    ApprovedAt = SeedDate.AddDays(3),
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Volunteer3,
                    ExperienceYears = 7,
                    ApprovalStatus = VolunteerApprovalStatus.Approved,
                    CVUrl = "https://example.com/cv/volunteer-3",
                    ApprovedBy = SeedConstants.CoordinatorUser,
                    ApprovedAt = SeedDate.AddDays(1),
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Volunteer4,
                    ExperienceYears = 1,
                    ApprovalStatus = VolunteerApprovalStatus.Pending,
                    CVUrl = "https://example.com/cv/volunteer-4",
                    ApprovedBy = null,
                    ApprovedAt = null,
                    CreatedAt = SeedDate.AddDays(5)
                }
            ];
        }

        public static List<VolunteerSkillDataModel> VolunteerSkills()
        {
            return
            [
                new()
                {
                    VolunteerId = SeedConstants.Volunteer1,
                    SkillId = SeedConstants.FirstAidSkill,
                    Level = 5
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer1,
                    SkillId = SeedConstants.RescueSkill,
                    Level = 4
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer1,
                    SkillId = SeedConstants.SwimmingSkill,
                    Level = 3
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer2,
                    SkillId = SeedConstants.MedicalSkill,
                    Level = 5
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer2,
                    SkillId = SeedConstants.FirstAidSkill,
                    Level = 4
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer2,
                    SkillId = SeedConstants.CommunicationSkill,
                    Level = 4
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer3,
                    SkillId = SeedConstants.DrivingSkill,
                    Level = 5
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer3,
                    SkillId = SeedConstants.LogisticsSkill,
                    Level = 5
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer3,
                    SkillId = SeedConstants.RescueSkill,
                    Level = 4
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer4,
                    SkillId = SeedConstants.CookingSkill,
                    Level = 3
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer4,
                    SkillId = SeedConstants.CommunicationSkill,
                    Level = 3
                },

                new()
                {
                    VolunteerId = SeedConstants.Volunteer4,
                    SkillId = SeedConstants.FirstAidSkill,
                    Level = 2
                }
            ];
        }

        public static List<WarehouseDataModel> Warehouses()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Warehouse1,
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Name = "Da Nang Central Relief Warehouse",
                    ManagerName = "Nguyễn Văn Bình",
                    Province = "Đà Nẵng",
                    Phone = "0910000001",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Warehouse2,
                    Location = SeedLocation.Create(108.1190, 16.0700),
                    Name = "Lien Chieu Relief Warehouse",
                    ManagerName = "Trần Văn Nam",
                    Province = "Đà Nẵng",
                    Phone = "0910000002",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.Warehouse3,
                    Location = SeedLocation.Create(108.4630, 16.3300),
                    Name = "Hoi An Relief Warehouse",
                    ManagerName = "Lê Minh Tuấn",
                    Province = "Quảng Nam",
                    Phone = "0910000003",
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<WarehouseInventoryDataModel> WarehouseInventories()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.WarehouseInventory1,
                    WarehouseId = SeedConstants.Warehouse1,
                    SupplyId = SeedConstants.Rice,
                    Quantity = 2500,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory2,
                    WarehouseId = SeedConstants.Warehouse1,
                    SupplyId = SeedConstants.DrinkingWater,
                    Quantity = 5000,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory3,
                    WarehouseId = SeedConstants.Warehouse1,
                    SupplyId = SeedConstants.FirstAidKit,
                    Quantity = 250,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory4,
                    WarehouseId = SeedConstants.Warehouse2,
                    SupplyId = SeedConstants.Blanket,
                    Quantity = 700,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory5,
                    WarehouseId = SeedConstants.Warehouse2,
                    SupplyId = SeedConstants.CannedFood,
                    Quantity = 3000,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory6,
                    WarehouseId = SeedConstants.Warehouse2,
                    SupplyId = SeedConstants.LifeJacket,
                    Quantity = 120,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory7,
                    WarehouseId = SeedConstants.Warehouse3,
                    SupplyId = SeedConstants.Medicine,
                    Quantity = 350,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory8,
                    WarehouseId = SeedConstants.Warehouse3,
                    SupplyId = SeedConstants.HygieneKit,
                    Quantity = 450,
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.WarehouseInventory9,
                    WarehouseId = SeedConstants.Warehouse3,
                    SupplyId = SeedConstants.BabyFood,
                    Quantity = 180,
                    CreatedAt = SeedDate
                }
            ];
        }

        public static List<WarehouseTransactionDataModel> WarehouseTransactions()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.WarehouseTransaction1,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory1,
                    Quantity = 3000,
                    TransactionType = WarehouseTransactionType.Import,
                    Status = WarehouseTransactionStatus.Completed,
                    CreatedBy = SeedConstants.AdminUser,
                    CreatedAt = SeedDate.AddDays(1)
                },

                new()
                {
                    Id = SeedConstants.WarehouseTransaction2,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory1,
                    Quantity = 500,
                    TransactionType = WarehouseTransactionType.Export,
                    Status = WarehouseTransactionStatus.Completed,
                    CreatedBy = SeedConstants.CoordinatorUser,
                    CreatedAt = SeedDate.AddDays(10)
                },

                new()
                {
                    Id = SeedConstants.WarehouseTransaction3,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory2,
                    Quantity = 5000,
                    TransactionType = WarehouseTransactionType.Import,
                    Status = WarehouseTransactionStatus.Completed,
                    CreatedBy = SeedConstants.AdminUser,
                    CreatedAt = SeedDate.AddDays(2)
                },

                new()
                {
                    Id = SeedConstants.WarehouseTransaction4,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory4,
                    Quantity = 1000,
                    TransactionType = WarehouseTransactionType.Import,
                    Status = WarehouseTransactionStatus.Completed,
                    CreatedBy = SeedConstants.AdminUser,
                    CreatedAt = SeedDate.AddDays(3)
                },

                new()
                {
                    Id = SeedConstants.WarehouseTransaction5,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory5,
                    Quantity = 500,
                    TransactionType = WarehouseTransactionType.Export,
                    Status = WarehouseTransactionStatus.Completed,
                    CreatedBy = SeedConstants.CoordinatorUser,
                    CreatedAt = SeedDate.AddDays(12)
                },

                new()
                {
                    Id = SeedConstants.WarehouseTransaction6,
                    WarehouseInventoryId = SeedConstants.WarehouseInventory7,
                    Quantity = 100,
                    TransactionType = WarehouseTransactionType.Adjustment,
                    Status = WarehouseTransactionStatus.Pending,
                    CreatedBy = SeedConstants.CoordinatorUser,
                    CreatedAt = SeedDate.AddDays(15)
                }
            ];
        }

        public static List<ReliefRequestDataModel> ReliefRequests()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Request1,
                    RequesterId = SeedConstants.RequesterUser,
                    CoordinatorId = SeedConstants.CoordinatorUser,
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Title = "Flooded residential area in Hai Chau",
                    Description =
                        "Several residential streets are flooded and residents need drinking water and food.",
                    ReliefImageUrl = "https://example.com/images/request-1.jpg",
                    RequestedResource = "Drinking water, rice, canned food",
                    StartTime = SeedDate.AddDays(10),
                    EndTime = SeedDate.AddDays(12),
                    UrgencyLevel = 5,
                    EstimatedAffectedPeople = 350,
                    EstimatedAffectedRadiusKm = 2.5m,
                    Status = ReliefRequestStatus.InProgress,
                    CreatedAt = SeedDate.AddDays(5)
                },

                new()
                {
                    Id = SeedConstants.Request2,
                    RequesterId = SeedConstants.RequesterUser,
                    CoordinatorId = SeedConstants.CoordinatorUser,
                    Location = SeedLocation.Create(108.1190, 16.0700),
                    Title = "Medical support needed in Lien Chieu",
                    Description =
                        "Residents require first aid and basic medical supplies after flooding.",
                    ReliefImageUrl = "https://example.com/images/request-2.jpg",
                    RequestedResource = "Medicine, first aid kits",
                    StartTime = SeedDate.AddDays(15),
                    EndTime = SeedDate.AddDays(17),
                    UrgencyLevel = 4,
                    EstimatedAffectedPeople = 180,
                    EstimatedAffectedRadiusKm = 1.8m,
                    Status = ReliefRequestStatus.Approved,
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.Request3,
                    RequesterId = SeedConstants.RequesterUser,
                    CoordinatorId = SeedConstants.CoordinatorUser,
                    Location = SeedLocation.Create(108.1540, 16.0390),
                    Title = "Rescue operation near river",
                    Description =
                        "Several households are isolated due to rising water levels.",
                    ReliefImageUrl = "https://example.com/images/request-3.jpg",
                    RequestedResource = "Rescue team, life jackets, drinking water",
                    StartTime = SeedDate.AddDays(20),
                    EndTime = SeedDate.AddDays(22),
                    UrgencyLevel = 5,
                    EstimatedAffectedPeople = 90,
                    EstimatedAffectedRadiusKm = 3.2m,
                    Status = ReliefRequestStatus.Pending,
                    CreatedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Request4,
                    RequesterId = SeedConstants.RequesterUser,
                    CoordinatorId = SeedConstants.CoordinatorUser,
                    Location = SeedLocation.Create(108.4630, 16.3300),
                    Title = "Food distribution in Hoi An",
                    Description =
                        "Food supplies are required for households affected by flooding.",
                    ReliefImageUrl = "https://example.com/images/request-4.jpg",
                    RequestedResource = "Rice, canned food, baby food",
                    StartTime = SeedDate.AddDays(2),
                    EndTime = SeedDate.AddDays(4),
                    UrgencyLevel = 3,
                    EstimatedAffectedPeople = 500,
                    EstimatedAffectedRadiusKm = 4.5m,
                    Status = ReliefRequestStatus.Completed,
                    CreatedAt = SeedDate.AddDays(-10),
                    CompletedAt = SeedDate.AddDays(4)
                },

                new()
                {
                    Id = SeedConstants.Request5,
                    RequesterId = SeedConstants.RequesterUser,
                    CoordinatorId = SeedConstants.CoordinatorUser,
                    Location = SeedLocation.Create(108.2100, 16.0500),
                    Title = "Small emergency supply request",
                    Description =
                        "A small group of residents requested emergency hygiene supplies.",
                    ReliefImageUrl = null,
                    RequestedResource = "Hygiene kits",
                    StartTime = SeedDate.AddDays(25),
                    EndTime = SeedDate.AddDays(26),
                    UrgencyLevel = 2,
                    EstimatedAffectedPeople = 40,
                    EstimatedAffectedRadiusKm = 0.8m,
                    Status = ReliefRequestStatus.Rejected,
                    CreatedAt = SeedDate.AddDays(9)
                }
            ];
        }

        public static List<ReliefTaskDataModel> ReliefTasks()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Task1,
                    RequestId = SeedConstants.Request1,
                    Title = "Deliver drinking water",
                    Description = "Transport drinking water to affected households.",
                    RequiredVolunteers = 3,
                    Priority = 5,
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Status = ReliefTaskStatus.InProgress,
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.Task2,
                    RequestId = SeedConstants.Request1,
                    Title = "Distribute food supplies",
                    Description = "Distribute rice and canned food to affected residents.",
                    RequiredVolunteers = 4,
                    Priority = 4,
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Status = ReliefTaskStatus.Pending,
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.Task3,
                    RequestId = SeedConstants.Request2,
                    Title = "Medical first aid",
                    Description = "Provide first aid and basic medical assistance.",
                    RequiredVolunteers = 2,
                    Priority = 5,
                    Location = SeedLocation.Create(108.1190, 16.0700),
                    Status = ReliefTaskStatus.Pending,
                    CreatedAt = SeedDate.AddDays(7)
                },

                new()
                {
                    Id = SeedConstants.Task4,
                    RequestId = SeedConstants.Request3,
                    Title = "Water rescue",
                    Description = "Rescue isolated households from flooded areas.",
                    RequiredVolunteers = 5,
                    Priority = 5,
                    Location = SeedLocation.Create(108.1540, 16.0390),
                    Status = ReliefTaskStatus.Pending,
                    CreatedAt = SeedDate.AddDays(9)
                },

                new()
                {
                    Id = SeedConstants.Task5,
                    RequestId = SeedConstants.Request4,
                    Title = "Food distribution",
                    Description = "Distribute food packages to affected households.",
                    RequiredVolunteers = 5,
                    Priority = 3,
                    Location = SeedLocation.Create(108.4630, 16.3300),
                    Status = ReliefTaskStatus.Completed,
                    CreatedAt = SeedDate.AddDays(-9)
                },

                new()
                {
                    Id = SeedConstants.Task6,
                    RequestId = SeedConstants.Request4,
                    Title = "Baby food distribution",
                    Description = "Deliver baby food to families with young children.",
                    RequiredVolunteers = 2,
                    Priority = 4,
                    Location = SeedLocation.Create(108.4630, 16.3300),
                    Status = ReliefTaskStatus.Completed,
                    CreatedAt = SeedDate.AddDays(-8)
                },

                new()
                {
                    Id = SeedConstants.Task7,
                    RequestId = SeedConstants.Request1,
                    Title = "Emergency logistics support",
                    Description = "Coordinate transportation and supply movement.",
                    RequiredVolunteers = 2,
                    Priority = 4,
                    Location = SeedLocation.Create(108.2022, 16.0544),
                    Status = ReliefTaskStatus.InProgress,
                    CreatedAt = SeedDate.AddDays(7)
                }
            ];
        }

        public static List<TaskSkillDataModel> TaskSkills()
        {
            return
            [
                new()
                {
                    TaskId = SeedConstants.Task1,
                    SkillId = SeedConstants.DrivingSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task1,
                    SkillId = SeedConstants.LogisticsSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task2,
                    SkillId = SeedConstants.LogisticsSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task3,
                    SkillId = SeedConstants.FirstAidSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task3,
                    SkillId = SeedConstants.MedicalSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task4,
                    SkillId = SeedConstants.SwimmingSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task4,
                    SkillId = SeedConstants.RescueSkill
                },

                new()
                {
                    TaskId = SeedConstants.Task5,
                    SkillId = SeedConstants.CommunicationSkill
                }
            ];
        }

        public static List<VolunteerEngagementDataModel> VolunteerEngagements()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Engagement1,
                    VolunteerId = SeedConstants.Volunteer1,
                    RequestId = SeedConstants.Request1,
                    PerformanceAssessment = "Good performance during water distribution.",
                    Status = VolunteerEngagementStatus.Active,
                    CreatedAt = SeedDate.AddDays(7)
                },

                new()
                {
                    Id = SeedConstants.Engagement2,
                    VolunteerId = SeedConstants.Volunteer2,
                    RequestId = SeedConstants.Request2,
                    PerformanceAssessment = null,
                    Status = VolunteerEngagementStatus.Pending,
                    CreatedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Engagement3,
                    VolunteerId = SeedConstants.Volunteer3,
                    RequestId = SeedConstants.Request3,
                    PerformanceAssessment = null,
                    Status = VolunteerEngagementStatus.Pending,
                    CreatedAt = SeedDate.AddDays(9)
                },

                new()
                {
                    Id = SeedConstants.Engagement4,
                    VolunteerId = SeedConstants.Volunteer3,
                    RequestId = SeedConstants.Request4,
                    PerformanceAssessment = "Successfully completed food delivery.",
                    Status = VolunteerEngagementStatus.Completed,
                    CreatedAt = SeedDate.AddDays(-8)
                },

                new()
                {
                    Id = SeedConstants.Engagement5,
                    VolunteerId = SeedConstants.Volunteer4,
                    RequestId = SeedConstants.Request4,
                    PerformanceAssessment = "Completed assigned support activities.",
                    Status = VolunteerEngagementStatus.Completed,
                    CreatedAt = SeedDate.AddDays(-7)
                }
            ];
        }

        public static List<TaskAssignmentDataModel> TaskAssignments()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Assignment1,
                    TaskId = SeedConstants.Task1,
                    VolunteerId = SeedConstants.Volunteer1,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.Coordinator,
                    Status = TaskAssignmentStatus.Accepted,
                    AssignedAt = SeedDate.AddDays(7),
                    AcceptedAt = SeedDate.AddDays(7).AddHours(2),
                    ResponseAt = SeedDate.AddDays(7).AddHours(1)
                },

                new()
                {
                    Id = SeedConstants.Assignment2,
                    TaskId = SeedConstants.Task1,
                    VolunteerId = SeedConstants.Volunteer3,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.Coordinator,
                    Status = TaskAssignmentStatus.InProgress,
                    AssignedAt = SeedDate.AddDays(7),
                    AcceptedAt = SeedDate.AddDays(7).AddHours(1),
                    ResponseAt = SeedDate.AddDays(7).AddMinutes(30)
                },

                new()
                {
                    Id = SeedConstants.Assignment3,
                    TaskId = SeedConstants.Task2,
                    VolunteerId = SeedConstants.Volunteer3,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.System,
                    Status = TaskAssignmentStatus.Pending,
                    AssignedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Assignment4,
                    TaskId = SeedConstants.Task3,
                    VolunteerId = SeedConstants.Volunteer2,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.Coordinator,
                    Status = TaskAssignmentStatus.Pending,
                    AssignedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Assignment5,
                    TaskId = SeedConstants.Task4,
                    VolunteerId = SeedConstants.Volunteer1,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.VolunteerRequest,
                    Status = TaskAssignmentStatus.Accepted,
                    AssignedAt = SeedDate.AddDays(10),
                    AcceptedAt = SeedDate.AddDays(10).AddHours(1),
                    ResponseAt = SeedDate.AddDays(10).AddMinutes(30)
                },

                new()
                {
                    Id = SeedConstants.Assignment6,
                    TaskId = SeedConstants.Task5,
                    VolunteerId = SeedConstants.Volunteer3,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.Coordinator,
                    Status = TaskAssignmentStatus.Completed,
                    AssignedAt = SeedDate.AddDays(-7),
                    AcceptedAt = SeedDate.AddDays(-7).AddHours(1),
                    ResponseAt = SeedDate.AddDays(-7).AddMinutes(30),
                    CompletedAt = SeedDate.AddDays(-5)
                },

                new()
                {
                    Id = SeedConstants.Assignment7,
                    TaskId = SeedConstants.Task6,
                    VolunteerId = SeedConstants.Volunteer4,
                    AssignedBy = SeedConstants.CoordinatorUser,
                    AssignmentSource = TaskAssignmentSource.Coordinator,
                    Status = TaskAssignmentStatus.Completed,
                    AssignedAt = SeedDate.AddDays(-6),
                    AcceptedAt = SeedDate.AddDays(-6).AddHours(1),
                    ResponseAt = SeedDate.AddDays(-6).AddMinutes(20),
                    CompletedAt = SeedDate.AddDays(-4)
                }
            ];
        }

        public static List<DonationDataModel> Donations()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Donation1,
                    DonatorId = SeedConstants.RequesterUser,
                    Status = DonationStatus.Approved,
                    DonationDate = SeedDate.AddDays(5),
                    ApprovedBy = SeedConstants.CoordinatorUser,
                    ApprovedAt = SeedDate.AddDays(5).AddHours(3),
                    Remark = "Donation of food and drinking water.",
                    CreatedAt = SeedDate.AddDays(5)
                },

                new()
                {
                    Id = SeedConstants.Donation2,
                    DonatorId = SeedConstants.Volunteer1,
                    Status = DonationStatus.Pending,
                    DonationDate = SeedDate.AddDays(12),
                    ApprovedBy = null,
                    ApprovedAt = null,
                    Remark = "Volunteer donated emergency supplies.",
                    CreatedAt = SeedDate.AddDays(12)
                },

                new()
                {
                    Id = SeedConstants.Donation3,
                    DonatorId = SeedConstants.Volunteer2,
                    Status = DonationStatus.Rejected,
                    DonationDate = SeedDate.AddDays(14),
                    ApprovedBy = SeedConstants.CoordinatorUser,
                    ApprovedAt = SeedDate.AddDays(14).AddHours(2),
                    Remark = "Donation information could not be verified.",
                    CreatedAt = SeedDate.AddDays(14)
                }
            ];
        }

        public static List<DonationTransactionDataModel> DonationTransactions()
        {
            return
            [
                new()
                {
                    TransactionId = SeedConstants.WarehouseTransaction1,
                    DonationId = SeedConstants.Donation1
                },

                new()
                {
                    TransactionId = SeedConstants.WarehouseTransaction3,
                    DonationId = SeedConstants.Donation2
                },

                new()
                {
                    TransactionId = SeedConstants.WarehouseTransaction4,
                    DonationId = SeedConstants.Donation3
                }
            ];
        }

        public static List<InventoryTransactionDataModel> InventoryTransactions()
        {
            return
            [
                new()
        {
            TransactionId = SeedConstants.WarehouseTransaction2,
            TaskId = SeedConstants.Task1
        },

        new()
        {
            TransactionId = SeedConstants.WarehouseTransaction5,
            TaskId = SeedConstants.Task5
        },

        new()
        {
            TransactionId = SeedConstants.WarehouseTransaction6,
            TaskId = SeedConstants.Task7
        }
            ];
        }

        public static List<NotificationDataModel> Notifications()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.Notification1,
                    UserId = SeedConstants.RequesterUser,
                    Title = "Relief request approved",
                    Content = "Your relief request has been approved.",
                    Type = NotificationType.ReliefRequest,
                    UrlLink = "/relief-requests/" + SeedConstants.Request1,
                    IsRead = true,
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.Notification2,
                    UserId = SeedConstants.RequesterUser,
                    Title = "Relief task started",
                    Content = "A task related to your relief request has started.",
                    Type = NotificationType.ReliefTask,
                    UrlLink = "/relief-requests/" + SeedConstants.Request1,
                    IsRead = false,
                    CreatedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Notification3,
                    UserId = SeedConstants.VolunteerUser1,
                    Title = "New task assignment",
                    Content = "You have been assigned to a relief task.",
                    Type = NotificationType.TaskAssignment,
                    UrlLink = "/tasks/" + SeedConstants.Task1,
                    IsRead = true,
                    CreatedAt = SeedDate.AddDays(7)
                },

                new()
                {
                    Id = SeedConstants.Notification4,
                    UserId = SeedConstants.VolunteerUser2,
                    Title = "New volunteer assignment",
                    Content = "You have received a new task assignment.",
                    Type = NotificationType.TaskAssignment,
                    UrlLink = "/tasks/" + SeedConstants.Task3,
                    IsRead = false,
                    CreatedAt = SeedDate.AddDays(8)
                },

                new()
                {
                    Id = SeedConstants.Notification5,
                    UserId = SeedConstants.VolunteerUser3,
                    Title = "Task completed",
                    Content = "Your task has been marked as completed.",
                    Type = NotificationType.TaskAssignment,
                    UrlLink = "/tasks/" + SeedConstants.Task5,
                    IsRead = true,
                    CreatedAt = SeedDate.AddDays(-5)
                },

                new()
                {
                    Id = SeedConstants.Notification6,
                    UserId = SeedConstants.CoordinatorUser,
                    Title = "New relief request",
                    Content = "A new relief request requires your attention.",
                    Type = NotificationType.ReliefRequest,
                    UrlLink = "/relief-requests/" + SeedConstants.Request3,
                    IsRead = false,
                    CreatedAt = SeedDate.AddDays(9)
                },

                new()
                {
                    Id = SeedConstants.Notification7,
                    UserId = SeedConstants.CoordinatorUser,
                    Title = "Donation pending approval",
                    Content = "A new donation is waiting for approval.",
                    Type = NotificationType.Donation,
                    UrlLink = "/donations/" + SeedConstants.Donation2,
                    IsRead = false,
                    CreatedAt = SeedDate.AddDays(12)
                },

                new()
                {
                    Id = SeedConstants.Notification8,
                    UserId = SeedConstants.AdminUser,
                    Title = "System activity detected",
                    Content = "Several system activities were recorded.",
                    Type = NotificationType.System,
                    UrlLink = "/audit-logs",
                    IsRead = true,
                    CreatedAt = SeedDate.AddDays(15)
                }
            ];
        }

        public static List<AuditLogDataModel> AuditLogs()
        {
            return
            [
                new()
                {
                    Id = SeedConstants.AuditLog1,
                    UserId = SeedConstants.RequesterUser,
                    Action = "Create",
                    EntityName = "ReliefRequest",
                    EntityId = SeedConstants.Request1,
                    OldValue = null,
                    NewValue = "{\"status\":\"Pending\"}",
                    CreatedAt = SeedDate.AddDays(5)
                },

                new()
                {
                    Id = SeedConstants.AuditLog2,
                    UserId = SeedConstants.CoordinatorUser,
                    Action = "Approve",
                    EntityName = "ReliefRequest",
                    EntityId = SeedConstants.Request1,
                    OldValue = "{\"status\":\"Pending\"}",
                    NewValue = "{\"status\":\"Approved\"}",
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.AuditLog3,
                    UserId = SeedConstants.CoordinatorUser,
                    Action = "Create",
                    EntityName = "ReliefTask",
                    EntityId = SeedConstants.Task1,
                    OldValue = null,
                    NewValue = "{\"status\":\"Pending\"}",
                    CreatedAt = SeedDate.AddDays(6)
                },

                new()
                {
                    Id = SeedConstants.AuditLog4,
                    UserId = SeedConstants.CoordinatorUser,
                    Action = "Assign",
                    EntityName = "TaskAssignment",
                    EntityId = SeedConstants.Assignment1,
                    OldValue = null,
                    NewValue =
                        "{\"volunteerId\":\"" +
                        SeedConstants.Volunteer1 +
                        "\"}",
                    CreatedAt = SeedDate.AddDays(7)
                },

                new()
                {
                    Id = SeedConstants.AuditLog5,
                    UserId = SeedConstants.VolunteerUser1,
                    Action = "Accept",
                    EntityName = "TaskAssignment",
                    EntityId = SeedConstants.Assignment1,
                    OldValue = "{\"status\":\"Pending\"}",
                    NewValue = "{\"status\":\"Accepted\"}",
                    CreatedAt = SeedDate.AddDays(7)
                },

                new()
                {
                    Id = SeedConstants.AuditLog6,
                    UserId = SeedConstants.CoordinatorUser,
                    Action = "Approve",
                    EntityName = "Donation",
                    EntityId = SeedConstants.Donation1,
                    OldValue = "{\"status\":\"Pending\"}",
                    NewValue = "{\"status\":\"Approved\"}",
                    CreatedAt = SeedDate.AddDays(5)
                },

                new()
                {
                    Id = SeedConstants.AuditLog7,
                    UserId = SeedConstants.AdminUser,
                    Action = "Create",
                    EntityName = "Warehouse",
                    EntityId = SeedConstants.Warehouse1,
                    OldValue = null,
                    NewValue = "{\"name\":\"Da Nang Central Relief Warehouse\"}",
                    CreatedAt = SeedDate
                },

                new()
                {
                    Id = SeedConstants.AuditLog8,
                    UserId = SeedConstants.CoordinatorUser,
                    Action = "Update",
                    EntityName = "ReliefTask",
                    EntityId = SeedConstants.Task1,
                    OldValue = "{\"status\":\"Pending\"}",
                    NewValue = "{\"status\":\"InProgress\"}",
                    CreatedAt = SeedDate.AddDays(8)
                }
            ];
        }
    }
}

using AmoebaGameMatcherServer;
using AmoebaGameMatcherServer.Controllers;
using AmoebaGameMatcherServer.Services;
using AmoebaGameMatcherServer.Services.LobbyInitialization;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace IntegrationTests
{
    /// <summary>
    /// Отвечает за настройку БД и создание сервисов.
    /// </summary>
    [SetUpFixture]
    internal sealed class SetUpFixture
    {
        internal static ApplicationDbContext DbContext;
        private const string DatabaseName = "DevelopmentDb102";
        internal static AccountFacadeService AccountFacadeService;
        internal static LobbyModelController LobbyModelController;
        internal static AccountDbReaderService AccountReaderService;
        internal static LobbyModelFacadeService LobbyModelFacadeService;
        internal static StubLobbyModelDbWriteService StubLobbyModelDbWriteService;
        internal static NotShownRewardsReaderService NotShownRewardsReaderService;
        internal static WarshipImprovementCostChecker WarshipImprovementCostChecker;
        internal static WarshipImprovementFacadeService WarshipImprovementFacadeService;

        [OneTimeSetUp]
        public void Initialize()
        {
            //Создать БД
            DbContext = new DbContextFactory().Create(DatabaseName);
            //Ввести базовые данные
            var seeder = new DataSeeder();
            seeder.Seed(DbContext);
            //Прервать текущие сессии
            DbContext.Accounts.FromSql(new RawSqlString("ALTER DATABASE {0} SET postgres WITH ROLLBACK IMMEDIATE"), DatabaseName);
            //Очиста аккаунта
            TruncateAccountsTable();
            string connectionString = DbConnectionConfig.GetConnectionString(DatabaseName);
            //Создать сервисы
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(connectionString);
            SkinsDbReaderService skinsDbReaderService = new SkinsDbReaderService(DbContext);
            DbWarshipsStatisticsReader dbWarshipsStatisticsReader = new DbWarshipsStatisticsReader(npgsqlConnection);
            DbAccountWarshipReaderService dbAccountWarshipReaderService = new DbAccountWarshipReaderService(dbWarshipsStatisticsReader, skinsDbReaderService);
            AccountResourcesDbReader accountResourcesDbReader = new AccountResourcesDbReader(npgsqlConnection);
            AccountReaderService = new AccountDbReaderService(dbAccountWarshipReaderService, accountResourcesDbReader);
            NotShownRewardsReaderService = new NotShownRewardsReaderService(DbContext);
            DefaultAccountFactoryService defaultAccountFactoryService = new DefaultAccountFactoryService(DbContext);
            var accountRegistrationService = new AccountRegistrationService(defaultAccountFactoryService);
            var warshipsCharacteristicsService = new WarshipsCharacteristicsService();
            AccountMapperService accountMapperService = new AccountMapperService(warshipsCharacteristicsService);
            AccountFacadeService = new AccountFacadeService(AccountReaderService, accountRegistrationService);
            LobbyModelFacadeService = new LobbyModelFacadeService(AccountFacadeService, NotShownRewardsReaderService, accountMapperService);
            StubLobbyModelDbWriteService = new StubLobbyModelDbWriteService(DbContext);
            
            LobbyModelController = new LobbyModelController(LobbyModelFacadeService, StubLobbyModelDbWriteService);
            var warshipPowerScaleModelStorage = new WarshipPowerScaleModelStorage();
            WarshipImprovementCostChecker = new WarshipImprovementCostChecker(warshipPowerScaleModelStorage);
            WarshipImprovementFacadeService = new WarshipImprovementFacadeService(AccountReaderService, DbContext, WarshipImprovementCostChecker);
        }

        public static void SetUp()
        {
            ReloadDbContext();
            TruncateAccountsTable();
        }

        private static void ReloadDbContext()
        {
            DbContext = new DbContextFactory().Create(DatabaseName);
        }

        private static void TruncateAccountsTable()
        {
            DbContext.Database.ExecuteSqlCommand(new RawSqlString("TRUNCATE TABLE \"Accounts\" CASCADE;"));
        }
    }
}




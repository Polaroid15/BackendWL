using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

const string ENVIRONMENT_VARIABLE_NAME = "ASPNETCORE_ENVIRONMENT";

var serviceProvider = CreateServices();

using var scope = serviceProvider.CreateScope();
UpdateDatabase(scope.ServiceProvider);

static IServiceProvider CreateServices()
{
    var env = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME);
    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json")
        .AddEnvironmentVariables()
        .Build();

    return new ServiceCollection()
        .AddSingleton<IConfiguration>(config)
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddPostgres11_0()
            .WithGlobalConnectionString(config.GetConnectionString("Main"))
            .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
            .ScanIn(typeof(Program).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

static void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateDown(0);
    // runner.MigrateDown(0); for revert to version 0
    // runner.MigrateDown(migrationNumber); for revert to version migrationNumber
    //[Migration(202206031314, "WISH-999. Create table migr_test for MainApp")]
    //in our single migration number is 202206031314
    // for example: runner.MigrateDown(202206031314);
    
    //in this migration number use some naming convention like: date(20220603) + time (1314)
    // we also can use another naming and start from 1 (1,2,3...n)
}
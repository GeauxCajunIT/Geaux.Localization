using FluentAssertions;
using Geaux.Localization.EFCore.Seeding;
using System.Reflection;

public class KeySeederConstructorTests
{
    [Fact]
    public void Constructor_Accepts_Assemblies()
    {
        Assembly[] assemblies = new[] { typeof(KeySeederConstructorTests).Assembly };

        var seeder = new KeySeeder(assemblies);

        seeder.Should().NotBeNull();
    }
}


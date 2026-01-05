using Geaux.Localization.Tenancy;

public class FakeTenantProvider : ITenantProvider
{
    public string? GetTenantId() => "test-tenant";
}


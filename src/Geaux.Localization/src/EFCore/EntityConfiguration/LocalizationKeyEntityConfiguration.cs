namespace Geaux.Localization.EFCore.EntityConfiguration
{
    internal class LocalizationKeyEntityConfiguration : IEntityTypeConfiguration<LocalizationKey>
    {
        public void Configure(EntityTypeBuilder<LocalizationKey> e)
        {
            e.HasIndex(x => x.Key).IsUnique();
        }
    }
}

namespace Geaux.Localization.EFCore.EntityConfiguration
{
    internal class LocalizationCulturesEntityConfiguration : IEntityTypeConfiguration<LocalizationCulture>
    {
        public void Configure(EntityTypeBuilder<LocalizationCulture> b)
        {
            b.ToTable("LocalizationCultures");
            b.HasKey(x => x.Id);
            b.Property(x => x.CultureCode).IsRequired().HasMaxLength(16);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.FlagCode).HasMaxLength(8);
            b.Property(x => x.FallbackCulture).HasMaxLength(16);
            b.HasIndex(x => x.CultureCode).IsUnique();
        }
    }
}

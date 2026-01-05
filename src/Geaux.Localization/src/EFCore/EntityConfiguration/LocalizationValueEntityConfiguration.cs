namespace Geaux.Localization.EFCore.EntityConfiguration
{
    internal class LocalizationValueEntityConfiguration : IEntityTypeConfiguration<LocalizationValue>
    {
        public void Configure(EntityTypeBuilder<LocalizationValue> e)
        {
            e.HasOne(v => v.LocalizationKey)
             .WithMany(k => k.Values)
             .HasForeignKey(v => v.LocalizationKeyId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(v => new { v.LocalizationKeyId, v.Culture })
             .IsUnique()
             .HasFilter("[TenantId] IS NULL");

            e.HasIndex(v => new { v.TenantId, v.LocalizationKeyId, v.Culture })
             .IsUnique()
             .HasFilter("[TenantId] IS NOT NULL");
        }
    }
}

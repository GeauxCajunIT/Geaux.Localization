namespace Geaux.Localization.Models
{
    public sealed class LocalizationKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsSystem { get; set; }

        public ICollection<LocalizationValue> Values { get; set; } = new List<LocalizationValue>();
    }
}

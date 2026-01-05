public static class LocalizationKeyScanner
{
    public static IReadOnlyList<string> ScanAll(IEnumerable<Assembly> assemblies)
    {
        HashSet<string> keys = new HashSet<string>();

        foreach (Assembly assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (Type type in types)
            {
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    LocalizedAttribute? attr = prop.GetCustomAttribute<LocalizedAttribute>();
                    if (attr == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(attr.Key))
                        keys.Add(attr.Key);

                    if (!string.IsNullOrWhiteSpace(attr.DisplayNameKey))
                        keys.Add(attr.DisplayNameKey);

                    if (!string.IsNullOrWhiteSpace(attr.DisplayMessageKey))
                        keys.Add(attr.DisplayMessageKey);

                    if (!string.IsNullOrWhiteSpace(attr.ErrorMessageKey))
                        keys.Add(attr.ErrorMessageKey);
                }
            }
        }

        return keys.ToList();
    }
}

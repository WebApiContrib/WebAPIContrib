using System.Text.RegularExpressions;

namespace WebApiContrib.Serialization
{
    public class UnderscoreMappingResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, "([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4", RegexOptions.Compiled).ToLower();
        }
    }
}
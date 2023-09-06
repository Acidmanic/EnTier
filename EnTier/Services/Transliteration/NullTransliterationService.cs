using EnTier.Contracts;

namespace EnTier.Services.Transliteration;

public class NullTransliterationService:ITransliterationService
{
    public string Transliterate(string text)
    {
        return text;
    }
}
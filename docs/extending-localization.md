# Adding a language

The package ships English and Brazilian Portuguese. Adding a third language is three edits.

Culture matching is by **prefix**, case-insensitive: any `Culture` starting with `pt` selects
Portuguese, so `pt-BR` and `pt-PT` both work. Anything unrecognized falls back to English.

## 1. Add the messages

In `Constants/ErrorMessages.cs`:

```csharp
/// <summary>
/// Error messages in German.
/// </summary>
public static class GermanErrorMessages
{
    public const string InternalServerError = "Ein unerwarteter Fehler ist aufgetreten.";
    public const string ValidationError = "Ein oder mehrere Validierungsfehler sind aufgetreten.";
    public const string NotFound = "Ressource nicht gefunden.";
    public const string AccessDenied = "Zugriff verweigert.";
    public const string InconsistentState = "Operation führte zu einem inkonsistenten Zustand.";
    public const string BusinessRule = "Eine Geschäftsregelvalidierung ist fehlgeschlagen.";
}
```

## 2. Add the lookup

In `Localization/DefaultErrorLocalizer.cs`, mirroring the existing methods:

```csharp
private static string GetGermanMessage(string key)
    => key switch
    {
        ErrorMessageKeys.InternalServerError => GermanErrorMessages.InternalServerError,
        ErrorMessageKeys.ValidationError => GermanErrorMessages.ValidationError,
        ErrorMessageKeys.NotFound => GermanErrorMessages.NotFound,
        ErrorMessageKeys.AccessDenied => GermanErrorMessages.AccessDenied,
        ErrorMessageKeys.InconsistentState => GermanErrorMessages.InconsistentState,
        ErrorMessageKeys.BusinessRule => GermanErrorMessages.BusinessRule,
        _ => GetEnglishMessage(key),          // always fall back
    };
```

The `_ => GetEnglishMessage(key)` arm matters: a key added later to the package still returns
something readable instead of an empty string.

## 3. Route the culture to it

In the same file, in `Get`:

```csharp
public string Get(string key)
{
    if (_options.CustomMessages.TryGetValue(key, out var customMessage))
        return customMessage;

    // Ordinal: a culture tag is an identifier, not user-facing text. A culture-sensitive
    // comparison would make the result depend on the thread's current culture.
    if (_options.Culture?.StartsWith("pt", StringComparison.OrdinalIgnoreCase) ?? false)
        return GetPortugueseBRMessage(key);

    if (_options.Culture?.StartsWith("de", StringComparison.OrdinalIgnoreCase) ?? false)
        return GetGermanMessage(key);

    return GetEnglishMessage(key);
}
```

Custom messages are checked first, so `CustomMessages["ValidationError"]` overrides every
language.

## 4. Test it

```csharp
services.AddErrorHandling(options => options.Culture = "de-DE");
```

Add a case to `DefaultErrorLocalizerTests` covering the new culture and the fallback for an
unknown key.

## Notes

- Keep messages short and addressed to the API consumer, not the developer.
- Match on the prefix, not the full tag, so regional variants work without extra code.
- Update the README's list of supported languages.

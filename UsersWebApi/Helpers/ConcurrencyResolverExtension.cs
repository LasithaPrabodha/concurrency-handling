namespace UsersWebApi.Helpers;

public static class ConcurrencyResolverExtension
{
    public static void ResolveConcurrency<T>(this T source, int hash)
    {
        if (source != null && source.GetHashCode() != hash)
            throw new PreconditionFailedException();
    }
}


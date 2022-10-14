using UsersWebApi.Services;

namespace UsersWebApi.Helpers
{
    public static class ConcurrencyResolverExtension
    {
        public static void ResolveConcurrency(this RowVersionBase source, byte[] etag)
        {
            if (Convert.ToInt64(ByteArrayToHexString(source.RowVersion), 16) > Convert.ToInt64(ByteArrayToHexString(etag), 16))
                throw new PreconditionFailedException();
        }

        private static string ByteArrayToHexString(byte[] b)
        {
            System.Text.StringBuilder sb = new();

            sb.Append("0x");

            foreach (byte val in b)
            {
                sb.Append(val.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

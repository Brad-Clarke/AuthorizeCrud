using System;

namespace ModelAuthorization.Enums
{
    [Flags]
    public enum CrudPermission
    {
        None = 0,
        Create = 2,
        Read = 4,
        Update = 8,
        Delete = 16
    }
}

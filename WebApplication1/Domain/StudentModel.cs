using ModelAuthorization.Attributes;
using ModelAuthorization.Enums;

namespace WebApplication1.Domain
{
    [AllowCrud("user.admin", CrudPermission.Create | CrudPermission.Read | CrudPermission.Update | CrudPermission.Delete)]
    public class StudentModel
    {
        public Guid Id { get; set; }

        [RestrictCrud(CrudPermission.Update)]
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
    }
}

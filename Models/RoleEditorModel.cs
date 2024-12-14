namespace Sklep2.Models
{
    public class RoleEditorModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleAssignmentModel> Roles { get; set; }
        public string SelectedRole { get; set; }
    }

    public class RoleAssignmentModel
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}

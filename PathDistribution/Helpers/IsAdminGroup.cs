using PathDistribution.Models.DAL;
using System.Web;

namespace PathDistribution.Helpers
{
    public static class IsAdminGroup
    {
        private static bool isAdmin = false;
        private static bool RoleChecked = false;
        private static string copathAbbr = string.Empty;

        public static bool IsAdmin(this HttpContext context)
        {
            if (!RoleChecked)
            {
                AdminDAL admin = new AdminDAL();
                bool check = admin.IsAdmin(context.User.Identity.Name.Replace("SARAPATH\\", ""));
                if (!check)
                    check = context.User.IsInRole("MIS Department");
                isAdmin = check;
                RoleChecked = true;
            }
            return isAdmin;
        }

        public static string CopathAbbr(this HttpContext context)
        {
            if (string.IsNullOrEmpty(copathAbbr))
            {
                AdminDAL admin = new AdminDAL();
                copathAbbr = admin.GetCopathAbbr(context.User.Identity.Name.Replace("SARAPATH\\", ""));
            }
            return copathAbbr;
        }
    }
}
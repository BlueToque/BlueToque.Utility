namespace BlueToque.Utility.Windows
{
    public static class IdentityHelper
    {
        /// <summary>
        /// Call this from windows to initialze the windows identity provider
        /// </summary>
        /// <param name="systemIdentity"></param>
        public static void InitializeWindows(this SystemIdentity systemIdentity) => systemIdentity.Register(new WindowsIdentityProvider());
    }

    /// <summary>
    /// Implement identity on windows
    /// </summary>
    class WindowsIdentity : IIdentity
    {
        public string Name { get; set; } = "";

        public string Issuer => "Windows";
    }

    /// <summary>
    /// The windows identity providers
    /// </summary>
    public class WindowsIdentityProvider : IIdentityProvider
    {
        public IIdentity GetCurrent() => 
            new WindowsIdentity()
            {
                Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
            };
    }
}

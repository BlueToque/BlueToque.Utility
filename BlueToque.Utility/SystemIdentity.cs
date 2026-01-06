namespace BlueToque.Utility
{
    public interface IIdentity
    {
        string Name { get; }
        string Issuer { get; }
    }

    public interface IIdentityProvider
    {
        IIdentity GetCurrent();
    }

    public class DefaultIdentity : IIdentity
    {
        public DefaultIdentity()
        {
            Name = "Default";
            Issuer = "Default";
        }

        public string Name { get; }
        public string Issuer { get; }

    }

    class DefaultProvider : IIdentityProvider
    {
        public IIdentity GetCurrent() => new DefaultIdentity();
    }

    public class SystemIdentity : Singleton<SystemIdentity> 
    {
        public IIdentityProvider Provider { get; private set; } = new DefaultProvider();

        public void Register(IIdentityProvider provider) => Provider = provider;

        public IIdentity GetCurrent() => Provider.GetCurrent();
    }
}

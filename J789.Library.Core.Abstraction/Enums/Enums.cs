namespace J789.Library.Core.Abstraction.Enums
{
    public enum ContextType
    {
        User,
        System,
        Integration,
        Anonymous
    }

    public enum IdentityIntegrationType
    {
        Local,
        AmazonCognito
    }

    public enum SocialProviderType
    {
        None,
        Facebook,
        Google,
        Amazon
    }
}

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
        AmazonCognito,
        Auth0
    }

    public enum SocialProviderType
    {
        None,
        Facebook,
        Google,
        Amazon
    }
}

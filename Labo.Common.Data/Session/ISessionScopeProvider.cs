namespace Labo.Common.Data.Session
{
    public interface ISessionScopeProvider
    {
        ISessionScope CreateSessionScope(SessionScopeOption sessionScopeOption = SessionScopeOption.Required);
    }
}
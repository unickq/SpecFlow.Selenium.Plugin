namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public interface ICustomRemoteWebDriver
    {
        void UpdateTestResult();
        string SessionId { get; }
        string SecretUser { get; }
        string SecretKey { get; }
    }
}

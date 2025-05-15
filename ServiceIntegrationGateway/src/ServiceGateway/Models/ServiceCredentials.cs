namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Represents credentials for an external service.
/// This flexible model can hold various types of credential information (e.g., username/password, API key, OAuth token)
/// required to authenticate with external services. It is used by the ICredentialManager.
/// </summary>
public class ServiceCredentials
{
    /// <summary>
    /// Gets or sets the username, typically used for basic authentication or similar schemes.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Gets or sets the password, typically used with a username.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets or sets the API key, used for services that authenticate via a single key.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the token, such as an OAuth bearer token or a session token.
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCredentials"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="apiKey">The API key.</param>
    /// <param name="token">The token.</param>
    public ServiceCredentials(string? username = null, string? password = null, string? apiKey = null, string? token = null)
    {
        Username = username;
        Password = password;
        ApiKey = apiKey;
        Token = token;
    }
}
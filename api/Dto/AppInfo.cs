namespace Api.Dto;

/// <summary>
/// Represents Application information
/// </summary>
/// <param name="VersionNumber">Readonly version number of the application</param>
public record class AppInfo(string VersionNumber) { }

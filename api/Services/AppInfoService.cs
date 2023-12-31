﻿using System.Reflection;
using Api.Dto;

namespace Api.Services;

/// <summary>
/// Contains functionality for retrieving application information.
/// </summary>
public interface IAppInfoService
{
    /// <summary>
    /// Retrieves application information.
    /// </summary>
    /// <returns>An instance containing information about the running application.</returns>
    AppInfo GetApiInfo();
}

public class AppInfoService : IAppInfoService
{
    public AppInfo GetApiInfo()
    {
        var appInfo = new AppInfo(GetVersionNumber());
        return appInfo;
    }

    private static string GetVersionNumber()
    {
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version!;
        var versionNum = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        return versionNum;
    }
}

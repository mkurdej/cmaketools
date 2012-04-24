// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace CMakeTools
{
    /// <summary>
    /// Visual Studio package class for CMake Tools for Visual Studio.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#100", "#101", "1.0", IconResourceID = 400)]
    [ProvideService(typeof(CMakeLanguageService),
        ServiceName = "CMake Language Service")]
    [ProvideLanguageService(typeof(CMakeLanguageService), "CMake", 102,
        RequestStockColors = true)]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".cmake")]
    [Guid("2986e95d-f97a-45a7-be3e-f07f1a931950")]
    class CMakePackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            // Register the language service.
            IServiceContainer container = this as IServiceContainer;
            CMakeLanguageService service = new CMakeLanguageService();
            service.SetSite(this);
            container.AddService(typeof(CMakeLanguageService), service, true);
        }
    }
}

// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
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
        RequestStockColors = true,
        CodeSense = true,
        ShowCompletion = true,
        EnableCommenting = true)]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".cmake")]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".txt")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(CMakeGuids.guidCMakeTools)]
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

            // Register callback to respond to menu command.
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService))
                as OleMenuCommandService;
            if (mcs != null)
            {
                CommandID cmdid = new CommandID(new Guid(CMakeGuids.guidCMakeCmdSet),
                    (int)CMakeCmdIds.cmdidCMake);
                MenuCommand menuItem = new MenuCommand(CMakeMenuCallback, cmdid);
                mcs.AddCommand(menuItem);
            }
        }

        private void CMakeMenuCallback(object sender, EventArgs e)
        {
            // Attempt to find CMake in the registry.
            string location = null;
            RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                RegistryView.Registry32);
            RegistryKey kitwareKey = localMachine.OpenSubKey("Software\\Kitware");
            if (kitwareKey != null)
            {
                foreach (string keyName in kitwareKey.GetSubKeyNames())
                {
                    if (!keyName.StartsWith("CMake"))
                    {
                        continue;
                    }
                    RegistryKey cmakeKey = kitwareKey.OpenSubKey(keyName);
                    if (cmakeKey.GetValueKind(null) == RegistryValueKind.String)
                    {
                        location = cmakeKey.GetValue(null) as string;
                        if (location != null)
                        {
                            break;
                        }
                    }
                }
            }

            // If we found CMake, attempt to spawn it.
            if (location != null)
            {
                bool launchFailed = false;
                try
                {
                    Process process = Process.Start(Path.Combine(location,
                        "bin\\cmake-gui.exe"));
                    if (process != null)
                    {
                        process.Dispose();
                    }
                    else
                    {
                        launchFailed = true;
                    }
                }
                catch (Exception)
                {
                    launchFailed = true;
                }
                if (launchFailed)
                {
                    // Display an error message that CMake failed to launch.
                    MessageBox.Show(CMakeStrings.FailedToLaunchCMake,
                        CMakeStrings.MessageBoxTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                }
            }
            else
            {
                // Display an error message that CMake could not be found.
                MessageBox.Show(CMakeStrings.CMakeNotFound, CMakeStrings.MessageBoxTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}

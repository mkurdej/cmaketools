/* ****************************************************************************
 * 
 * Copyright (C) 2012 by David Golub.  All rights reserved.
 * 
 * This software is subject to the Microsoft Reciprocal License (Ms-RL).
 * A copy of the license can be found in the License.txt file included
 * in this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 * 
 * **************************************************************************/

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualStudio.OLE.Interop;
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
        EnableCommenting = true,
        AutoOutlining = true)]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".cmake")]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".txt")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(CMakeGuids.guidCMakeTools)]
    class CMakePackage : Package, IOleComponent
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

            // Register this object as an OLE component.  This is boilerplate code that
            // every language service package must have in order for the language
            // service's OnIdle method to be called.
            IOleComponentManager manager =
                (IOleComponentManager)GetService(typeof(SOleComponentManager));
            if (manager != null)
            {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                    (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                    (uint)_OLECADVF.olecadvfRedrawOff |
                    (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 100;
                uint componentID = 0;
                manager.FRegisterComponent(this, crinfo, out componentID);
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

        // The implementation of IOleComponent is boilerplate code that every language
        // service package must provide in order for the language service's OnIdle
        // method to be called.  The implementation FDoIdle manually calls OnIdle on the
        // language service.  The other methods are all stubbed out.
        #region IOleComponent Implementation
        public virtual int FDoIdle(uint grfidlef)
        {
            bool periodic = ((grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0);
            CMakeLanguageService service =
                (CMakeLanguageService)GetService(typeof(CMakeLanguageService));
            if (service != null)
            {
                service.OnIdle(periodic);
            }
            return 0;
        }

        public virtual void Terminate()
        {
        }

        public virtual int FPreTranslateMessage(MSG[] msg)
        {
            return 0;
        }

        public virtual void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public virtual void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public virtual void OnLoseActivation()
        {
        }

        public virtual void OnActivationChange(IOleComponent pic, int fSameComponent,
            OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo,
            uint dwReserved)
        {
        }

        public virtual int FContinueMessageLoop(uint uReason, IntPtr pvLoopData,
            MSG[] pMsgPeeked)
        {
            return 1;
        }

        public virtual int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public virtual IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public virtual int FReserved1(uint reserved, uint message, IntPtr wParam,
            IntPtr lParam)
        {
            return 1;
        }
        #endregion
    }
}

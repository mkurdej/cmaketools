/* ****************************************************************************
 * 
 * Copyright (C) 2012-2014 by David Golub.  All rights reserved.
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Visual Studio package class for CMake Tools for Visual Studio.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#100", "#101", "1.3", IconResourceID = 400)]
    [ProvideService(typeof(CMakeLanguageService),
        ServiceName = "CMake Language Service")]
    [ProvideLanguageService(typeof(CMakeLanguageService), "CMake", 102,
        RequestStockColors = true,
        CodeSense = true,
        ShowCompletion = true,
        EnableCommenting = true,
        AutoOutlining = true,
        QuickInfo = true,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        ShowSmartIndent = true)]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".cmake")]
    [ProvideLanguageExtension(typeof(CMakeLanguageService), ".txt")]
    [ProvideLanguageCodeExpansion(typeof(CMakeLanguageService), "CMake", 102, "CMake",
        @"%%InstallRoot%%\CMake\Snippets\%%LCID%%\SnippetsIndex.xml",
        SearchPaths = @"%InstallRoot%\CMake\Snippets\%LCID%\Snippets\;" +
            @"%MyDocs%\Code Snippets\CMake\My Code Snippets\",
        ForceCreateDirs = @"%MyDocs%\Code Snippets\CMake\My Code Snippets\")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(CMakeOptionPage), "CMake Tools", "Advanced", 103, 104,
        false)]
    [ProvideProfile(typeof(CMakeOptionPage), "CMake Tools", "Advanced", 103, 104,
        true)]
    [ProvideProjectItem(VSConstants.CLSID.MiscellaneousFilesProject_string, "CMake",
        "Templates\\NewItems", 10)]
    [ProvideFileFilter(VSConstants.CLSID.MiscellaneousFilesProject_string, "CMake",
        "CMake Files (*.cmake);*.cmake", 10)]
    [ProvideEditorFactory(typeof(CMakeEditorFactory), 108)]
    [ProvideEditorExtension(typeof(CMakeEditorFactory), ".cmake", 100)]
    [ProvideEditorExtension(typeof(CMakeEditorFactory), ".*", 1)]
    [Guid(CMakeGuids.guidCMakeTools)]
    public class CMakePackage : Package, IOleComponent
    {
        /// <summary>
        /// The one and only instance of the class.
        /// </summary>
        public static CMakePackage Instance { get; private set; }

        public CMakePackage()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Register the editor factory.
            RegisterEditorFactory(new CMakeEditorFactory(this));

            // Register the language service.
            IServiceContainer container = this as IServiceContainer;
            CMakeLanguageService service = new CMakeLanguageService();
            service.SetSite(this);
            container.AddService(typeof(CMakeLanguageService), service, true);

            // Register callbacks to respond to menu commands.
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService))
                as OleMenuCommandService;
            if (mcs != null)
            {
                RegisterMenuCallback(mcs, CMakeCmdIds.cmdidCMake, CMakeMenuCallback);
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelp,
                    "html\\index.html", "cmake.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpCommands,
                    "html\\manual\\cmake-commands.7.html", "cmake-commands.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpModules,
                    "html\\manual\\cmake-modules.7.html", "cmake-modules.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpProperties,
                    "html\\manual\\cmake-properties.7.html", "cmake-properties.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpVariables,
                    "html\\manual\\cmake-variables.7.html", "cmake-variables.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpCPack,
                    "html\\manual\\cpack.1.html", "cpack.html");
                RegisterHelpMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpCTest,
                    "html\\manual\\ctest.1.html", "ctest.html");
                RegisterMenuCallback(mcs, CMakeCmdIds.cmdidCMakeHelpWebSite,
                    CMakeHelpWebSiteMenuCallback);
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

        private void RegisterMenuCallback(OleMenuCommandService mcs, uint cmdid,
            EventHandler handler)
        {
            // Helper function to register a callback for a menu command.
            CommandID cmdidObj = new CommandID(new Guid(CMakeGuids.guidCMakeCmdSet),
                (int)cmdid);
            MenuCommand menuItem = new MenuCommand(handler, cmdidObj);
            mcs.AddCommand(menuItem);
        }
        
        private void RegisterHelpMenuCallback(OleMenuCommandService mcs, uint cmdid,
            params string[] htmlFiles)
        {
            // Register a callback to open the specified HTML file in the CMake
            // documentation directory when the specified menu command is selected.
            RegisterMenuCallback(mcs, cmdid, (sender, e) => OpenCMakeHelpPage(htmlFiles));
        }

        private void CMakeMenuCallback(object sender, EventArgs e)
        {
            // Attempt to find CMake in the registry.
            string location = CMakePath.FindCMake();

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

        private void CMakeHelpWebSiteMenuCallback(object sender, EventArgs e)
        {
            // Open the CMake web site.
            OpenWebPage("http://www.cmake.org");
        }

        private void OpenWebPage(string url)
        {
            // Open the specified URL in Visual Studio's built-in web browser.
            IVsWebBrowsingService service = (IVsWebBrowsingService)GetService(
                typeof(SVsWebBrowsingService));
            if (service != null)
            {
                IVsWindowFrame frame;
                service.Navigate(url, (uint)__VSWBNAVIGATEFLAGS.VSNWB_ForceNew,
                    out frame);
            }
        }

        /// <summary>
        /// Open the specified CMake Help HTML file in Visual Studio's built-in web
        /// browser.
        /// </summary>
        /// <param name="fileName">The name of HTML file to open.</param>
        public void OpenCMakeHelpPage(params string[] fileNames)
        {
            // Attempt to find CMake in the registry.
            string location = CMakePath.FindCMakeHelp();

            // If we found CMake, attempt to open the request help page in
            // Visual Studio's built-in web browser.
            if (location != null)
            {
                foreach (string fileName in fileNames)
                {
                    string absolutePath = Path.Combine(location, fileName);
                    int hashPos = absolutePath.IndexOf('#');
                    string strippedPath =
                        hashPos >= 0 ? absolutePath.Substring(0, hashPos) : absolutePath;
                    if (File.Exists(strippedPath))
                    {
                        OpenWebPage(absolutePath);
                        break;
                    }
                }
            }
            else
            {
                // Display an error message that CMake could not be found.
                MessageBox.Show(CMakeStrings.CMakeNotFound, CMakeStrings.MessageBoxTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        public CMakeOptionPage CMakeOptionPage
        {
            get
            {
                return (CMakeOptionPage)GetDialogPage(typeof(CMakeOptionPage));
            }
        }

        internal static bool IsDeprecatedWarningEnabled()
        {
            return Instance.CMakeOptionPage.ShowDeprecatedWarning;
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

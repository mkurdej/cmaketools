// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Language service for CMake.
    /// </summary>
    class CMakeLanguageService : LanguageService
    {
        private LanguagePreferences _preferences;

        public override string GetFormatFilterList()
        {
            return "CMake Files (*.cmake)\n*.cmake";
        }

        public override string Name
        {
            get
            {
                return "CMake";
            }
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new CMakeAuthoringScope();
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new CMakeScanner();
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(Site,
                    typeof(CMakeLanguageService).GUID, Name);
                _preferences.Init();
            }
            return _preferences;
        }
    }
}

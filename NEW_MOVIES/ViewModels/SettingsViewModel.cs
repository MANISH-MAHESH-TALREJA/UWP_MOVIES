﻿using System;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using NEW_MOVIES.Helpers;
using NEW_MOVIES.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace NEW_MOVIES.ViewModels
{
    //For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : ViewModelBase
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme {
            get {
                return _elementTheme;
            }

            set {
                Set(ref _elementTheme, value);
            }
        }

        private bool _noticeSwitchValue;

        public bool NoticeSwitchValue {
            get => _noticeSwitchValue;
            set => Set(ref _noticeSwitchValue, value);
        }

        private bool _guessLikeSwitchValue;

        public bool GuessLikeSwitchValue {
            get => _guessLikeSwitchValue;
            set => Set(ref _guessLikeSwitchValue, value);
        }

        private string _versionDescription;

        public string VersionDescription {
            get {
                return _versionDescription;
            }

            set {
                Set(ref _versionDescription, value);
            }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand {
            get {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public SettingsViewModel()
        {
        }

        public void Initialize()
        {
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}

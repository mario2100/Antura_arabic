using Antura.Audio;
using System;
using UnityEngine;

namespace Antura.Core
{
    public class AppSettingsManager
    {
        private const string SETTINGS_PREFS_KEY = "OPTIONS";
        private AppSettings _settings = new AppSettings();

        public AppSettings Settings
        {
            get { return _settings; }
            set {
                if (value != _settings) {
                    _settings = value;
                    // Auto save at any change
                    SaveSettings();
                } else {
                    _settings = value;
                }
            }
        }

        public bool IsAppJustUpdated;
        public Version AppVersionPrevious;

        /// <summary>
        /// Loads the settings. Creates new settings if none are found.
        /// </summary>
        public AppSettings LoadSettings()
        {
            if (PlayerPrefs.HasKey(SETTINGS_PREFS_KEY)) {
                var serializedObjs = PlayerPrefs.GetString(SETTINGS_PREFS_KEY);
                Settings = JsonUtility.FromJson<AppSettings>(serializedObjs);
            } else {
                Settings = new AppSettings();
            }

            AudioManager.I.MusicEnabled = Settings.MusicEnabled;
            // Debug.Log("Setting music to " + Settings.MusicOn);
            return _settings;
        }

        /// <summary>
        /// Save all settings. This also saves player profiles.
        /// </summary>
        public void SaveSettings()
        {
            var serializedObjs = JsonUtility.ToJson(Settings);
            PlayerPrefs.SetString(SETTINGS_PREFS_KEY, serializedObjs);
            PlayerPrefs.Save();
            //Debug.Log("AppSettingsManager SaveSettings()");
        }

        /// <summary>
        /// Delete all settings. This also deletes all player profiles.
        /// </summary>
        public void DeleteAllSettings()
        {
            PlayerPrefs.DeleteAll();
        }

        #region external API to save single settings
        public void SaveMusicSetting(bool musicOn)
        {
            Settings.MusicEnabled = musicOn;
            SaveSettings();
        }
        #endregion

        public void UpdateAppVersion()
        {
            Debug.Log("UpdateAppVersion() " + Settings.AppVersion);
            if (Settings.AppVersion == "") {
                IsAppJustUpdated = true;
                AppVersionPrevious = new Version(0, 0, 0, 0);
            } else {
                AppVersionPrevious = new Version(Settings.AppVersion);
                IsAppJustUpdated = AppConfig.AppVersion > AppVersionPrevious;
            }
            Debug.Log("isAppJustUpdated " + IsAppJustUpdated + " previous: " + AppVersionPrevious + " current: " + AppConfig.AppVersion);
            Settings.SetAppVersion(AppConfig.AppVersion);
            SaveSettings();
        }

        public void AppUpdateCheckDone()
        {
            IsAppJustUpdated = false;
        }

        public void EnableOnlineAnalytics(bool status)
        {
            Settings.OnlineAnalyticsEnabled = status;
            SaveSettings();
        }

        public void DeleteAllPlayers()
        {
            Settings.DeletePlayers();
            SaveSettings();
        }
    }
}
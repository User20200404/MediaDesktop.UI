using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using IniParser.Parser;
using IniParser;
using Windows.Storage;
using MediaDesktop.UI.ViewModels;
using System.ComponentModel;

namespace MediaDesktop.UI.Services
{
    public class MediaDesktopItemViewModelConfig
    {
        private ViewModelCollection viewModelCollection;
        private FileIniDataParser FileIniDataParser = new FileIniDataParser();
        private ObservableCollection<MediaDesktopItemViewModel> ViewModels { get { return viewModelCollection.ViewModelItems; } }

        public static string ConfigPath
        {
            get { return GlobalResources.ViewModelCollection.SettingsItemViewModel.MediaItemRecordINIPath; }
        }

        public MediaDesktopItemViewModelConfig(ViewModelCollection viewModelCollection)
        {
            this.viewModelCollection = viewModelCollection;
        }

        private IniData EncodeIniDataWithViewModel()
        {
            /*
            IniData iniData = new IniData();

            foreach (var model in ViewModels)
            {
                SectionData section = new SectionData(model.MediaPath);
                section.Keys.AddKey(nameof(model.MediaPath), model.MediaPath);
                section.Keys.AddKey(nameof(model.ImagePath), model.ImagePath);
                section.Keys.AddKey(nameof(model.Title), model.Title);
                section.Keys.AddKey(nameof(model.SubTitle), model.SubTitle);
                section.Keys.AddKey(nameof(model.IsFavourite), model.IsFavourite.ToString());
                section.Keys.AddKey(nameof(model.HistoryLevel), model.HistoryLevel.ToString());
                iniData.Sections.Add(section);
            }

            return iniData;*/
            return ViewModels.EncodeIniData();
        }


        /// <summary>
        /// Fills <see cref="ViewModels"/> with items generated from default ini config. [WARNING]This method will clear all the existing items in <see cref="ViewModels"/>.
        /// </summary>
        public void InitViewModel()
        {
            InitViewModelWith(ConfigPath);
        }

        /// <summary>
        /// Fills <see cref="ViewModels"/> with items generated from specified ini config. [WARNING]This method will clear all the existing items in <see cref="ViewModels"/>.
        /// </summary>
        /// <param name="configPath">path of ini config.</param>
        public void InitViewModelWith(string configPath)
        {
            /*
            if (System.IO.File.Exists(configPath))
            {
                IniData iniData = FileIniDataParser.ReadFile(configPath);
                if (iniData.Sections.Count > 0)
                {
                    ViewModels.Clear();

                    foreach (var section in iniData.Sections)
                    {
                        MediaDesktopItemViewModel viewModel = new MediaDesktopItemViewModel()
                        {
                            MediaPath = section.Keys[nameof(viewModel.MediaPath)],
                            ImagePath = section.Keys[nameof(viewModel.ImagePath)],
                            Title = section.Keys[nameof(viewModel.Title)],
                            SubTitle = section.Keys[nameof(viewModel.SubTitle)],
                            IsFavourite = bool.Parse(section.Keys[nameof(viewModel.IsFavourite)]),
                            HistoryLevel = int.Parse(section.Keys[nameof(viewModel.HistoryLevel)])
                        };
                        viewModel.MediaItemViewModel.LoadMedia(GlobalResources.LibVLC);
                        ViewModels.Add(viewModel);
                    }
                }
            }*/

            ViewModels.InitMediaDesktopItemViewModelCollectionFromINI(configPath);
        }

        public void Save()
        {
            SaveTo(ConfigPath);
        }

        public void SaveTo(string configPath)
        {
            IniData iniData = EncodeIniDataWithViewModel();
            FileIniDataParser.WriteFile(configPath, iniData);
        }

    }
    /*
    public class RuntimeConfig
    {
        private FileIniDataParser FileIniDataParser = new FileIniDataParser();
        public int Volume { get; set; }
        public PlayBackMode PlayBackMode { get; set; }

        public static string ConfigPath
        {
            get { return ApplicationData.Current.LocalCacheFolder.Path + @"\base.ini"; }
        }
        public RuntimeConfig()
        {
            Initialize();
        }

        #region Methods


        public void Save()
        {
            SaveTo(ConfigPath);
        }

        public void SaveTo(string configPath)
        {
            IniData data = new IniData();
            data.Sections.AddSection(nameof(Volume));
            data.Sections.AddSection(nameof(PlayBackMode));
            data.Sections[nameof(Volume)].AddKey("Value", Volume.ToString());
            data.Sections[nameof(PlayBackMode)].AddKey("Value", PlayBackMode.ToString());
            FileIniDataParser.WriteFileMerged(configPath, data);
        }
        #endregion
        #region Inner Methods
        private void Initialize()
        {
            if (System.IO.File.Exists(ConfigPath))
            {
                IniData iniData = FileIniDataParser.ReadFile(ConfigPath);
                try
                {
                    Volume = int.Parse(iniData.Sections[nameof(Volume)]["Value"]);
                }
                catch (Exception)
                {
                    Volume = 100;
                }

                try
                {
                    PlayBackMode = (PlayBackMode)Enum.Parse(typeof(PlayBackMode), iniData.Sections[nameof(PlayBackMode)]["Value"]);
                }
                catch (Exception)
                {
                    PlayBackMode = PlayBackMode.Shuffle;
                }
            }
        }
        #endregion

    }
    */
}

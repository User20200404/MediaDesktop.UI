using IniParser;
using IniParser.Model;
using MediaDesktop.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Services
{
    public class MediaPlayingListConfig
    {
        //private readonly string defaultKey = "Value";
        private ViewModelCollection viewModelCollection;
        private FileIniDataParser FileIniDataParser = new FileIniDataParser();
        private ObservableCollection<MediaPlayingListViewModel> ViewModels
        {
            get { return viewModelCollection.MediaPlayingListViewModels; }
        }
        public static string ConfigDir { get { return GlobalResources.ViewModelCollection.SettingsItemViewModel.MediaPlayingListINIDir; } }
        public MediaPlayingListConfig(ViewModelCollection viewModelCollection)
        {
            this.viewModelCollection = viewModelCollection;
        }


        public void InitViewModel()
        {
            InitViewModelWith(ConfigDir);
        }

        public void InitViewModelWith(string configDir)
        {
            string defFile = configDir + @"\_lists.ini";

            //defFile stores every MediaPlayingList's Title, CoverImagePath, CreatedTime, ModifiedTime and path of collection file.
            //collection file stores the MediaPlayingList's Collection.

            if (File.Exists(defFile))
            {
                IniData iniData = FileIniDataParser.ReadFile(defFile);
                foreach (var sectionData in iniData.Sections)
                {
                    string collectionFilePath = configDir + "\\" + sectionData.SectionName;
                    MediaPlayingListViewModel model = new MediaPlayingListViewModel();
                    model.Title = iniData.GetStringValueOrDefault(sectionData.SectionName, nameof(model.Title), "未命名");
                    model.Description = iniData.GetStringValueOrDefault(sectionData.SectionName, nameof(model.Description), "自定义播放列表");
                    model.CoverImagePath = iniData.GetStringValueOrDefault(sectionData.SectionName, nameof(model.CoverImagePath), "");
                    model.CreatedTime = DateTime.Parse(iniData.GetStringValueOrDefault(sectionData.SectionName, nameof(model.CreatedTime), DateTime.MinValue.ToString()));
                    model.ModifiedTime = DateTime.Parse(iniData.GetStringValueOrDefault(sectionData.SectionName, nameof(model.ModifiedTime), DateTime.MinValue.ToString()));

                    var tempCollection = new ObservableCollection<MediaDesktopItemViewModel>();
                    tempCollection.InitMediaDesktopItemViewModelCollectionFromINI(collectionFilePath);
                    foreach (var item in tempCollection)
                    {
                        var i = viewModelCollection.ViewModelItems.Where(i => i.MediaPath == item.MediaPath).FirstOrDefault();
                        if (i != null)
                        {
                            model.MediaItems.Add(i);
                        }
                    }

                    ViewModels.Add(model);
                }
            }
        }

        public void Save()
        {
            SaveTo(ConfigDir);
        }

        public void SaveTo(string configDir)
        {
            IniData defFileData = new IniData();
            if(!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            foreach (var model in ViewModels)
            {
                string sectionName = "Item" + ViewModels.IndexOf(model) + ".ini";
                defFileData.Sections.AddSection(sectionName);
                defFileData.Sections[sectionName].AddKey(nameof(model.Title),model.Title);
                defFileData.Sections[sectionName].AddKey(nameof(model.Description),model.Description);
                defFileData.Sections[sectionName].AddKey(nameof(model.CoverImagePath),model.CoverImagePath);
                defFileData.Sections[sectionName].AddKey(nameof(model.CreatedTime),model.CreatedTime.ToString());
                defFileData.Sections[sectionName].AddKey(nameof(model.ModifiedTime),model.ModifiedTime.ToString());
                IniData iniData = model.MediaItems.EncodeIniData();
                FileIniDataParser.WriteFile(configDir +"\\"+ sectionName,iniData);
            }
            FileIniDataParser.WriteFile(configDir + "\\" + "_lists.ini", defFileData); 
        }


    }
}

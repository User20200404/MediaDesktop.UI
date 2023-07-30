using IniParser;
using IniParser.Model;
using MediaDesktop.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaDesktop.UI.ViewModels
{
    public static class ViewModelHelper
    {
        private static FileIniDataParser FileIniDataParser = new FileIniDataParser();

        /// <summary>
        /// [WARNING] This method will clear all existing elements in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configPath"></param>
        public static void InitMediaDesktopItemViewModelCollectionFromINI(this ObservableCollection<MediaDesktopItemViewModel> collection,string configPath)
        {
            if (System.IO.File.Exists(configPath))
            {
                IniData iniData = FileIniDataParser.ReadFile(configPath);
                if (iniData.Sections.Count > 0)
                {
                    collection.Clear();

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
                        //viewModel.MediaItemViewModel.LoadMedia(GlobalResources.LibVLC);
                        collection.Add(viewModel);
                    }
                }
            }
        }

        public static ObservableCollection<UpdateLogViewModel> GetUpdateLogViewModelCollectionFromJSON(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<ObservableCollection<UpdateLogViewModel>>(json);
        }

        public static IniData EncodeIniData(this ObservableCollection<MediaDesktopItemViewModel> collection)
        {
            IniData iniData = new IniData();

            foreach (var model in collection)
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

            return iniData;
        }
        public static MediaDesktopItemViewModel GetParentDesktopViewModel(this MediaItemViewModel viewModel)
        {
            MediaDesktopItemViewModel parentModel = GlobalResources.ViewModelCollection.ViewModelItems.
                FirstOrDefault(i => i.MediaItemViewModel == viewModel);
            return parentModel;
        }

        public static MediaDesktopItemViewModel GetParentDesktopViewModel(this MediaItemViewModel.RuntimeData dataSet)
        {
            MediaDesktopItemViewModel parentModel = GlobalResources.ViewModelCollection.ViewModelItems.
                FirstOrDefault(i => i.MediaItemViewModel.RuntimeDataSet == dataSet);
            return parentModel;
        }

        public static bool CurrentPlayingListContains(MediaDesktopItemViewModel viewModel)
        {
            return GlobalResources.ViewModelCollection.CurrentPlayingList.Contains(viewModel);
        }
    }
}

// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using Windows.Data.Json;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace MedicineGuide.Data
{
    /// <summary>
    /// Base class for <see cref="diseaseDataItem"/> and <see cref="diseaseDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class diseaseDataCommon : MedicineGuide.Common.BindableBase
    {
        internal  static Uri _baseUri = new Uri("ms-appx:///");

        public diseaseDataCommon(String uniqueId, String title, String shortTitle, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._shortTitle = shortTitle;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _shortTitle = string.Empty;
        public string ShortTitle
        {
            get { return this._shortTitle; }
            set { this.SetProperty(ref this._shortTitle, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;

        public Uri ImagePath
        {
            get
            {
                return new Uri(diseaseDataCommon._baseUri, this._imagePath); 
            }
        } 

        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(diseaseDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public string GetImageUri()
        {
            return _imagePath;
        }
    }

    /// <summary>
    /// disease item data model.
    /// </summary>
    public class diseaseDataItem : diseaseDataCommon
    {
        public diseaseDataItem()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }
        
        public diseaseDataItem(String uniqueId, String title, String shortTitle, String imagePath,  String treatments, ObservableCollection<string> medicines, diseaseDataGroup group)
            : base(uniqueId, title, shortTitle, imagePath)
        {
            
            this._treatments = treatments;
            this._medicines = medicines;
            this._group = group;
        }
        
        private string _treatments = string.Empty;
        public string treatments
        {
            get { return this._treatments; }
            set { this.SetProperty(ref this._treatments, value); }
        }

        private ObservableCollection<string> _medicines;
        public ObservableCollection<string> medicines
        {
            get { return this._medicines; }
            set { this.SetProperty(ref this._medicines, value); }
        }
    
        private diseaseDataGroup _group;
        public diseaseDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        private ImageSource _tileImage;
        private string _tileImagePath;

        public Uri TileImagePath
        {
            get
            {
                return new Uri(diseaseDataCommon._baseUri, this._tileImagePath);
            }
        }
        
        public ImageSource TileImage
        {
            get
            {
                if (this._tileImage == null && this._tileImagePath != null)
                {
                    this._tileImage = new BitmapImage(new Uri(diseaseDataCommon._baseUri, this._tileImagePath));
                }
                return this._tileImage;
            }
            set
            {
                this._tileImagePath = null;
                this.SetProperty(ref this._tileImage, value);
            }
        }

        public void SetTileImage(String path)
        {
            this._tileImage = null;
            this._tileImagePath = path;
            this.OnPropertyChanged("TileImage");
        }
    }

    /// <summary>
    /// disease group data model.
    /// </summary>
    public class diseaseDataGroup : diseaseDataCommon
    {
        public diseaseDataGroup()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }
        
        public diseaseDataGroup(String uniqueId, String title, String shortTitle, String imagePath, String description)
            : base(uniqueId, title, shortTitle, imagePath)
        {
        }

        private ObservableCollection<diseaseDataItem> _items = new ObservableCollection<diseaseDataItem>();
        public ObservableCollection<diseaseDataItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<diseaseDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _groupImage;
        private string _groupImagePath;  

        public ImageSource GroupImage
        {
            get
            {
                if (this._groupImage == null && this._groupImagePath != null)
                {
                    this._groupImage = new BitmapImage(new Uri(diseaseDataCommon._baseUri, this._groupImagePath));
                }
                return this._groupImage;
            }
            set
            {
                this._groupImagePath = null;
                this.SetProperty(ref this._groupImage, value);
            }
        }

        public int diseasesCount
        {
            get
            {
                return this.Items.Count; 
            } 
        } 

        public void SetGroupImage(String path)
        {
            this._groupImage = null;
            this._groupImagePath = path;
            this.OnPropertyChanged("GroupImage");
        }
    }

    /// <summary>
    /// Creates a collection of groups and items.
    /// </summary>
    public sealed class diseaseDataSource
    {
        //public event EventHandler diseasesLoaded;

        private static diseaseDataSource _diseaseDataSource = new diseaseDataSource();
        
        private ObservableCollection<diseaseDataGroup> _allGroups = new ObservableCollection<diseaseDataGroup>();
        public ObservableCollection<diseaseDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<diseaseDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _diseaseDataSource.AllGroups;
        }

        public static diseaseDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _diseaseDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static diseaseDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _diseaseDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task LoadRemoteDataAsync()
        {
            // Retrieve disease data from Azure
            var client = new HttpClient();
            client.MaxResponseContentBufferSize = 1024 * 1024; // Read up to 1 MB of data
            var response = await client.GetAsync(new Uri("http://contosodiseases8.blob.core.windows.net/AzurediseasesRP"));
            var result = await response.Content.ReadAsStringAsync();

            // Parse the JSON disease data
            var diseases = JsonArray.Parse(result);

            // Convert the JSON objects into diseaseDataItems and diseaseDataGroups
            CreatediseasesAnddiseaseGroups(diseases);
        }

        public static async Task LoadLocalDataAsync()
        {
            // Retrieve disease data from diseases.txt
            var file = await Package.Current.InstalledLocation.GetFileAsync("Data\\diseases.txt");
            var result = await FileIO.ReadTextAsync(file);

            // Parse the JSON disease data
            var diseases = JsonArray.Parse(result);

            // Convert the JSON objects into diseaseDataItems and diseaseDataGroups
            CreatediseasesAnddiseaseGroups(diseases);
        }

        private static void CreatediseasesAnddiseaseGroups(JsonArray array)
        {
            foreach (var item in array)
            {
                var obj = item.GetObject();
                diseaseDataItem disease = new diseaseDataItem();
                diseaseDataGroup group = null;

                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;

                    switch (key)
                    {
                        case "key":
                            disease.UniqueId = val.GetNumber().ToString();
                            break;
                        case "title":
                            disease.Title = val.GetString();
                            break;
                        case "shortTitle":
                            disease.ShortTitle = val.GetString();
                            break;
                        case "treatments":
                            disease.treatments = val.GetString();
                            break;
                        case "medicines":
                            var medicines = val.GetArray();
                            var list = (from i in medicines select i.GetString()).ToList();
                            disease.medicines = new ObservableCollection<string>(list);
                            break;
                        case "backgroundImage":
                            disease.SetImage(val.GetString());
                            break;
                        case "tileImage":
                            disease.SetTileImage(val.GetString());
                            break;
                        case "group":
                            var diseaseGroup = val.GetObject();

                            IJsonValue groupKey;
                            if (!diseaseGroup.TryGetValue("key", out groupKey))
                                continue;

                            group = _diseaseDataSource.AllGroups.FirstOrDefault(c => c.UniqueId.Equals(groupKey.GetString()));

                            if (group == null)
                                group = CreatediseaseGroup(diseaseGroup);

                            disease.Group = group;
                            break;
                    }
                }

                if (group != null)
                    group.Items.Add(disease);
            }
        }
        
        private static diseaseDataGroup CreatediseaseGroup(JsonObject obj)
        {
            diseaseDataGroup group = new diseaseDataGroup();

            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;

                switch (key)
                {
                    case "key":
                        group.UniqueId = val.GetString();
                        break;
                    case "title":
                        group.Title = val.GetString();
                        break;
                    case "shortTitle":
                        group.ShortTitle = val.GetString();
                        break;
                    case "description":
                        group.Description = val.GetString();
                        break;
                    case "backgroundImage":
                        group.SetImage(val.GetString());
                        break;
                    case "groupImage" :
                        group.SetGroupImage(val.GetString());
                        break; 
                }
            }

            _diseaseDataSource.AllGroups.Add(group);
            return group;
        }
    }
}

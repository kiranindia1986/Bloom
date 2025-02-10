using System.Collections.ObjectModel;
using Bloom.Models;
using Bloom.Services;

namespace Bloom
{
    public partial class MainPage : TabbedPage
    {
        private ObservableCollection<TextEvent> _textEvents;
        private Level _currentLevel;

        //private string XMLSoloDirectory = "C:\\dev\\games\\farmBelt\\LevelScripts\\Solo\\";
        //private string XMLCoopDirectory = "C:\\dev\\games\\farmBelt\\LevelScripts\\Coop\\";

        private string XMLSoloDirectory = "C:\\Users\\Admin\\Downloads\\Solo\\";
        private string XMLCoopDirectory = "C:\\Users\\Admin\\Downloads\\Coop\\";

        public ObservableCollection<string> Errors { get; set; }
        public ObservableCollection<string> SearchResults { get; set; } = new ObservableCollection<string>();


        public ObservableCollection<string> SpawnerItems { get; set; }
        public ObservableCollection<string> AliensItems { get; set; }
        public ObservableCollection<string> BuildingItems { get; set; }
        public ObservableCollection<string> PowerItems { get; set; }
        public ObservableCollection<string> WinningItems { get; set; }
        public ObservableCollection<string> WaitItems { get; set; }

        public ObservableCollection<string> ControlsItems { get; set; }
        public ObservableCollection<string> WaitResourceItems { get; set; }
        public ObservableCollection<string> WeatherItems { get; set; }

        private ObservableCollection<string> SoloErrors { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<string> CoopErrors { get; set; } = new ObservableCollection<string>();

        private ObservableCollection<FileItem> SoloFiles { get; set; }
        private ObservableCollection<FileItem> CoopFiles { get; set; }

        private ObservableCollection<TextEvent> textEvents = new ObservableCollection<TextEvent>();


        public MainPage()
        {
            InitializeComponent();

            Errors = new ObservableCollection<string>();

            // Initialize ObservableCollection
            _textEvents = new ObservableCollection<TextEvent>();
            dataGridView1.ItemsSource = _textEvents;

            // Set the BindingContext for the page
            BindingContext = this;

            AliensItems = new ObservableCollection<string>
            {
                "Yellowbird", "Pinkbird", "Assassin", "Greenpirate", "Worm", "Yellowbirdmother", "Minemother", "Mine", "Crab", "Crabmother", "Jelly", "Pulsar"
            };

            AliensList.ItemsSource = AliensItems;

            // Populate the ObservableCollection
            SpawnerItems = new ObservableCollection<string>
            {
                "Yellowbird", "Pinkbird", "Assassin", "Greenpirate"
            };

            // Bind the collection to the ListView
            listBoxSpawner.ItemsSource = SpawnerItems;

            BuildingItems = new ObservableCollection<string>
            {
                "arc", "health", "chaff", "flip", "plant"
            };

            // Bind the collection to the ListView
            listBoxBuilding.ItemsSource = BuildingItems;

            PowerItems = new ObservableCollection<string>
            {
                "health", "missile", "laser", "shield", "landcrab", "artifact1", "artifact2", "jelly"
            };

            // Bind the collection to the ListView
            listBoxPowerUp.ItemsSource = PowerItems;

            WinningItems = new ObservableCollection<string> {
            "health", "missile", "laser", "shield", "landcrab", "artifact1", "artifact2", "jelly"
            };

            listBoxWinningCondition.ItemsSource = WinningItems;


            WaitItems = new ObservableCollection<string> {
             "water", "seeds", "lava", "metal", "wood", "crystal", "firefruit", "crab", "jelly"
            };

            listBoxCarrying.ItemsSource = WinningItems;


            ControlsItems = new ObservableCollection<string> {
             "acceleratekey", "leftkey", "rightkey", "missilekey", "chaffkey", "flipkey", "laserkey", "zoomin", "zoomout", "activatekey", "acknowledgekey"
            };
            
            listBoxControls.ItemsSource = ControlsItems;

            WaitResourceItems = new ObservableCollection<string> {
             "lava", "mineral", "wood", "crystal"
            };

            listBoxResources.ItemsSource = ControlsItems;

            WeatherItems = new ObservableCollection<string> {
             "rain", "snow", "hail"
            };

            listBoxWeather.ItemsSource = WeatherItems;

            // Bind ListView to ObservableCollections
            listBoxCheckSoloErrors.ItemsSource = SoloErrors;
            listBoxCheckCoopErrors.ItemsSource = CoopErrors;

            textBoxSoloDirectory.Text = XMLSoloDirectory;
            textBoxCoopDirectory.Text = XMLCoopDirectory;

            // Initialize collections
            SoloFiles = new ObservableCollection<FileItem>();
            CoopFiles = new ObservableCollection<FileItem>();

            // Bind collections to ListView
            listViewSoloXmlFiles.ItemsSource = SoloFiles;
            listViewCoopXmlFiles.ItemsSource = CoopFiles;

            LoadFiles();



        }

        private void OnSoloDirectoryChanged(object sender, TextChangedEventArgs e)
        {
            //SoloFiles.Clear();
            LoadFiles();
        }

        private void OnCoopDirectoryChanged(object sender, TextChangedEventArgs e)
        {
            //CoopFiles.Clear();
            LoadFiles();
        }

        private void LoadFiles()
        {
            // Load files for Solo and Coop directories
            LoadFilesFromDirectory(textBoxSoloDirectory.Text, listViewSoloXmlFiles);
            LoadFilesFromDirectory(textBoxCoopDirectory.Text, listViewCoopXmlFiles);
        }

        private void LoadFilesFromDirectory(string directoryPath, ListView listView)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            {
                DisplayAlert("Error", $"Directory '{directoryPath}' does not exist.", "OK");
                return;
            }

            // Get all XML files from the directory
            var files = Directory.GetFiles(directoryPath, "*.xml");
            var fileDetails = new List<FileItem>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                fileDetails.Add(new FileItem
                {
                    FileName = fileInfo.Name,
                    FileSize = (fileInfo.Length / 1024.0).ToString("F2"), // Size in KB
                    LastModified = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            listView.ItemsSource = fileDetails;
        }

        private void OnFileSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is FileItem selectedFile)
            {
                string directory = sender == listViewSoloXmlFiles ? textBoxSoloDirectory.Text : textBoxCoopDirectory.Text;
                string fullPath = Path.Combine(directory, selectedFile.FileName);

                try
                {
                    LoadScript(fullPath); // Call your LoadScript method here
                                          // Perform your navigation or update UI accordingly
                    //DisplayAlert("File Loaded", $"Loaded file: {fullPath}", "OK");
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", $"Error loading file: {ex.Message}", "OK");
                }
            }
        }


        private Level currentLevel;

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".xml" } },
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.Android, new[] { "application/xml" } }
    });

            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a Level Script XML File",
                FileTypes = xmlFileType
            });

            if (result != null)
            {
                try
                {
                    _currentLevel = ScriptManager.LoadScript(result.FullPath);
                    currentLevel = ScriptManager.LoadScript(result.FullPath);

                    // Clear the ObservableCollection to avoid duplicates
                    _textEvents.Clear();

                    foreach (var textEvent in _currentLevel.TextEvents)
                    {
                        _textEvents.Add(new TextEvent
                        {
                            Time = textEvent.Time,
                            Text = textEvent.Text,
                            SpeechType = textEvent.SpeechType,
                            Face = textEvent.Face,
                            Character = textEvent.Character,
                            WaitForInput = textEvent.WaitForInput
                        });
                    }

                    //LabelFilename.Text = result.FullPath;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to load script: {ex.Message}", "OK");
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_currentLevel == null || !_textEvents.Any())
            {
                await DisplayAlert("Error", "No data to save. Load or create a script first.", "OK");
                return;
            }

            try
            {
                _currentLevel.TextEvents = _textEvents.ToList();
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "level_script.xml");

                ScriptManager.SaveScript(filePath, _currentLevel);

                await DisplayAlert("Success", $"Script saved to: {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save script: {ex.Message}", "OK");
            }
        }

        private void OnClearScriptClicked(object sender, EventArgs e)
        {
            _currentLevel = new Level { TextEvents = new List<TextEvent>() };
            _textEvents.Clear();
            //LabelFilename.Text = "New Script";
        }

        private void OnAddEventClicked(object sender, EventArgs e)
        {
            _textEvents.Add(new TextEvent
            {
                Time = 0,
                Text = "New Event",
                SpeechType = Bloom.Models.TextType.text,
                Face = FaceType.neutral,
                Character = CharacterType.hero,
                WaitForInput = false
            });
        }

        private void OnRemoveEventClicked(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItem is TextEvent selectedEvent)
            {
                _textEvents.Remove(selectedEvent);
            }
        }

        private void OnCreateAliensClicked(object sender, EventArgs e)
        {
            if (AliensList.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                var alienTag = "{alien:" + AliensList.SelectedItem.ToString() +
                               (radioButtonRandomAliens.IsChecked ?
                               (":random:" + textBoxAlienQuantity.Text) :
                               textBoxAlienX.Text + "," + textBoxAlienY.Text) + "}";

                // Get the selected row in the CollectionView
                var selectedRow = dataGridView1.SelectedItem as TextEvent;

                if (selectedRow != null)
                {
                    selectedRow.Text += " " + alienTag; // Append the alien tag to the existing text
                    dataGridView1.ItemsSource = null; // Refresh the CollectionView
                    dataGridView1.ItemsSource = _textEvents; // Reassign the updated list
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select an alien from the list and a row in the grid.", "OK");
            }
        }

        private void ButtonCreateSpawner_Clicked(object sender, EventArgs e)
        {
            if (listBoxSpawner.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                // Validate the location
                if (!ValidateLocation(textBoxSpawnX.Text, textBoxSpawnY.Text, out int x, out int y))
                {
                    return; // Stop if invalid
                }

                // Validate spawn time (ensure within range, if applicable)
                if (!ValidateQuantity(textBoxSpawnTime.Text, out int spawnTime))
                {
                    return; // Stop if invalid
                }

                string spawnerTag = "{spawner:" + listBoxSpawner.SelectedItem.ToString() +
                                    ":" + spawnTime + ":" + x + "," + y + "}";

                AppendTagToCurrentRow(spawnerTag);
            }
            else
            {
                DisplayAlert("Selection Error", "Please select a spawner from the list and a row in the grid.", "OK");
            }
        }

        private void AppendTagToCurrentRow(string tag)
        {
            if (dataGridView1.SelectedItem == null) return;

            var selectedItem = (TextEvent)dataGridView1.SelectedItem;

            if (!string.IsNullOrEmpty(selectedItem.Text))
            {
                selectedItem.Text += " " + tag;
            }
            else
            {
                selectedItem.Text = tag;
            }

            // Notify CollectionView about the change (needed for UI updates)
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = _textEvents;
        }

        private void buttonWaitBuilding_Click(object sender, EventArgs e)
        {
            if (listBoxBuilding.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                // Validate the building quantity
                if (!ValidateQuantity(textBoxBuildingQuantity.Text, out int buildingQuantity))
                {
                    return; // Stop if invalid
                }

                string buildingTag = "{building:" + listBoxBuilding.SelectedItem.ToString() +
                                     ":" + buildingQuantity + "}";

                AppendTagToCurrentRow(buildingTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a building from the list and a row in the grid.", "OK");
            }
        }

        private void buttonCreatePowerUp_Click(object sender, EventArgs e)
        {
            if (listBoxPowerUp.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                // Validate location for powerup
                if (!ValidateLocation(textBoxPowerUpX.Text, textBoxPowerUpY.Text, out int x, out int y))
                {
                    return; // Stop if invalid
                }

                string powerupTag = "{powerup:" + listBoxPowerUp.SelectedItem.ToString() +
                                    ":" + x + "," + y + "}";

                AppendTagToCurrentRow(powerupTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a powerup from the list and a row in the grid.", "OK");
            }
        }

        private void buttonWinningCondition_Click(object sender, EventArgs e)
        {
            if (listBoxWinningCondition.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                string winningTag = "{winning:" + listBoxWinningCondition.SelectedItem.ToString() +
                                    ":" + textBoxResourceQuantity.Text + "}";

                AppendTagToCurrentRow(winningTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a winning condition from the list and a row in the grid.", "OK");
            }
        }

        private void buttonCreateImage_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItem != null)
            {
                string imageTag = "{image:" + textBoxImageName.Text + ":" + textBoxImageIndex.Text + "}";

                AppendTagToCurrentRow(imageTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a row in the grid.", "OK");
            }
        }

        private void buttonCarrying_Click(object sender, EventArgs e)
        {
            if (listBoxCarrying.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                string carryingTag = "{carrying:" + listBoxCarrying.SelectedItem.ToString() + "}";

                AppendTagToCurrentRow(carryingTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select an item to carry from the list and a row in the grid.", "OK");
            }
        }

        private void buttonControls_Click(object sender, EventArgs e)
        {
            if (listBoxControls.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                string controlsTag = "{controls:" + listBoxControls.SelectedItem.ToString() + "}";

                AppendTagToCurrentRow(controlsTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a control from the list and a row in the grid.", "OK");
            }
        }

        private void buttonWaitResources_Click(object sender, EventArgs e)
        {
            if (listBoxResources.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                // Validate the resource quantity
                if (!ValidateQuantity(textBoxResourceQuantity.Text, out int resourceQuantity))
                {
                    return; // Stop if invalid
                }

                string resourcesTag = "{resources:" + listBoxResources.SelectedItem.ToString() +
                                      ":" + resourceQuantity + "}";

                AppendTagToCurrentRow(resourcesTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a resource from the list and a row in the grid.", "OK");
            }
        }

        private void buttonWeather_Click(object sender, EventArgs e)
        {
            if (listBoxWeather.SelectedItem != null && dataGridView1.SelectedItem != null)
            {
                string weatherTag = "{weather:" + listBoxWeather.SelectedItem.ToString() +
                                    ":" + textBoxWeather.Text + "}";

                AppendTagToCurrentRow(weatherTag);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Selection Error",
                    "Please select a weather condition from the list and a row in the grid.", "OK");
            }
        }

        private void buttonNewScript_Click(object sender, EventArgs e)
        {
            currentLevel = new Level();  // Create a new Level instance
            currentLevel.TextEvents = new List<TextEvent>(); // Ensure TextEvents is initialized

            dataGridView1.ItemsSource = null; // Clear existing data
            dataGridView1.ItemsSource = currentLevel.TextEvents; // Set new data source

            labelFilename.Text = "New Script"; // Update filename label
        }

        private async void buttonSave_Clicked(object sender, EventArgs e)
        {
            if (currentLevel == null || currentLevel.TextEvents == null)
            {
                await DisplayAlert("Save Error", "There is no script data to save. Load or create a new script first.", "OK");
                return;
            }

            // Ensure any pending edits are applied
            dataGridView1.Handler?.UpdateValue(nameof(dataGridView1.ItemsSource));

            // Refresh data binding
            if (dataGridView1.ItemsSource is ObservableCollection<TextEvent> textEvents)
            {
                currentLevel.TextEvents = textEvents.ToList();
            }
            else
            {
                currentLevel.TextEvents = new List<TextEvent>();
            }

            // 🔥 Validate the script before saving
            List<string> validationErrors = ValidateLevel(currentLevel, "Current Script");
            if (validationErrors.Any())
            {
                string errorMessage = string.Join("\n", validationErrors);
                await DisplayAlert("Validation Error", $"Validation failed. Fix these issues before saving:\n\n{errorMessage}", "OK");
                return;
            }

            // Define a save file path
            string saveFileName = $"{FileSystem.AppDataDirectory}/LevelScript.xml";

            try
            {
                // Save the script
                ScriptManager.SaveScript(saveFileName, currentLevel);

                // Update filename label
                labelFilename.Text = saveFileName;

                await DisplayAlert("Save Successful", "Script saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Save Error", $"Error saving file: {ex.Message}", "OK");
            }
        }

        private List<string> ValidateLevel(Level level, string fileName)
        {
            List<string> errors = new List<string>();

            // Validate time increments
            float previousTime = -1;
            foreach (var textEvent in level.TextEvents)
            {
                if (textEvent.Time < previousTime)
                {
                    errors.Add($"Script {fileName}: Time value {textEvent.Time} is out of order (previous: {previousTime}).");
                }
                previousTime = textEvent.Time;

                // Validate tags in the text of each event
                var tagErrors = TagValidator.ValidateTags(textEvent.Text);
                if (tagErrors.Any())
                {
                    foreach (var tagError in tagErrors)
                    {
                        errors.Add($"Script {fileName}: {tagError} in text \"{textEvent.Text}\".");
                    }
                }
            }

            return errors;
        }


        //private void OnCreateSpawnerClicked(object sender, EventArgs e)
        //{
        //    if (SpawnerList.SelectedItem != null && DataGrid.SelectedItem != null)
        //    {
        //        // Validate the location
        //        if (!ValidateLocation(SpawnXEntry.Text, SpawnYEntry.Text, out int x, out int y))
        //        {
        //            return; // Stop if invalid
        //        }

        //        // Validate spawn time
        //        if (!ValidateQuantity(SpawnTimeEntry.Text, out int spawnTime))
        //        {
        //            return; // Stop if invalid
        //        }

        //        string spawnerTag = "{spawner:" + SpawnerList.SelectedItem.ToString() +
        //                            ":" + spawnTime + ":" + x + "," + y + "}";

        //        // Get the selected row in the CollectionView
        //        var selectedRow = DataGrid.SelectedItem as TextEvent;

        //        if (selectedRow != null)
        //        {
        //            selectedRow.Text += " " + spawnerTag; // Append the spawner tag to the existing text
        //            DataGrid.ItemsSource = null; // Refresh the CollectionView
        //            DataGrid.ItemsSource = _textEvents; // Reassign the updated list
        //        }
        //    }
        //    else
        //    {
        //        Application.Current.MainPage.DisplayAlert("Selection Error",
        //            "Please select a spawner from the list and a row in the grid.", "OK");
        //    }
        //}

        // Helper methods
        private bool ValidateLocation(string xInput, string yInput, out int x, out int y)
        {
            x = 0;
            y = 0;

            if (!int.TryParse(xInput, out x) || !int.TryParse(yInput, out y))
            {
                Application.Current.MainPage.DisplayAlert("Input Error",
                    "Please enter valid X and Y coordinates.", "OK");
                return false;
            }
            return true;
        }

        private bool ValidateQuantity(string input, out int quantity)
        {
            quantity = 0;

            if (!int.TryParse(input, out quantity) || quantity <= 0)
            {
                Application.Current.MainPage.DisplayAlert("Input Error",
                    "Please enter a valid number greater than 0.", "OK");
                return false;
            }
            return true;
        }

        private void buttonCheckScripts_Clicked(object sender, EventArgs e)
        {
            listBoxCheckSoloErrors.ItemsSource = null;  // Clear previous items
            listBoxCheckCoopErrors.ItemsSource = null;

            // Perform script checks
            CheckScripts(XMLSoloDirectory, listViewSoloXmlFiles, SoloErrors);
            CheckScripts(XMLCoopDirectory, listViewCoopXmlFiles, CoopErrors);

            listBoxCheckSoloErrors.ItemsSource = null;
            listBoxCheckSoloErrors.ItemsSource = SoloErrors;

            listBoxCheckCoopErrors.ItemsSource = null;
            listBoxCheckCoopErrors.ItemsSource = CoopErrors;

        }

        private void CheckScripts(string directory, ListView listView, ObservableCollection<string> listErrors)
        {
            listErrors.Clear();  // Clear previous errors
            listErrors.Add($"Checking scripts in directory: {directory}");

            int errorCount = 0;

            // Ensure ItemsSource is not null and is a valid collection
            var scriptFiles = listView.ItemsSource as IEnumerable<FileItem>; // FileModel is the assumed model for items
            if (scriptFiles == null || !scriptFiles.Any())
            {
                listErrors.Add("Error: No scripts found in " + directory);
                return;
            }

            foreach (var file in scriptFiles) // Iterate through the bound collection
            {
                string fileName = file.FileName; // Assume FileModel has a FileName property
                string filePath = Path.Combine(directory, fileName);

                try
                {
                    // Load the script
                    Level level = ScriptManager.LoadScript(filePath);

                    // Validate the loaded script
                    var errors = ValidateLevel(level, fileName);

                    if (errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            listErrors.Add(error);
                        }
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    listErrors.Add($"Error loading script {fileName}: {ex.Message}");
                }
            }

            // If no errors were found, add a success message
            if (errorCount == 0)
            {
                listErrors.Add("All scripts passed validation!");
            }
        }

        private async void OnPageAppearing(object sender, EventArgs e)
        {
            // Check if there's a selected script in Solo or Coop ListView
            if (listViewSoloXmlFiles.SelectedItem is string soloFile)
            {
                await LoadSelectedScript(soloFile, XMLSoloDirectory);
            }
            else if (listViewCoopXmlFiles.SelectedItem is string coopFile)
            {
                await LoadSelectedScript(coopFile, XMLCoopDirectory);
            }
        }

        private async void listViewXmlFiles_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is string selectedFile)
            {
                string directory = (sender == listViewSoloXmlFiles) ? XMLSoloDirectory : XMLCoopDirectory;
                await LoadSelectedScript(selectedFile, directory);
            }
        }

        private async Task LoadSelectedScript(string fileName, string directory)
        {
            string fullPath = Path.Combine(directory, fileName);

            try
            {
                // Ensure LoadScript is properly implemented
                await LoadScript(fullPath);

                // Set the filename label
                labelFilename.Text = fullPath;

                // 🔥 Navigate to "Loaded Script" tab
                await Shell.Current.GoToAsync("//LoadedScriptPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading file: {ex.Message}", "OK");
            }
        }

        private async Task LoadScript(string filePath)
        {
            // Simulate loading asynchronously (replace with actual async logic if available)
            await Task.Run(() =>
            {
                currentLevel = ScriptManager.LoadScript(filePath);
            });

            _textEvents.Clear();

            foreach (var textEvent in currentLevel.TextEvents)
            {
                _textEvents.Add(new TextEvent
                {
                    Time = textEvent.Time,
                    Text = textEvent.Text,
                    SpeechType = textEvent.SpeechType,
                    Face = textEvent.Face,
                    Character = textEvent.Character,
                    WaitForInput = textEvent.WaitForInput
                });
            }

            // Update directory paths
            textBoxSoloDirectory.Text = XMLSoloDirectory;
            textBoxCoopDirectory.Text = XMLCoopDirectory;

            this.CurrentPage = this.Children[0]; // Set the first tab as the active one

        }

        private void buttonFindWord_Clicked(object sender, EventArgs e)
        {
            // Ensure search text is not null or empty
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a word to search for.", "OK");
                return;
            }

            // Call CheckScriptsForWord for both Solo and Coop directories
            CheckScriptsForWord(textBoxSearch.Text.Trim(), XMLSoloDirectory, SoloErrors);
            CheckScriptsForWord(textBoxSearch.Text.Trim(), XMLCoopDirectory, CoopErrors);
        }

        private void CheckScriptsForWord(string word, string directory, ObservableCollection<string> listboxItems)
        {
            listboxItems.Clear(); // Clear previous results

            if (string.IsNullOrWhiteSpace(word))
            {
                Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a word to search for.", "OK");
                return;
            }

            try
            {
                // Get all .xml files in the directory
                string[] xmlFiles = Directory.GetFiles(directory, "*.xml");

                foreach (string filePath in xmlFiles)
                {
                    string fileName = Path.GetFileName(filePath);

                    try
                    {
                        // Load the script
                        Level level = ScriptManager.LoadScript(filePath);

                        // Find matching events
                        var matchingEvents = level.TextEvents
                            .Where(te => te.Text.Contains(word, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (matchingEvents.Any())
                        {
                            listboxItems.Add($"Script {fileName}: Found \"{word}\" in {matchingEvents.Count} events.");
                            foreach (var textEvent in matchingEvents)
                            {
                                listboxItems.Add($"  Time: {textEvent.Time}, Text: \"{textEvent.Text}\"");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        listboxItems.Add($"Error loading script {fileName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Error reading directory: {ex.Message}", "OK");
            }
        }




    }
}

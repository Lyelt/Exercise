using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ExerciseProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _muscleGroupFile = "MuscleGroups.xml";
        private string _exerciseFile = "Exercises.xml";

        private string _searchFilter;
        private WeightType? _wtFilter;
        private ExerciseType? _etFilter;
        private List<MuscleGroup> _mgFilter = new List<MuscleGroup>();

        private List<ExerciseItem> _filtered;

        public List<MuscleGroup> MuscleGroups;
        public List<ExerciseItem> Exercises;
        public List<ExerciseItem> FilteredExercises
        {
            get { return _filtered; }
            set
            {
                _filtered = value;
                exerciseListBox.ItemsSource = _filtered;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MuscleGroups = GetMuscleGroups();
            Exercises = GetExercises();

            muscleGroupListBox.ItemsSource = MuscleGroups;
            muscleGroupListBox.SelectionMode = SelectionMode.Multiple;

            exerciseListBox.ItemsSource = Exercises;
            weightTypeComboBox.ItemsSource = Enum.GetValues(typeof(WeightType));
            exerciseTypeComboBox.ItemsSource = Enum.GetValues(typeof(ExerciseType));
        }

        public List<MuscleGroup> GetMuscleGroups()
        {
            var groups = new List<MuscleGroup>();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(_muscleGroupFile);
            }
            catch { /* Error loading xml document */ }

            var nodes = xmlDoc.GetElementsByTagName("group");
            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes?["name"]?.Value;
                string longname = node.Attributes?["fullname"]?.Value;
                string loc = node.Attributes?["location"]?.Value;
                string bodypart = node.Attributes?["bodypart"]?.Value;
                string desc = node.Attributes?["description"]?.Value;
                groups.Add(new MuscleGroup(name, longname, loc, bodypart, desc));
            }

            return groups;
        }

        public List<ExerciseItem> GetExercises()
        {
            var exercises = new List<ExerciseItem>();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(_exerciseFile);
            }
            catch {  }

            var exerciseNodes = xmlDoc.GetElementsByTagName("exercise");
            foreach (XmlNode node in exerciseNodes)
            {
                string name = node.Attributes?["name"]?.Value;
                string type = node.ParentNode.Name;
                string weightType;

                if (node.Attributes["weight"] != null)
                    weightType = node.Attributes["weight"].Value;
                else
                {
                    switch (type.ToLower())
                    {
                        case "strength":
                            weightType = "Barbell";
                            break;
                        case "flexibility":
                            weightType = "Bodyweight";
                            break;
                        case "aerobic":
                            weightType = "Bodyweight";
                            break;
                        default:
                            weightType = "Unknown";
                            break;
                    }
                }

                List<MuscleGroup> groups = new List<MuscleGroup>();

                // List of muscle groups
                var groupNodes = node.SelectNodes("mg");
                foreach (XmlNode mg in groupNodes)
                {
                    // Take the first matching muscle group
                    MuscleGroup addMg = MuscleGroups.FirstOrDefault(m => m.Name.ToLower() == mg.InnerText.ToLower());
                    groups.Add(addMg);
                }

                exercises.Add(new ExerciseItem(name, groups, type, weightType));
            }

            return exercises;
        }

        private List<ExerciseItem> GetFilteredExercises()
        {
            var filteredExercises = new List<ExerciseItem>();
            try
            {
                foreach (var ex in Exercises)
                {
                    bool match = true;

                    // filter by search term
                    if (!string.IsNullOrEmpty(_searchFilter))
                        if (!ex.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase))
                            match = false;

                    // filter by weight type
                    if (_wtFilter != null)
                        if (!(ex.Weight == _wtFilter))
                            match = false;

                    // filter by exercise type
                    if (_etFilter != null)
                        if (!(ex.Type == _etFilter))
                            match = false;

                    // filter by selected muscle groups
                    if (_mgFilter.Count > 0)
                    {
                        bool muscleMatch = false;
                        foreach (var mg in _mgFilter)
                        {
                            if (ex.Muscles.Any(m => m.Name == mg.Name))
                                muscleMatch = true;
                        }
                        if (!muscleMatch) match = false;
                    }

                    if (match) filteredExercises.Add(ex);
                }
            }
            catch (Exception) {/* still constructing the form */ }

            return filteredExercises;
        }

        #region Filter Handlers
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchFilter = searchBox.Text;
            FilteredExercises = GetFilteredExercises();
        }

        private void weightTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (weightTypeComboBox.SelectedIndex > -1)
            {
                WeightType wt;
                Enum.TryParse(weightTypeComboBox.SelectedItem.ToString(), out wt);
                _wtFilter = wt;
                FilteredExercises = GetFilteredExercises();
            }
        }

        private void exerciseTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (exerciseTypeComboBox.SelectedIndex > -1)
            {
                ExerciseType et;
                Enum.TryParse(exerciseTypeComboBox.SelectedItem.ToString(), out et);
                _etFilter = et;
                FilteredExercises = GetFilteredExercises();
            }
        }

        private void muscleGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mgFilter.Clear();
            foreach (var mg in muscleGroupListBox.SelectedItems)
            {
                _mgFilter.Add((MuscleGroup)mg);
            }

            FilteredExercises = GetFilteredExercises();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            _mgFilter.Clear();
            _etFilter = null;
            _wtFilter = null;
            _searchFilter = string.Empty;

            muscleGroupListBox.UnselectAll();
            weightTypeComboBox.SelectedIndex = -1;
            exerciseTypeComboBox.SelectedIndex = -1;
            searchBox.Clear();

            FilteredExercises = GetFilteredExercises();
        }
        #endregion
    }
}

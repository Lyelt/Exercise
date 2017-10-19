using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private const string CONNSTRING = "Data Source=NICK\\SQLEXPRESS;Initial Catalog=Exercises;Integrated Security=True";
        private const string EXERCISEQUERY = "SELECT * FROM ExerciseItems";
        private const string ALLMGQUERY = "SELECT * FROM MuscleGroups";
        private const string EXMGQUERY = "SELECT * FROM ExerciseMG WHERE ExerciseID = @exerciseID";
        private const string MUSCLEQUERY = "SELECT * FROM MuscleGroups WHERE ID = @musclegroupID";
        //private string _muscleGroupFile = "MuscleGroups.xml";
        //private string _exerciseFile = "Exercises.xml";

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

        public List<ExerciseItem> GetExercises()
        {
            var exercises = new List<ExerciseItem>();
            using (SqlConnection conn = new SqlConnection(CONNSTRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(EXERCISEQUERY, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                { 
                    while (reader.Read())
                    {
                        int id = int.Parse(reader["ID"].ToString());
                        string name = reader["Name"].ToString();
                        string type = reader["Type"].ToString();
                        List<MuscleGroup> groups = GetMuscleGroups(id);
                        List<string> weightTypes = new List<string>(reader["WeightType"].ToString().Split('/'));
                        string desc = reader["Description"].ToString();
                        string imgSrc = reader["Image"].ToString();
                        string lnk = reader["Link"].ToString();

                        exercises.Add(new ExerciseItem(id, name, groups, type, weightTypes, desc, imgSrc, lnk));
                    }
                }
            }
           
            return exercises;
        }

        /// <summary>
        /// Gets list of all the muscle groups specified by the exercise ID
        /// </summary>
        /// <param name="ExerciseID">ID of the exercise to get the muscle groups for. -1 to get all muscle groups.</param>
        /// <returns>List of muscle groups specified by exercise (or all)</returns>
        private List<MuscleGroup> GetMuscleGroups(int ExerciseID = -1)
        {
            var groups = new List<MuscleGroup>();
            using (SqlConnection conn = new SqlConnection(CONNSTRING))
            {
                conn.Open();
                if (ExerciseID > -1)
                {
                    using (SqlCommand cmd = new SqlCommand(EXMGQUERY, conn))
                    {
                        cmd.Parameters.AddWithValue("@exerciseID", ExerciseID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                groups.Add(GetMuscleGroup(int.Parse(reader["MuscleGroupID"].ToString())));
                            }
                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(ALLMGQUERY, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            groups.Add(GetMuscleGroup(int.Parse(reader["ID"].ToString())));
                        }
                    }
                    
                }
            }

            return groups;
        }

        private MuscleGroup GetMuscleGroup(int mgID)
        {
            MuscleGroup muscleGroup = null;
            using (SqlConnection conn = new SqlConnection(CONNSTRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(MUSCLEQUERY, conn))
                {
                    cmd.Parameters.AddWithValue("@musclegroupID", mgID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["ID"].ToString());
                            string name = reader["Name"].ToString();
                            string fullName = reader["FullName"].ToString();
                            string bodyPart = reader["BodyPart"].ToString();
                            muscleGroup = new MuscleGroup(id, name, fullName, bodyPart);
                        }
                    }
                }
            }
            return muscleGroup;
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

                    // filter by exercise type
                    if (_etFilter != null)
                        if (!(ex.Type == _etFilter))
                            match = false;

                    // filter by weight type
                    if (_wtFilter != null)
                        if (!(ex.WeightTypes.Any(w=>w == _wtFilter)))
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

        private void exerciseListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExerciseExpansion newWindow = new ExerciseExpansion((ExerciseItem)exerciseListBox.SelectedItem);
            newWindow.Show();
        }
    }
}

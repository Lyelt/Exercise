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

        public List<MuscleGroup> MuscleGroups;
        public List<ExerciseItem> Exercises;

        public MainWindow()
        {
            InitializeComponent();
            MuscleGroups = GetMuscleGroups();
            Exercises = GetExercises();

            muscleGroupListBox.ItemsSource = MuscleGroups;
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
    }
}

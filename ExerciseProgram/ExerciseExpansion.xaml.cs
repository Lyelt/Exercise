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
using System.Windows.Shapes;

namespace ExerciseProgram
{
    /// <summary>
    /// Interaction logic for ExerciseExpansion.xaml
    /// </summary>
    public partial class ExerciseExpansion : Window
    {
        public ExerciseItem ExItem { get; set; }
        public ExerciseExpansion(ExerciseItem exItem)
        {
            ExItem = exItem;
            InitializeComponent();
            BuildExerciseWindow();
        }

        public void BuildExerciseWindow()
        {
            this.Title = ExItem.Name;
            etLabel.Content = ExItem.Type.ToString();
            wtLabel.Content = ExItem.Weight.ToString();
            descLabel.Content = ExItem.Description;
            foreach (var mg in ExItem.Muscles)
            {
                mgLabel.Content += mg.Name + Environment.NewLine;
            }
        }
    }
}

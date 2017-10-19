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
using Google.Apis.Customsearch.v1;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Navigation;

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
            // Set the title and label content
            this.Title = ExItem.Name;
            etLabel.Content = ExItem.Type.ToString();
            descLabel.Text = string.IsNullOrEmpty(ExItem.Description) ? "No description provided" : ExItem.Description;

            // URL for learning more about the exercise
            if (!string.IsNullOrEmpty(ExItem.Link))
            {
                hyperlinkText.Text = ExItem.Name + ": Click to learn more. ";
                hyperlink.NavigateUri = new Uri(ExItem.Link);
            }    
            
            // Image if provided
            if (!string.IsNullOrEmpty(ExItem.ImageSource))
                exerciseImage.Source = new BitmapImage(new Uri(ExItem.ImageSource));

            // Muscle groups and weight types
            foreach (var mg in ExItem.Muscles)
                mgLabel.Content += mg.Name + Environment.NewLine;
            
            foreach (var wt in ExItem.WeightTypes)
                wtLabel.Content += wt.ToString() + Environment.NewLine;
        }

        private void Hyperlink_Click(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

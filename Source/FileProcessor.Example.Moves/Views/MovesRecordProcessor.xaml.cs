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

namespace FileProcessor.Example.Moves.Views
{
    using System.Reflection;
    using Microsoft.Win32;
    using ViewModels;

    /// <summary>
    /// Interaction logic for MovesRecordProcessor.xaml
    /// </summary>
    public partial class MovesRecordProcessor : Window
    {
        public MovesRecordProcessor()
        {
            InitializeComponent();
        }

        private void LoadFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
                                 {
                                     InitialDirectory = Assembly.GetEntryAssembly().Location,
                                     Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                                 };

            if (openFileDialog.ShowDialog() == true)
            {
                var viewModel = LayoutGrid.DataContext as MovesRecordProcessorViewModel;

                viewModel?.LoadFile(openFileDialog.FileName);
            }
        }

        private void WriteFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
                                 {
                                     InitialDirectory = Assembly.GetEntryAssembly().Location,
                                     Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                                     CheckFileExists = false
                                 };

            if (openFileDialog.ShowDialog() == true)
            {
                var viewModel = LayoutGrid.DataContext as MovesRecordProcessorViewModel;

                viewModel?.WriteFile(openFileDialog.FileName);
            }
        }
    }
}

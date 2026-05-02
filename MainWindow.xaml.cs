// <copyright file="MainWindow.xaml.cs" company="LocalAIWorkspace">
// Copyright (c) LocalAIWorkspace. All rights reserved.
// </copyright>

namespace LocalAIWorkspace
{
    using System.Windows;
    using LocalAIWorkspace.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private MainViewModel VM => (MainViewModel)this.DataContext;

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.Description = "Select a project folder";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.VM.LoadFolder(dialog.SelectedPath);
            }
        }

        private async void Explain_Click(object sender, RoutedEventArgs e)
        {
            await this.VM.Explain();
        }

        /// <summary>
        /// Selects Folder B.
        /// </summary>
        private void SelectFolderB_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (this.DataContext is MainViewModel vm)
                {
                    vm.LoadFolderB(dialog.SelectedPath);
                }
            }
        }

        /// <summary>
        /// Swaps Version A and Version B.
        /// </summary>
        private void Swap_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                vm.SwapVersions();
            }
        }

        private async void Compare_Click(object sender, RoutedEventArgs e)
        {
            await this.VM.Compare();
        }
    }
}
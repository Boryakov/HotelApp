using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using HotelApp.Core;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class MainWindow : Window
    {
        private Hotel hotelEngine;
        private int currentFilterMode = 0; // 0 - Display All, 1 - Vacant Only, 2 - Occupied Only

        public MainWindow()
        {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            // Core logic administration operations
            btnInitialize.Click += BtnInitialize_Click;
            btnGenerateData.Click += BtnGenerateData_Click;
            btnOpenCheckIn.Click += BtnOpenCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;

            // System file persistence configurations
            menuOpen.Click += MenuOpen_Click;
            menuSave.Click += MenuSave_Click;
            menuExit.Click += (s, e) => Application.Current.Shutdown();

            // Menu reporting layout filter events
            menuShowAll.Click += (s, e) => UpdateFilterMode(0);
            menuShowVacant.Click += (s, e) => UpdateFilterMode(1);
            menuShowOccupied.Click += (s, e) => UpdateFilterMode(2);
        }

        private void RefreshConsoleDisplay()
        {
            if (hotelEngine == null)
            {
                txtConsoleMonitor.Text = "System idle. Please initialize the hotel fund database or import a configuration.";
                return;
            }
            txtConsoleMonitor.Text = hotelEngine.PrintReport(currentFilterMode);
        }

        private void UpdateFilterMode(int mode)
        {
            currentFilterMode = mode;
            menuShowAll.IsChecked = (mode == 0);
            menuShowVacant.IsChecked = (mode == 1);
            menuShowOccupied.IsChecked = (mode == 2);
            RefreshConsoleDisplay();
        }

        private void BtnInitialize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtTotalRooms.Text, out int roomsCount) || roomsCount <= 0)
                {
                    throw new HotelException("Configuration failed: Total number of rooms must be a valid positive integer.");
                }

                hotelEngine = new Hotel(roomsCount);
                RefreshConsoleDisplay();
                MessageBox.Show($"Hotel database initialized successfully with {roomsCount} clean rooms.", "System Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (HotelException ex)
            {
                MessageBox.Show(ex.Message, "Runtime Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateData_Click(object sender, RoutedEventArgs e)
        {
            if (hotelEngine == null)
            {
                MessageBox.Show("Please initialize the hotel fund before generating mock records.", "Action Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string[] mockNames = { "John Doe", "Alex Smith", "Emma Watson", "Anton Vance", "Michael Jordan", "Sophia Loren" };
            Random rand = new Random();
            int successCount = 0;

            foreach (RoomClass roomClass in Enum.GetValues(typeof(RoomClass)))
            {
                int instancesToFill = rand.Next(1, 3);
                for (int i = 0; i < instancesToFill; i++)
                {
                    try
                    {
                        string randomName = mockNames[rand.Next(mockNames.Length)];
                        hotelEngine.CheckInClient(roomClass, randomName);
                        successCount++;
                    }
                    catch (HotelException) { /* Quietly skip if a specific class capacity is exceeded */ }
                }
            }

            RefreshConsoleDisplay();
            MessageBox.Show($"Mock execution completed. Registered {successCount} random occupants.", "System Data Generator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnOpenCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (hotelEngine == null)
            {
                MessageBox.Show("Please initialize the hotel fund database prior to executing operations.", "Action Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CheckInWindow dialog = new CheckInWindow();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    hotelEngine.CheckInClient(dialog.SelectedRoomClass, dialog.SelectedGuestName);
                    RefreshConsoleDisplay();
                    MessageBox.Show($"Guest [{dialog.SelectedGuestName}] successfully checked into a {dialog.SelectedRoomClass} room.", "Operation Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (HotelException ex)
                {
                    MessageBox.Show(ex.Message, "Business Logic Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (hotelEngine == null)
            {
                MessageBox.Show("Please initialize the hotel fund database prior to executing operations.", "Action Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(txtRoomToVacate.Text, out int roomNum))
                {
                    throw new HotelException("Operation processing failed: Target room identifier must be a numeric integer.");
                }

                hotelEngine.CheckOutClient(roomNum);
                txtRoomToVacate.Clear();
                RefreshConsoleDisplay();
                MessageBox.Show($"Room {roomNum} successfully cleared and flagged as vacant.", "Operation Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (HotelException ex)
            {
                MessageBox.Show(ex.Message, "Business Logic Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            if (hotelEngine == null)
            {
                MessageBox.Show("There is no active matrix grid configuration model to backup.", "Export Aborted", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.Title = "Save Current Hotel Status Log Report";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    string layoutReportText = hotelEngine.PrintReport(0); // System saves full state metrics layout logs
                    File.WriteAllText(saveDialog.FileName, layoutReportText);
                    MessageBox.Show("Hotel fund state matrix log compiled and saved successfully.", "System Backup Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"File system streaming error: {ex.Message}", "IO Exception Signal", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openDialog.Title = "Import Existing Hotel Database State Log File";

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    string loggingTextStream = File.ReadAllText(openDialog.FileName);
                    hotelEngine = Hotel.FromTextFile(loggingTextStream);

                    UpdateFilterMode(0); // Standardize system display grid layout matrix back to full report view
                    MessageBox.Show("Hotel database allocation recovered and synchronized from file source logs.", "Data Integration Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Parsing process sequence crash: {ex.Message}", "Deserialization Integrity Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
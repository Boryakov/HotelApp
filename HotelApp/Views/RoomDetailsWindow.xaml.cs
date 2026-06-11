using System;
using System.Windows;
using HotelApp.Models;

namespace HotelApp.Views
{
    public partial class RoomDetailsWindow : Window
    {
        // Operational outcome track state fields for data transition pipeline
        public RoomClass UpdatedClass { get; private set; }
        public bool UpdatedIsOccupied { get; private set; }
        public string UpdatedGuestName { get; private set; }
        public bool IsDeleteRequested { get; private set; }

        public RoomDetailsWindow(Room targetRoom)
        {
            InitializeComponent();

            // Core mapping layout of the room class enum constants to data visualization matrix
            cbRoomClass.ItemsSource = Enum.GetValues(typeof(RoomClass));

            // Populate current system state logs straight into structured visual items
            lblRoomHeader.Text = $"Room Record Management: Room {targetRoom.Number}";
            cbRoomClass.SelectedItem = targetRoom.Class;
            chbIsOccupied.IsChecked = targetRoom.IsOccupied;
            txtGuestName.Text = targetRoom.GuestName;

            ToggleGuestInputPanel(targetRoom.IsOccupied);
        }

        private void ChbIsOccupied_Changed(object sender, RoutedEventArgs e)
        {
            if (chbIsOccupied == null || panelGuest == null) return;
            ToggleGuestInputPanel(chbIsOccupied.IsChecked == true);
        }

        private void ToggleGuestInputPanel(bool isOccupied)
        {
            panelGuest.IsEnabled = isOccupied;
            if (!isOccupied)
            {
                txtGuestName.Clear();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Safety context data validation sequence constraint rules
            if (chbIsOccupied.IsChecked == true && string.IsNullOrWhiteSpace(txtGuestName.Text))
            {
                MessageBox.Show("Validation failed: Active occupied room entity requires valid resident name credentials.",
                                "Data Processing Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lock and cache active structural metric references before exit state
            UpdatedClass = (RoomClass)cbRoomClass.SelectedItem;
            UpdatedIsOccupied = chbIsOccupied.IsChecked == true;
            UpdatedGuestName = txtGuestName.Text.Trim();
            IsDeleteRequested = false;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var userAffirmation = MessageBox.Show("Are you completely sure you want to permanently delete and wipe this room entity from the database index?",
                                                 "Confirm Entity Destruction", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (userAffirmation == MessageBoxResult.OK)
            {
                IsDeleteRequested = true;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
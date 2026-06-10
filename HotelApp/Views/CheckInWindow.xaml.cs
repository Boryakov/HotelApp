using System;
using System.Windows;
using System.Windows.Controls;
using HotelApp.Models;
using HotelApp.Core;

namespace HotelApp.Views
{
    public partial class CheckInWindow : Window
    {
        // Properties to pass results back to MainWindow after closing
        public string SelectedGuestName { get; private set; }
        public RoomClass SelectedRoomClass { get; private set; }

        public CheckInWindow()
        {
            InitializeComponent();

            // Wire up interface control elements
            btnRegister.Click += BtnRegister_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputName = txtGuestName.Text;

                if (string.IsNullOrWhiteSpace(inputName))
                {
                    throw new HotelException("Registration rejected: The guest name field cannot be empty.");
                }

                SelectedGuestName = inputName.Trim();

                // Map selected ComboBoxItem back to our core RoomClass configuration enum
                var selectedItem = (ComboBoxItem)cmbRoomClass.SelectedItem;
                string classString = selectedItem.Content.ToString();
                SelectedRoomClass = (RoomClass)Enum.Parse(typeof(RoomClass), classString);

                // Successfully verified dialog input data parameters
                this.DialogResult = true;
                this.Close();
            }
            catch (HotelException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
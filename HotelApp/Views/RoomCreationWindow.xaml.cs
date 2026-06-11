using System;
using System.Windows;
using System.Windows.Controls;
using HotelApp.Models;
using HotelApp.Core;

namespace HotelApp.Views
{
    public partial class RoomCreationWindow : Window
    {
        public int CreatedRoomNumber { get; private set; }
        public RoomClass CreatedRoomClass { get; private set; }

        public RoomCreationWindow()
        {
            InitializeComponent();
            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = false; this.Close(); };
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtRoomNumber.Text, out int roomNum) || roomNum <= 0)
                {
                    throw new HotelException("Creation failed: Room number must be a valid positive integer.");
                }

                CreatedRoomNumber = roomNum;

                var selectedItem = (ComboBoxItem)cmbRoomClass.SelectedItem;
                CreatedRoomClass = (RoomClass)Enum.Parse(typeof(RoomClass), selectedItem.Content.ToString());

                this.DialogResult = true;
                this.Close();
            }
            catch (HotelException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
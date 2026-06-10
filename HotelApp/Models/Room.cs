using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace HotelApp.Models
{
    // Represents a single hotel room entity
    public class Room
    {
        private int number;
        private RoomClass roomClass;
        private bool isOccupied;
        private string guestName;

        // Public property for room number identification
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        // Public property for the comfort category of the room
        public RoomClass Class
        {
            get { return roomClass; }
            set { roomClass = value; }
        }

        // Public property indicating occupancy status
        public bool IsOccupied
        {
            get { return isOccupied; }
            set { isOccupied = value; }
        }

        // Public property storing the current resident's full name
        public string GuestName
        {
            get { return guestName; }
            set { guestName = value; }
        }

        // Default constructor initializing an empty standard room
        public Room()
        {
            number = 0;
            roomClass = RoomClass.Standard;
            isOccupied = false;
            guestName = string.Empty;
        }

        // Parameterized constructor to instantiate a specific room configuration
        public Room(int number, RoomClass roomClass)
        {
            this.number = number;
            this.roomClass = roomClass;
            this.isOccupied = false;
            this.guestName = string.Empty;
        }

        // Copy constructor for safe object duplication
        public Room(Room other)
        {
            if (other != null)
            {
                this.number = other.number;
                this.roomClass = other.roomClass;
                this.isOccupied = other.isOccupied;
                this.guestName = other.guestName;
            }
        }
    }
}
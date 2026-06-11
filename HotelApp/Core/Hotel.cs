using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using HotelApp.Models;

namespace HotelApp.Core
{
    // Core controller class managing the collection of rooms and main operations
    public class Hotel
    {
        private List<Room> rooms;

        // Encapsulated list of rooms within the hotel inventory
        public List<Room> Rooms
        {
            get { return rooms; }
            set { rooms = value; }
        }

        // Default constructor initializing an empty room roster
        public Hotel()
        {
            rooms = new List<Room>();
        }

        // Parameterized constructor creating a predefined set of rooms
        public Hotel(int totalRooms)
        {
            rooms = new List<Room>();

            for (int i = 0; i < totalRooms; i++)
            {
                // All rooms are strictly instantiated as RoomClass.Standard by default
                rooms.Add(new Room(101 + i, RoomClass.Standard));
            }
        }

        // Registers a unique custom room configuration to the existing database fund
        public void AddCustomRoom(int number, RoomClass roomClass)
        {
            // Validate identifier collision within the master list collection
            if (rooms.Exists(r => r.Number == number))
            {
                throw new HotelException($"Configuration constraint: Room number {number} already exists in the database fund.");
            }

            // Append the new customized room instance to the collection
            rooms.Add(new Room(number, roomClass));
        }

        public string ToJsonString()
        {
            var executionOptions = new JsonSerializerOptions
            {
                WriteIndented = true // Formats the file layout beautifully for human reading
            };
            return JsonSerializer.Serialize(this.rooms, executionOptions);
        }

        // CORE UPGRADE: Parses structured JSON back into a valid operational Hotel collection entity
        public static Hotel FromJsonString(string jsonContent)
        {
            try
            {
                var processingOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Protects against minor case mismatches
                };

                List<Room> extractedRooms = JsonSerializer.Deserialize<List<Room>>(jsonContent, processingOptions);

                if (extractedRooms == null || extractedRooms.Count == 0)
                {
                    throw new HotelException("Import aborted: Provided file does not contain valid structured matrix layout.");
                }

                // Create a temporary instance to restore application state metrics
                Hotel fullyLoadedHotel = new Hotel();
                fullyLoadedHotel.Rooms = extractedRooms;

                return fullyLoadedHotel;
            }
            catch (Exception ex) when (!(ex is HotelException))
            {
                throw new HotelException($"Deserialization integrity failure: Processing sequence broke down. Details: {ex.Message}");
            }
        }

        // Assigns a client to the first available room matching the requested category
        public void CheckInClient(RoomClass requestedClass, string guestName)
        {
            if (string.IsNullOrWhiteSpace(guestName))
            {
                throw new HotelException("Guest name processing failed: The input cannot be empty or consist only of white spaces.");
            }

            // Using standard List.Find method to locate a suitable empty room
            Room availableRoom = rooms.Find(r => r.Class == requestedClass && !r.IsOccupied);

            if (availableRoom == null)
            {
                throw new HotelException($"Registration error: No vacant rooms available for the requested category [{requestedClass}].");
            }

            availableRoom.IsOccupied = true;
            availableRoom.GuestName = guestName;
        }

        // Vacates a room using its specific unique numerical identifier
        public void CheckOutClient(int roomNumber)
        {
            Room targetRoom = rooms.Find(r => r.Number == roomNumber);

            if (targetRoom == null)
            {
                throw new HotelException($"Operation failed: Room number {roomNumber} does not exist in the hotel database.");
            }

            if (!targetRoom.IsOccupied)
            {
                throw new HotelException($"Operation failed: Room number {roomNumber} is already vacant.");
            }

            // Resetting room operational parameters to default vacant state
            targetRoom.IsOccupied = false;
            targetRoom.GuestName = string.Empty;
        }

        // Formats and builds a comprehensive tabular report of the room statuses based on view filters
        // Mode options: 0 - Display All Rooms, 1 - Display Vacant Only, 2 - Display Occupied Only
        public string PrintReport(int displayMode = 0)
        {
            StringBuilder reportBuilder = new StringBuilder();
            reportBuilder.AppendLine("===============================================================================");
            reportBuilder.AppendLine("   Room Num   |      Comfort Class      |     Status     |      Current Guest    ");
            reportBuilder.AppendLine("===============================================================================");

            int displayedCount = 0;

            foreach (var room in rooms)
            {
                // Filter handling matching the current UI selected view mode
                if (displayMode == 1 && room.IsOccupied) continue;
                if (displayMode == 2 && !room.IsOccupied) continue;

                string statusText = room.IsOccupied ? "Occupied" : "Vacant";
                string residentName = room.IsOccupied ? room.GuestName : "-";

                reportBuilder.AppendLine($"{room.Number,12}   | {room.Class,-23} | {statusText,-14} | {residentName}");
                displayedCount++;
            }

            reportBuilder.AppendLine("===============================================================================");
            reportBuilder.AppendLine($"Total records listed based on current view criteria: {displayedCount}");

            return reportBuilder.ToString();
        }

        // Class destructor releasing the inner list memory block
        ~Hotel()
        {
            if (rooms != null)
            {
                rooms.Clear();
            }
        }
    }
}
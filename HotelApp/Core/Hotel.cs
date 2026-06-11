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
        }

        // Default constructor initializing an empty room roster
        public Hotel()
        {
            rooms = new List<Room>();
        }

        // Parameterized constructor creating a predefined set of rooms with randomized classes
        public Hotel(int totalRooms)
        {
            rooms = new List<Room>();

            for (int i = 1; i <= totalRooms; i++)
            {
                // All rooms are strictly instantiated as RoomClass.Standard by default
                rooms.Add(new Room(101 + i, RoomClass.Standard));
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

        // Static parser engine to reconstitute a Hotel data model object from a structured log text file
        public static Hotel FromTextFile(string fileContent)
        {
            string[] textLines = fileContent.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (textLines.Length < 4)
            {
                throw new HotelException("Parsing error: The source file is corrupted or has an invalid template structure.");
            }

            Hotel parsedHotel = new Hotel();
            bool isDataSectionReached = false;

            foreach (var line in textLines)
            {
                if (line.Contains("Room Num"))
                {
                    isDataSectionReached = true;
                    continue;
                }

                // End processing if boundary indicator or final footer summaries are reached
                if (line.Contains("====") && isDataSectionReached && parsedHotel.Rooms.Count > 0)
                {
                    break;
                }

                if (isDataSectionReached && line.Contains("|"))
                {
                    string[] splitDataParts = line.Split('|');
                    if (splitDataParts.Length < 4) continue;

                    int number = Convert.ToInt32(splitDataParts[0].Trim());
                    string classString = splitDataParts[1].Trim();
                    string statusString = splitDataParts[2].Trim();
                    string guest = splitDataParts[3].Trim();

                    // Map string back to RoomClass enum value
                    RoomClass parsedClass = (RoomClass)Enum.Parse(typeof(RoomClass), classString);

                    Room reconstructedRoom = new Room(number, parsedClass);

                    if (statusString == "Occupied")
                    {
                        reconstructedRoom.IsOccupied = true;
                        reconstructedRoom.GuestName = (guest == "-") ? "Unknown Resident" : guest;
                    }

                    parsedHotel.Rooms.Add(reconstructedRoom);
                }
            }

            if (parsedHotel.Rooms.Count == 0)
            {
                throw new HotelException("Parsing error: Unable to identify or reconstruct any valid room entries. Schema verification failed.");
            }

            return parsedHotel;
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
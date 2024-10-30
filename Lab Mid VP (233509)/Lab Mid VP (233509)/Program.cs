using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

abstract class User
{
    public int UserID { get; set; }
    private string name;
    private string phoneNumber;

    public string Name
    {
        get => name;
        set
        {
            if (!string.IsNullOrEmpty(value))
                name = value;
            else
                throw new ArgumentException("Name cannot be empty.");
        }
    }

    public string PhoneNumber
    {
        get => phoneNumber;
        set
        {
            if (Regex.IsMatch(value, @"^\d{10}$"))
                phoneNumber = value;
            else
                throw new ArgumentException("Phone number must be a 10-digit number.");
        }
    }

    public abstract void Register();
    public abstract void DisplayProfile();
}

class Rider : User
{
    public List<Trip> RideHistory { get; private set; }

    public Rider()
    {
        RideHistory = new List<Trip>();
    }

    public override void Register()
    {
        Console.WriteLine("Rider registered successfully.");
    }

    public override void DisplayProfile()
    {
        Console.WriteLine($"Rider Profile: Name - {Name}, Phone Number - {PhoneNumber}");
    }

    public void RequestRide(RideSharing system, string startLocation, string destination)
    {
        system.RequestRide(this, startLocation, destination);
    }

    public void ViewRideHistory()
    {
        Console.WriteLine("Ride History:");
        foreach (var trip in RideHistory)
        {
            trip.DisplayTripDetails();
        }
    }
}

class Driver : User
{
    public int DriverID { get; set; }
    public string VehicleDetails { get; set; }
    public bool IsAvailable { get; private set; }
    public List<Trip> TripHistory { get; private set; }

    public Driver()
    {
        TripHistory = new List<Trip>();
        IsAvailable = true;
    }

    public override void Register()
    {
        Console.WriteLine("Driver registered successfully.");
    }

    public override void DisplayProfile()
    {
        Console.WriteLine($"Driver Profile: Name - {Name}, Phone Number - {PhoneNumber}, Vehicle - {VehicleDetails}");
    }

    public void AcceptRide(Trip trip)
    {
        trip.Status = "Accepted";
        TripHistory.Add(trip);
        IsAvailable = false;
        Console.WriteLine("Ride accepted successfully.");
    }

    public void CompleteTrip()
    {
        IsAvailable = true;
    }

    public void ViewTripHistory()
    {
        Console.WriteLine("Trip History:");
        foreach (var trip in TripHistory)
        {
            trip.DisplayTripDetails();
        }
    }
}

class Trip
{
    public int TripID { get; set; }
    public string RiderName { get; set; }
    public string DriverName { get; set; }
    public string StartLocation { get; set; }
    public string Destination { get; set; }
    public double Fare { get; private set; }
    public string Status { get; set; }

    public Trip(int tripID, string riderName, string driverName, string startLocation, string destination)
    {
        TripID = tripID;
        RiderName = riderName;
        DriverName = driverName;
        StartLocation = startLocation;
        Destination = destination;
        Fare = CalculateFare(startLocation, destination);
        Status = "Requested";
    }

    public static double CalculateFare(string startLocation, string destination)
    {
        return 100.0; // Simplified fare calculation logic
    }

    public void DisplayTripDetails()
    {
        Console.WriteLine($"Trip ID: {TripID}, Rider: {RiderName}, Driver: {DriverName}, Start: {StartLocation}, Destination: {Destination}, Fare: {Fare:C}, Status: {Status}");
    }
}

class RideSharing
{
    public List<Rider> RegisteredRiders { get; private set; }
    public List<Driver> RegisteredDrivers { get; private set; }
    public List<Trip> AllTrips { get; private set; }

    public RideSharing()
    {
        RegisteredRiders = new List<Rider>();
        RegisteredDrivers = new List<Driver>();
        AllTrips = new List<Trip>();
    }

    public void RegisterUser(User user)
    {
        if (user is Rider rider)
        {
            RegisteredRiders.Add(rider);
        }
        else if (user is Driver driver)
        {
            RegisteredDrivers.Add(driver);
        }
        user.Register();
    }

    public void RequestRide(Rider rider, string startLocation, string destination)
    {
        var availableDriver = RegisteredDrivers.FirstOrDefault(d => d.IsAvailable);
        if (availableDriver != null)
        {
            var trip = new Trip(AllTrips.Count + 1, rider.Name, availableDriver.Name, startLocation, destination);
            rider.RideHistory.Add(trip);
            availableDriver.AcceptRide(trip);
            AllTrips.Add(trip);
            Console.WriteLine("Ride requested successfully.");
        }
        else
        {
            Console.WriteLine("No available drivers at the moment.");
        }
    }

    public void AcceptRide(int tripID)
    {
        var trip = AllTrips.FirstOrDefault(t => t.TripID == tripID && t.Status == "Requested");
        if (trip != null)
        {
            var driver = RegisteredDrivers.First(d => d.Name == trip.DriverName);
            if (driver != null)
            {
                driver.AcceptRide(trip);
                Console.WriteLine("Ride accepted successfully.");
            }
            else
            {
                Console.WriteLine("Driver not found.");
            }
        }
        else
        {
            Console.WriteLine("Trip not found or not requested yet.");
        }
    }

    public void CompleteTrip(int tripID)
    {
        var trip = AllTrips.FirstOrDefault(t => t.TripID == tripID && t.Status == "Accepted");
        if (trip != null)
        {
            trip.Status = "Completed";
            var driver = RegisteredDrivers.First(d => d.Name == trip.DriverName);
            driver.CompleteTrip();
            Console.WriteLine("Trip completed successfully.");
        }
        else
        {
            Console.WriteLine("Trip not found or not accepted yet.");
        }
    }

    public void DisplayAllTrips()
    {
        Console.WriteLine("All Trips:");
        foreach (var trip in AllTrips)
        {
            trip.DisplayTripDetails();
        }
    }

    public void DisplayAllRiders()
    {
        Console.WriteLine("All Registered Riders:");
        foreach (var rider in RegisteredRiders)
        {
            rider.DisplayProfile();
        }
    }

    public void DisplayAllDrivers()
    {
        Console.WriteLine("All Registered Drivers:");
        foreach (var driver in RegisteredDrivers)
        {
            driver.DisplayProfile();
        }
    }
}

class Program
{
    static void Main()
    {
        var system = new RideSharing();

        while (true)
        {
            Console.WriteLine("\nRide Sharing System");
            Console.WriteLine("1. Register as Rider");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Request a Ride");
            Console.WriteLine("4. Accept a Ride");
            Console.WriteLine("5. Complete a Trip");
            Console.WriteLine("6. View Ride History (Riders)");
            Console.WriteLine("7. View Trip History (Drivers)");
            Console.WriteLine("8. Display All Trips");
            Console.WriteLine("9. Display All Riders");
            Console.WriteLine("10. Display All Drivers");
            Console.WriteLine("11. Exit");
            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter rider name: ");
                    string riderName = Console.ReadLine();
                    Console.Write("Enter rider phone number: ");
                    string riderPhone = Console.ReadLine();
                    var rider = new Rider { UserID = system.RegisteredRiders.Count + 1, Name = riderName, PhoneNumber = riderPhone };
                    system.RegisterUser(rider);
                    break;
                case 2:
                    Console.Write("Enter driver name: ");
                    string driverName = Console.ReadLine();
                    Console.Write("Enter driver phone number: ");
                    string driverPhone = Console.ReadLine();
                    Console.Write("Enter vehicle details: ");
                    string vehicleDetails = Console.ReadLine();
                    var driver = new Driver { UserID = system.RegisteredDrivers.Count + 1, Name = driverName, PhoneNumber = driverPhone, VehicleDetails = vehicleDetails };
                    system.RegisterUser(driver);
                    break;
                case 3:
                    Console.Write("Enter rider name: ");
                    riderName = Console.ReadLine();
                    Console.Write("Enter start location: ");
                    string startLocation = Console.ReadLine();
                    Console.Write("Enter destination: ");
                    string destination = Console.ReadLine();
                    var requestingRider = system.RegisteredRiders.FirstOrDefault(r => r.Name == riderName);
                    if (requestingRider != null)
                    {
                        requestingRider.RequestRide(system, startLocation, destination);
                    }
                    else
                    {
                        Console.WriteLine("Rider not found.");
                    }
                    break;
                case 4:
                    Console.Write("Enter trip ID to accept: ");
                    int tripID = int.Parse(Console.ReadLine());
                    system.AcceptRide(tripID);
                    break;
                case 5:
                    Console.Write("Enter trip ID to complete: ");
                    tripID = int.Parse(Console.ReadLine());
                    system.CompleteTrip(tripID);
                    break;
                case 6:
                    Console.Write("Enter rider name: ");
                    riderName = Console.ReadLine();
                    var viewingRider = system.RegisteredRiders.FirstOrDefault(r => r.Name == riderName);
                    if (viewingRider != null)
                    {
                        viewingRider.ViewRideHistory();
                    }
                    else
                    {
                        Console.WriteLine("Rider not found.");
                    }
                    break;
                case 7:
                    Console.Write("Enter driver name: ");
                    driverName = Console.ReadLine();
                    var viewingDriver = system.RegisteredDrivers.FirstOrDefault(d => d.Name == driverName);
                    if (viewingDriver != null)
                    {
                        viewingDriver.ViewTripHistory();
                    }
                    else
                    {
                        Console.WriteLine("Driver not found.");
                    }
                    break;
                case 8:
                    system.DisplayAllTrips();
                    break;
                case 9:
                    system.DisplayAllRiders();
                    break;
                case 10:
                    system.DisplayAllDrivers();
                    break;
                case 11:
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }
}

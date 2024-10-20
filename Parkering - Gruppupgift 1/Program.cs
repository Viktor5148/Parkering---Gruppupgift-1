using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Garage parkingGarage = new Garage();
        bool isRunning = true;

        while (isRunning)
        {
            ShowMenu();
            string userInput = Console.ReadLine()?.Trim();

            switch (userInput)
            {
                case "1":
                    ParkNewVehicle(parkingGarage);
                    break;
                case "2":
                    parkingGarage.ShowParkingSlots();
                    break;
                case "3":
                    MoveParkedVehicle(parkingGarage);
                    break;
                case "4":
                    LocateVehicle(parkingGarage);
                    break;
                case "5":
                    RemoveParkedVehicle(parkingGarage);
                    break;
                case "6":
                    isRunning = false;
                    Console.WriteLine("Stänger programmet, Ses nästa gång!...");
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, försök igen.");
                    break;
            }
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("\n--- KYH'S Parkeringsgarage ---");
        Console.WriteLine("1. Parkera fordon");
        Console.WriteLine("2. Visa parkeringsstatus");
        Console.WriteLine("3. Flytta fordon");
        Console.WriteLine("4. Sök fordon");
        Console.WriteLine("5. Ta bort fordon");
        Console.WriteLine("6. Avsluta");
        Console.Write("Välj ett alternativ: ");
    }

    static void ParkNewVehicle(Garage garage)
    {
        Console.Write("Ange fordonstyp (CAR/MC): ");
        string type = Console.ReadLine()?.ToUpper();

        string regNum;
        do
        {
            Console.Write("Ange registreringsnummer (max 10 tecken): ");
            regNum = Console.ReadLine()?.ToUpper();
            if (regNum.Length > 10)
                Console.WriteLine("Registreringsnummer får max vara 10 tecken.");
        } while (regNum.Length > 10);

        garage.AddVehicleToParking(type, regNum);
    }

    static void MoveParkedVehicle(Garage garage)
    {
        Console.Write("Ange registreringsnummer för fordonet som ska flyttas: ");
        string regNum = Console.ReadLine()?.ToUpper();

        Console.Write("Ange ny plats: ");
        if (int.TryParse(Console.ReadLine(), out int targetSpot))
        {
            garage.RelocateVehicle(regNum, targetSpot);
        }
        else
        {
            Console.WriteLine("Ogiltigt platsnummer.");
        }
    }

    static void LocateVehicle(Garage garage)
    {
        Console.Write("Ange registreringsnummer: ");
        string regNum = Console.ReadLine()?.ToUpper();
        garage.SearchForVehicle(regNum);
    }

    static void RemoveParkedVehicle(Garage garage)
    {
        Console.Write("Ange registreringsnummer för fordonet som ska tas bort: ");
        string regNum = Console.ReadLine()?.ToUpper();
        garage.RemoveVehicleFromParking(regNum);
    }
}

public class Garage
{
    private readonly List<string>[] parkingSpots;

    public Garage()
    {
        parkingSpots = new List<string>[100];
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            parkingSpots[i] = new List<string>();
        }
    }

    public void AddVehicleToParking(string vehicleType, string regNumber)
    {
        int spotIndex = GetFreeSpot(vehicleType);

        if (spotIndex == -1)
        {
            Console.WriteLine("Inga lediga parkeringsplatser.");
            return;
        }

        string vehicleData = $"{vehicleType}#{regNumber}";

        if (vehicleType == "CAR" && parkingSpots[spotIndex].Count == 0)
        {
            parkingSpots[spotIndex].Add(vehicleData);
            Console.WriteLine($"Bil {regNumber} parkerad på plats {spotIndex + 1}.");
        }
        else if (vehicleType == "MC" && parkingSpots[spotIndex].Count < 2)
        {
            parkingSpots[spotIndex].Add(vehicleData);
            Console.WriteLine($"MC {regNumber} parkerad på plats {spotIndex + 1}.");
        }
        else
        {
            Console.WriteLine("Platsen är upptagen.");
        }
    }

    public void ShowParkingSlots()
    {
        Console.WriteLine("\n--- Parkeringsstatus ---");
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            string spotStatus = parkingSpots[i].Count == 0 ? "[TOM]" : string.Join(", ", parkingSpots[i]);
            Console.WriteLine($"Plats {i + 1}: {spotStatus}");
        }
    }

    public void RelocateVehicle(string regNumber, int newSpot)
    {
        if (newSpot < 1 || newSpot > parkingSpots.Length)
        {
            Console.WriteLine("Ogiltig plats.");
            return;
        }

        newSpot--;

        for (int i = 0; i < parkingSpots.Length; i++)
        {
            for (int j = 0; j < parkingSpots[i].Count; j++)
            {
                if (parkingSpots[i][j].Contains(regNumber))
                {
                    if (parkingSpots[newSpot].Count == 0 || (parkingSpots[newSpot][0].StartsWith("MC") && parkingSpots[newSpot].Count < 2))
                    {
                        parkingSpots[newSpot].Add(parkingSpots[i][j]);
                        parkingSpots[i].RemoveAt(j);
                        Console.WriteLine($"Fordon {regNumber} flyttades till plats {newSpot + 1}.");
                    }
                    else
                    {
                        Console.WriteLine("Platsen är upptagen.");
                    }
                    return;
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
    }

    public void RemoveVehicleFromParking(string regNumber)
    {
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            for (int j = 0; j < parkingSpots[i].Count; j++)
            {
                if (parkingSpots[i][j].Contains(regNumber))
                {
                    parkingSpots[i].RemoveAt(j);
                    Console.WriteLine($"Fordon {regNumber} borttaget från plats {i + 1}.");
                    return;
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
    }

    public void SearchForVehicle(string regNumber)
    {
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (parkingSpots[i].Exists(vehicle => vehicle.Contains(regNumber)))
            {
                Console.WriteLine($"Fordon {regNumber} hittades på plats {i + 1}.");
                return;
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
    }

    private int GetFreeSpot(string vehicleType)
    {
        if (vehicleType == "MC")
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i].Count > 0 && parkingSpots[i][0].StartsWith("MC") && parkingSpots[i].Count < 2)
                {
                    return i;
                }
            }
        }

        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (parkingSpots[i].Count == 0)
            {
                return i;
            }
        }

        return -1;
    }
}

using Elevator.Challenge.Console.Models;

namespace Elevator.Challenge.Console.Helpers
{
    public class UserInputHelper : IUserInputHelper
    {
        public async Task<(bool success, int currentFloor, int destinationFloor, int passengerCount)> ParseUserInputAsync(IPrintHelper printHelper, Building building)
        {
            PrintWelcomeMessage(printHelper, building);

            var userInput = System.Console.ReadLine();

            if (userInput != null && userInput.ToLower() == "q")
            {
                printHelper.Print("Elevator request has been cancelled!", ConsoleColor.Red);
                return (true, 0, 0, 0);
            }

            if (userInput == null)
            {
                return (false, 0, 0, 0);
            }

            var elevatorDetails = userInput.Split();

            return await ProcessElevatorRequestValidation(printHelper, building, elevatorDetails);
        }

        private static void PrintWelcomeMessage(IPrintHelper printHelper, Building building)
        {
            printHelper.Print("Welcome to Elevation", ConsoleColor.Green);
            printHelper.Print("Enter current floor, destination floor and number of persons (separated by spaces)", ConsoleColor.Green);
            printHelper.Print($"e.g 4 10 5 and press enter or q to cancel. Floor range (0 - {building.NumberOfFloors})", ConsoleColor.Green);
        }

        private static Task<(bool success, int currentFloor, int destinationFloor, int passengerCount)> ProcessElevatorRequestValidation(IPrintHelper printHelper, Building building, string[] elevatorDetails)
        {
            if (elevatorDetails.Length == 3 &&
                int.TryParse(elevatorDetails[0], out var currentFloor) &&
                int.TryParse(elevatorDetails[1], out var destinationFloor) &&
                int.TryParse(elevatorDetails[2], out var passengerCount))
            {
                if (!IsValidFloor(building, currentFloor) || !IsValidFloor(building, destinationFloor))
                {
                    printHelper.Print("Invalid floor input. Please enter floors within the range of the building.", ConsoleColor.Red);
                    return Task.FromResult((false, 0, 0, 0));
                }

                var maxCapacity = building.Elevators.First().MaxCapacity;

                if (passengerCount <= 0 || passengerCount > maxCapacity)
                {
                    printHelper.Print($"Invalid passenger count. Max Capacity is {maxCapacity}", ConsoleColor.Red);
                    return Task.FromResult((false, 0, 0, 0));
                }

                if (currentFloor == destinationFloor)
                {
                    printHelper.Print("Current floor and destination floor cannot be the same.", ConsoleColor.Red);
                    return Task.FromResult((false, 0, 0, 0));
                }

                return Task.FromResult((true, currentFloor, destinationFloor, passengerCount));
            }

            printHelper.Print("Invalid input. Please enter current floor, destination floor, and passenger count.", ConsoleColor.Red);

            return Task.FromResult((false, 0, 0, 0));
        }

        private static bool IsValidFloor(Building building, int floor)
        {
            return floor >= 0 && floor <= building.NumberOfFloors;
        }
    }
}
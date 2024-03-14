using Elevator.Challenge.Console.Models;

namespace Elevator.Challenge.Console.Helpers
{
    public interface IUserInputHelper
    {
        Task<(bool success, int currentFloor, int destinationFloor, int passengerCount)> ParseUserInputAsync(IPrintHelper printHelper, Building building);
    }
}
using Elevator.Challenge.Console.Models;

namespace Elevator.Challenge.Console.Services
{
    public interface IBuildingService
    {
        Task RequestElevatorAsync(int currentFloor, int destinationFloor, int passengers);

        Building GetBuilding();

        void UpdateBuildingStatus();
    }
}
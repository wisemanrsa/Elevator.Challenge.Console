namespace Elevator.Challenge.Console.Services
{
    using Models;
    public interface IElevatorService
    {
        Task MoveElevatorToFloorAsync(Elevator? elevator, int destinationFloor);

        Task LoadPassengersAsync(Elevator? elevator, int passengers);

        Task UnloadPassengersAsync(Elevator? elevator, int passengers);
    }
}
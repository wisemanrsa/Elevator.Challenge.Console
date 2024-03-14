namespace Elevator.Challenge.Console.Models
{
    public class ElevatorRequest
    {
        public int CurrentFloor { get; }
        public int DestinationFloor { get; }
        public int Passengers { get; }

        public ElevatorRequest(int currentFloor, int destinationFloor, int passengers)
        {
            CurrentFloor = currentFloor;
            DestinationFloor = destinationFloor;
            Passengers = passengers;
        }
    }
}
using Elevator.Challenge.Console.Enums;

namespace Elevator.Challenge.Console.Models
{
    public class Elevator
    {
        public int CurrentFloor { get; set; }
        public int PassengerCount { get; set; }
        public bool IsMoving { get; set; }
        public int Id { get; }
        public int MaxCapacity { get; }
        public int MaxFloor { get; }
        public Direction Direction { get; set; }

        public Elevator(int maxCapacity, int id, int maxFloor)
        {
            MaxCapacity = maxCapacity;
            Id = id;
            Direction = Direction.Stationary;
            MaxFloor = maxFloor;
        }
    }
}
namespace Elevator.Challenge.Console.Models
{
    public class Building
    {
        public int NumberOfFloors { get; }
        public int NumberOfElevators { get; }
        public bool HasQueuedRequests { get; set; }
        public ICollection<Elevator> Elevators { get; set; }

        public Building(int numberOfFloors, int numberOfElevators, int elevatorsMaxLimit)
        {
            NumberOfElevators = numberOfElevators;
            NumberOfFloors = numberOfFloors;

            Elevators = Enumerable.Range(1, numberOfElevators)
                                    .Select(id => new Elevator(elevatorsMaxLimit, id, numberOfFloors))
                                    .ToArray();
        }
    }
}
using Elevator.Challenge.Console.Enums;
using Microsoft.Extensions.Logging;

namespace Elevator.Challenge.Console.Services
{
    using Models;

    public class ElevatorService : IElevatorService
    {
        private const int ElevatorOperationDelay = 1000;
        private readonly ILogger<ElevatorService> _logger;

        public ElevatorService(ILogger<ElevatorService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task MoveElevatorToFloorAsync(Elevator? elevator, int destinationFloor)
        {
            if (elevator == null)
            {
                _logger.LogError("Attempted to move a null elevator instance.");
                throw new ArgumentNullException(nameof(elevator));
            }

            if (destinationFloor < 0 || destinationFloor > elevator.MaxFloor)
            {
                _logger.LogError($"Destination floor {destinationFloor} is out of range for elevator {elevator.Id}.");
                throw new ArgumentOutOfRangeException(nameof(destinationFloor), "Destination floor is out of range.");
            }

            _logger.LogInformation($"Elevator {elevator.Id} is moving from floor {elevator.CurrentFloor} to floor {destinationFloor}.");

            elevator.IsMoving = true;

            // Determine the direction of movement
            elevator.Direction = elevator.CurrentFloor < destinationFloor ? Direction.Up :
                elevator.CurrentFloor > destinationFloor ? Direction.Down :
                Direction.Stationary;

            _logger.LogDebug($"Elevator {elevator.Id} direction set to {elevator.Direction}.");

            // Loop until the elevator reaches the destination floor
            while (elevator.CurrentFloor != destinationFloor)
            {
                elevator.CurrentFloor += elevator.Direction switch
                {
                    Direction.Up => 1,
                    Direction.Down => -1,
                    _ => 0
                };

                _logger.LogDebug($"Elevator {elevator.Id} is at floor {elevator.CurrentFloor}.");

                // Simulate elevator movement delay
                await Task.Delay(ElevatorOperationDelay);
            }

            // Elevator reached the destination floor
            elevator.IsMoving = false;
            elevator.Direction = Direction.Stationary;

            _logger.LogInformation($"Elevator {elevator.Id} reached the destination floor {destinationFloor}.");
        }

        public Task LoadPassengersAsync(Elevator? elevator, int passengers)
        {
            if (elevator == null)
            {
                _logger.LogError("Attempted to load passengers into a null elevator instance.");
                throw new ArgumentNullException(nameof(elevator));
            }

            if (passengers < 0)
            {
                _logger.LogError("Attempted to load a negative number of passengers.");
                throw new ArgumentOutOfRangeException(nameof(passengers), "Number of passengers cannot be negative.");
            }

            if (elevator.PassengerCount + passengers > elevator.MaxCapacity)
            {
                _logger.LogError($"Loading {passengers} passengers would exceed elevator {elevator.Id}'s capacity.");
                throw new InvalidOperationException("Loading passengers would exceed elevator capacity.");
            }

            elevator.PassengerCount += passengers;

            _logger.LogInformation($"Loaded {passengers} passengers into elevator {elevator.Id}. Total passengers now: {elevator.PassengerCount}.");

            return Task.CompletedTask;
        }

        public Task UnloadPassengersAsync(Elevator? elevator, int passengers)
        {
            if (elevator == null)
            {
                _logger.LogError("Attempted to unload passengers from a null elevator instance.");
                throw new ArgumentNullException(nameof(elevator));
            }

            if (passengers < 0)
            {
                _logger.LogError("Attempted to unload a negative number of passengers.");
                throw new ArgumentOutOfRangeException(nameof(passengers), "Number of passengers cannot be negative.");
            }

            if (elevator.PassengerCount - passengers < 0)
            {
                _logger.LogError($"Unloading {passengers} passengers would result in a negative count for elevator {elevator.Id}.");
                throw new InvalidOperationException("Unloading passengers would result in negative passenger count.");
            }

            elevator.PassengerCount -= passengers;

            _logger.LogInformation($"Unloaded {passengers} passengers from elevator {elevator.Id}. Total passengers now: {elevator.PassengerCount}.");

            return Task.CompletedTask;
        }
    }
}
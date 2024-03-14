using Elevator.Challenge.Console.Helpers;
using Elevator.Challenge.Console.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elevator.Challenge.Console.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<BuildingService> _logger;
        private readonly IPrintHelper _printHelper;
        private readonly Building _building;
        private readonly Queue<ElevatorRequest> _elevatorRequestQueue = new();


        public BuildingService(IConfiguration configuration, IElevatorService elevatorService, ILogger<BuildingService> logger, IPrintHelper printHelper)
        {
            _elevatorService = elevatorService ?? throw new ArgumentNullException(nameof(elevatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _printHelper = printHelper;
            _building = LoadBuildingFromConfiguration(configuration);
        }

        private Building LoadBuildingFromConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            try
            {
                var numberOfFloors = int.Parse(configuration["BuildingConfiguration:NumberOfFloors"] ?? string.Empty);
                var numberOfElevators = int.Parse(configuration["BuildingConfiguration:NumberOfElevators"] ?? string.Empty);
                var elevatorsMaxLimit = int.Parse(configuration["BuildingConfiguration:ElevatorsMaxLimit"] ?? string.Empty);
                return new Building(numberOfFloors, numberOfElevators, elevatorsMaxLimit);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid format for building configuration settings.");
                throw;
            }
            catch (OverflowException ex)
            {
                _logger.LogError(ex, "Overflow while parsing building configuration settings.");
                throw;
            }
        }

        public async Task RequestElevatorAsync(int currentFloor, int destinationFloor, int passengers)
        {
            try
            {
                var nearestElevator = FindNearestElevator(currentFloor);

                if (nearestElevator != null)
                {
                    await DispatchElevatorAsync(nearestElevator, currentFloor, destinationFloor, passengers);
                    await HandleQueuedRequestsAsync();
                }
                else
                {
                    QueueElevatorRequest(currentFloor, destinationFloor, passengers);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing an elevator request.");
            }
        }

        public Building GetBuilding() => _building;

        public void UpdateBuildingStatus()
        {
            LogElevatorStatuses();
        }

        private Models.Elevator? FindNearestElevator(int currentFloor)
        {
            return _building.Elevators
                .Where(e => !e.IsMoving)
                .OrderBy(e => Math.Abs(e.CurrentFloor - currentFloor))
                .FirstOrDefault();
        }

        private async Task DispatchElevatorAsync(Models.Elevator elevator, int currentFloor, int destinationFloor, int passengers)
        {
            _logger.LogInformation("Dispatching Elevator {ElevatorId} to floor {TargetFloor}", elevator.Id, currentFloor);
            await _elevatorService.MoveElevatorToFloorAsync(elevator, currentFloor);
            await _elevatorService.LoadPassengersAsync(elevator, passengers);
            await _elevatorService.MoveElevatorToFloorAsync(elevator, destinationFloor);
            await _elevatorService.UnloadPassengersAsync(elevator, passengers);
        }

        private void QueueElevatorRequest(int currentFloor, int destinationFloor, int passengers)
        {
            _building.HasQueuedRequests = true;
            _elevatorRequestQueue.Enqueue(new ElevatorRequest(currentFloor, destinationFloor, passengers));
            _logger.LogInformation("Elevator request queued as all elevators are busy.");
        }

        private async Task HandleQueuedRequestsAsync()
        {
            while (_elevatorRequestQueue.Count > 0)
            {
                var request = _elevatorRequestQueue.Dequeue();
                _logger.LogInformation("Handling queued elevator request.");
                await RequestElevatorAsync(request.CurrentFloor, request.DestinationFloor, request.Passengers);
            }
            _building.HasQueuedRequests = false;
        }

        private void LogElevatorStatuses()
        {
            _printHelper.Print("Building Elevator status:");

            foreach (var elevator in _building.Elevators)
            {
                _printHelper.Print($"Elevator {elevator.Id}: Floor {elevator.CurrentFloor}, Status: {(elevator.IsMoving ? "Moving" : "Idle")}", ConsoleColor.DarkCyan);
            }

            _printHelper.Print($"Queued requests: {_elevatorRequestQueue.Count}", ConsoleColor.DarkGreen);
            _printHelper.Print("Press ENTER to request for an elevator", ConsoleColor.Yellow);
            _printHelper.Print(string.Empty);
        }
    }
}
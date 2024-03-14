using Elevator.Challenge.Console.Helpers;

namespace Elevator.Challenge.Console.Services
{
    public class UserInteractionService
    {
        private readonly IBuildingService _buildingService;
        private readonly IPrintHelper _printHelper;
        private readonly IUserInputHelper _userInputHelper;
        private readonly CancellationTokenSource _cts = new();
        private CancellationTokenSource _pauseTokenSource = new();
        private readonly Task _elevatorStatusUpdateTask;

        public UserInteractionService(IBuildingService buildingService, IPrintHelper printHelper, IUserInputHelper userInputHelper)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
            _printHelper = printHelper ?? throw new ArgumentNullException(nameof(printHelper));
            _userInputHelper = userInputHelper ?? throw new ArgumentNullException(nameof(userInputHelper));
            _elevatorStatusUpdateTask = Task.Run(ElevatorStatusUpdate, _cts.Token);
        }

        public async Task RunAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    System.Console.ReadLine();
                    // Pause the elevator status updates when user starts typing
                    _pauseTokenSource.Cancel();
                    await HandleUserInputAsync();
                }
            }
            finally
            {
                _cts.Cancel();
                // Wait for the task to complete its current iteration before exiting
                await _elevatorStatusUpdateTask;
            }
        }

        private async Task HandleUserInputAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                var (success, currentFloor, destinationFloor, passengerCount) = await _userInputHelper.ParseUserInputAsync(_printHelper, _buildingService.GetBuilding());

                if (!success)
                {
                    continue; // Go back to the start of the loop and wait for new input
                }

                if (currentFloor == 0 && destinationFloor == 0 && passengerCount == 0)
                {
                    // User cancelled the request by pressing q
                    break;
                }

                // Start the elevator request without waiting for it to complete and observe the task
                _ = _buildingService.RequestElevatorAsync(currentFloor, destinationFloor, passengerCount)
                    .ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            // Log the exception or handle it as needed
                            _printHelper.Print($"An error occurred: {task.Exception.InnerException?.Message}");
                        }
                    }, TaskScheduler.Default);

                // Continue the elevator update on screen
                _pauseTokenSource = new CancellationTokenSource();
                break;
            }
        }

        private async Task ElevatorStatusUpdate()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_pauseTokenSource.IsCancellationRequested)
                {
                    // Wait a bit before trying again when updates are paused
                    await Task.Delay(500);
                    continue;
                }

                _buildingService.UpdateBuildingStatus();
                await Task.Delay(2000, _cts.Token);
            }
        }
    }
}
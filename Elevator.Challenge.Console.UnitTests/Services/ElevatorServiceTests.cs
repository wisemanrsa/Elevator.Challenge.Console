using Elevator.Challenge.Console.Enums;
using Elevator.Challenge.Console.Services;

#pragma warning disable CS8618

namespace Elevator.Challenge.Console.UnitTests.Services
{
    using Microsoft.Extensions.Logging;
    using Models;
    using Moq;

    [TestFixture]
    public class ElevatorServiceTests
    {
        private ElevatorService _elevatorService;
        private Elevator _elevator;
        private Mock<ILogger<ElevatorService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ElevatorService>>();
            _elevatorService = new ElevatorService(_mockLogger.Object);
            _elevator = new Elevator(maxCapacity: 10, id: 1, maxFloor: 10)
            {
                CurrentFloor = 0,
                IsMoving = false,
                Direction = Direction.Stationary,
                PassengerCount = 0
            };
        }

        [Test]
        public async Task MoveElevatorToFloorAsync_WhenCalled_MovesElevatorCorrectly()
        {
            const int destinationFloor = 5;
            await _elevatorService.MoveElevatorToFloorAsync(_elevator, destinationFloor);

            Assert.That(_elevator.CurrentFloor, Is.EqualTo(destinationFloor));
            Assert.That(_elevator.IsMoving, Is.False);
            Assert.That(_elevator.Direction, Is.EqualTo(Direction.Stationary));
        }

        [Test]
        public void MoveElevatorToFloorAsync_WithNullElevator_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _elevatorService.MoveElevatorToFloorAsync(null, 5));
        }

        [Test]
        public void MoveElevatorToFloorAsync_WithInvalidFloor_ThrowsArgumentOutOfRangeException()
        {
            var invalidFloor = _elevator.MaxFloor + 1;
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _elevatorService.MoveElevatorToFloorAsync(_elevator, invalidFloor));
        }

        [Test]
        public async Task LoadPassengersAsync_WithValidData_LoadsPassengersCorrectly()
        {
            const int passengersToLoad = 5;
            await _elevatorService.LoadPassengersAsync(_elevator, passengersToLoad);

            Assert.That(_elevator.PassengerCount, Is.EqualTo(passengersToLoad));
        }

        [Test]
        public void LoadPassengersAsync_WithExcessPassengers_ThrowsInvalidOperationException()
        {
            var passengersToLoad = _elevator.MaxCapacity + 1;
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _elevatorService.LoadPassengersAsync(_elevator, passengersToLoad));
        }

        [Test]
        public async Task UnloadPassengersAsync_WithValidData_UnloadsPassengersCorrectly()
        {
            const int passengersToUnload = 3;
            _elevator.PassengerCount = 5;
            await _elevatorService.UnloadPassengersAsync(_elevator, passengersToUnload);

            Assert.That(_elevator.PassengerCount, Is.EqualTo(2));
        }

        [Test]
        public void UnloadPassengersAsync_WithTooManyPassengers_ThrowsInvalidOperationException()
        {
            const int passengersToUnload = 1;
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _elevatorService.UnloadPassengersAsync(_elevator, passengersToUnload));
        }
    }
}
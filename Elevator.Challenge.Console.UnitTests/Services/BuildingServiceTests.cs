using Elevator.Challenge.Console.Helpers;
using Elevator.Challenge.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
#pragma warning disable CS8625
#pragma warning disable CS8618

namespace Elevator.Challenge.Console.UnitTests.Services
{
    using Models;

    [TestFixture]
    public class BuildingServiceTests
    {
        private Mock<IElevatorService> _elevatorServiceMock;
        private Mock<ILogger<BuildingService>> _loggerMock;
        private Mock<IPrintHelper> _printHelperMock;
        private Mock<IConfiguration> _configurationMock;
        private BuildingService _buildingService;

        [SetUp]
        public void Setup()
        {
            _elevatorServiceMock = new Mock<IElevatorService>();
            _loggerMock = new Mock<ILogger<BuildingService>>();
            _printHelperMock = new Mock<IPrintHelper>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["BuildingConfiguration:NumberOfFloors"]).Returns("10");
            _configurationMock.Setup(c => c["BuildingConfiguration:NumberOfElevators"]).Returns("5");
            _configurationMock.Setup(c => c["BuildingConfiguration:ElevatorsMaxLimit"]).Returns("20");

            _buildingService = new BuildingService(_configurationMock.Object, _elevatorServiceMock.Object,
                _loggerMock.Object, _printHelperMock.Object);
        }

        private void AssertBuildingConfiguration(int expectedFloors, int expectedElevators, int expectedMaxCapacity)
        {
            Assert.That(_buildingService.GetBuilding(), Is.Not.Null);
            Assert.That(_buildingService.GetBuilding().NumberOfFloors, Is.EqualTo(expectedFloors));
            Assert.That(_buildingService.GetBuilding().NumberOfElevators, Is.EqualTo(expectedElevators));
            Assert.That(_buildingService.GetBuilding().Elevators.First().MaxCapacity, Is.EqualTo(expectedMaxCapacity));
        }

        [Test]
        public void Constructor_WithValidArguments_ShouldLoadBuildingFromConfiguration()
        {
            AssertBuildingConfiguration(10, 5, 20);
        }

        [Test]
        public void Constructor_WithNullElevatorService_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BuildingService(_configurationMock.Object,
                null, _loggerMock.Object, _printHelperMock.Object));
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BuildingService(_configurationMock.Object,
                _elevatorServiceMock.Object, null, _printHelperMock.Object));
        }

        [Test]
        public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BuildingService(null,
                _elevatorServiceMock.Object, _loggerMock.Object, _printHelperMock.Object));
        }

        [Test]
        public async Task RequestElevatorAsync_WithNearestElevatorAvailable_ShouldDispatchElevatorAndHandleQueuedRequests()
        {
            await _buildingService.RequestElevatorAsync(0, 5, 3);
            _elevatorServiceMock.Verify(es => es.MoveElevatorToFloorAsync(It.IsAny<Elevator>(), 0), Times.Once);
            _elevatorServiceMock.Verify(es => es.LoadPassengersAsync(It.IsAny<Elevator>(), 3), Times.Once);
            _elevatorServiceMock.Verify(es => es.MoveElevatorToFloorAsync(It.IsAny<Elevator>(), 5), Times.Once);
            _elevatorServiceMock.Verify(es => es.UnloadPassengersAsync(It.IsAny<Elevator>(), 3), Times.Once);
        }

        [Test]
        public async Task RequestElevatorAsync_WithNoAvailableElevators_ShouldQueueElevatorRequest()
        {
            _configurationMock.Setup(c => c["BuildingConfiguration:NumberOfElevators"]).Returns("0");
            _buildingService = new BuildingService(_configurationMock.Object, _elevatorServiceMock.Object,
                _loggerMock.Object, _printHelperMock.Object);

            await _buildingService.RequestElevatorAsync(0, 50, 3);
            _elevatorServiceMock.Verify(es => es.MoveElevatorToFloorAsync(It.IsAny<Elevator>(), 3), Times.Never);
            Assert.That(_buildingService.GetBuilding().HasQueuedRequests, Is.EqualTo(true));
        }
    }
}
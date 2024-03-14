using Elevator.Challenge.Console.Helpers;
using Moq;

namespace Elevator.Challenge.Console.UnitTests.Helpers
{
    using Models;
    using System;

    [TestFixture]
    public class UserInputHelperTests
    {
        private Mock<IPrintHelper> _mockPrintHelper = null!;
        private Building _building = null!;
        private TextReader _originalInput = null!;
        private UserInputHelper _userInputHelper = null!;

        [SetUp]
        public void SetUp()
        {
            _mockPrintHelper = new Mock<IPrintHelper>();
            _building = new Building(10, 3, 10);
            _originalInput = Console.In;
            _userInputHelper = new UserInputHelper();
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetIn(_originalInput); // Restore the original standard input stream
        }

        [Test]
        public async Task ParseUserInputAsync_ValidInput_ReturnsSuccess()
        {
            // Arrange
            const string userInput = "2 5 3";
            Console.SetIn(new StringReader(userInput));

            // Act
            var result = await _userInputHelper.ParseUserInputAsync(_mockPrintHelper.Object, _building);

            // Assert
            Assert.That(result.success, Is.True);
            Assert.That(result.currentFloor, Is.EqualTo(2));
            Assert.That(result.destinationFloor, Is.EqualTo(5));
            Assert.That(result.passengerCount, Is.EqualTo(3));
        }

        [Test]
        public async Task ParseUserInputAsync_QuitCommand_ReturnsSuccessWithQuitFlag()
        {
            // Arrange
            const string userInput = "q";
            Console.SetIn(new StringReader(userInput));

            // Act
            var result = await _userInputHelper.ParseUserInputAsync(_mockPrintHelper.Object, _building);

            // Assert
            Assert.That(result.success, Is.True);
            Assert.That(result.currentFloor, Is.EqualTo(0));
            Assert.That(result.destinationFloor, Is.EqualTo(0));
            Assert.That(result.passengerCount, Is.EqualTo(0));
        }

        [TestCase("-1 5 3", Description = "Invalid floor number (below 0)")]
        [TestCase("11 5 3", Description = "Floor number above building's number of floors")]
        [TestCase("1 5 0", Description = "Invalid passenger count (below 1)")]
        [TestCase("1 5 11", Description = "Passenger count above elevator's max capacity")]
        public async Task ParseUserInputAsync_InvalidInputs_ReturnsFailure(string userInput)
        {
            // Arrange
            Console.SetIn(new StringReader(userInput));

            // Act
            var result = await _userInputHelper.ParseUserInputAsync(_mockPrintHelper.Object, _building);

            // Assert
            Assert.That(result.success, Is.False);
        }
    }
}
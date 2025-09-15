using FluentAssertions;
using LondonTubeNotifier.Core.Services;

namespace UnitTests.Core
{
    public class OnlineUsersTrackerTests
    {
        private readonly OnlineUsersTracker _tracker;

        public OnlineUsersTrackerTests()
        {
            _tracker = new OnlineUsersTracker();
        }

        [Fact]
        public void AddUser_ShouldAddNewUser_WhenUserDoesNotExist()
        {
            // Arrange
            string userId = "user1";
            string connectionId = "conn1";

            // Act
            _tracker.AddUser(userId, connectionId);

            // Assert
            _tracker.IsUserOnline(userId).Should().BeTrue();
        }

        [Fact]
        public void AddUser_ShouldAddAdditionalConnection_WhenUserAlreadyExists()
        {
            // Arrange
            string userId = "user1";
            string firstConnection = "conn1";
            string secondConnection = "conn2";

            // Act
            _tracker.AddUser(userId, firstConnection);
            _tracker.AddUser(userId, secondConnection);

            // Assert
            _tracker.IsUserOnline(userId).Should().BeTrue();
        }

        [Fact]
        public void RemoveUser_ShouldRemoveConnection_WhenUserHasMultipleConnections()
        {
            // Arrange
            string userId = "user1";
            string conn1 = "conn1";
            string conn2 = "conn2";

            _tracker.AddUser(userId, conn1);
            _tracker.AddUser(userId, conn2);

            // Act
            _tracker.RemoveUser(userId, conn1);

            // Assert
            // still online because conn2 exists
            _tracker.IsUserOnline(userId).Should().BeTrue();
        }

        [Fact]
        public void RemoveUser_ShouldRemoveUser_WhenLastConnectionIsRemoved()
        {
            // Arrange
            string userId = "user1";
            string conn1 = "conn1";

            _tracker.AddUser(userId, conn1);

            // Act
            _tracker.RemoveUser(userId, conn1);

            // Assert
            _tracker.IsUserOnline(userId).Should().BeFalse();
        }

        [Fact]
        public void RemoveUser_ShouldNotThrow_WhenUserDoesNotExist()
        {
            // Arrange
            string userId = "nonexistent";
            string conn1 = "conn1";

            // Act
            Action act = () => _tracker.RemoveUser(userId, conn1);

            // Assert
            act.Should().NotThrow();
            _tracker.IsUserOnline(userId).Should().BeFalse();
        }

        [Fact]
        public void IsUserOnline_ShouldReturnFalse_WhenUserNeverAdded()
        {
            // Arrange
            string userId = "unknown";

            // Act
            var result = _tracker.IsUserOnline(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AddAndRemoveMultipleUsers_ShouldTrackAllCorrectly()
        {
            // Arrange
            string userA = "A";
            string userB = "B";

            _tracker.AddUser(userA, "a1");
            _tracker.AddUser(userB, "b1");
            _tracker.AddUser(userA, "a2");

            // Act & Assert
            _tracker.IsUserOnline(userA).Should().BeTrue();
            _tracker.IsUserOnline(userB).Should().BeTrue();

            _tracker.RemoveUser(userA, "a1");
            _tracker.IsUserOnline(userA).Should().BeTrue(); // still online
            _tracker.RemoveUser(userA, "a2");
            _tracker.IsUserOnline(userA).Should().BeFalse();

            _tracker.IsUserOnline(userB).Should().BeTrue();
        }
    }
}

using System.Collections.Generic;
using ContosoTrips.Exceptions;
using ContosoTrips.Trips;
using ContosoTrips.Users;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace TripServiceTests
{
    public class Tests
    {
        [Fact]
        public void Throws_if_user_not_logged_in()
        {
            var user = new User();
            User loggedInUser = null;
            var sut = CreateSut(StubUserSession(loggedInUser));

            Assert.Throws<UserNotLoggedInException>(() => sut.GetTripsByUser(user));
        }

        [Fact]
        public void Returns_no_trips_if_user_not_friend()
        {
            var loggedInUser = new User();
            var user = new User();
            var stubUserSession = StubUserSession(loggedInUser);
            var sut = CreateSut(stubUserSession);

            var trips = sut.GetTripsByUser(user);

            Assert.Empty(trips);
        }

        [Fact]
        public void Returns_trips_if_user_is_friend_of_logged_in_user()
        {
            var loggedInUser = new User();
            var user = new User();
            user.AddFriend(loggedInUser);
            var trip = new Trip("foo");
            var stubUserSession = StubUserSession(loggedInUser);
            var stubTripDAO = Substitute.For<ITripDAO>();
            stubTripDAO.GetTripsBy(user).Returns(new List<Trip> { trip });
            var sut = CreateSut(stubUserSession, stubTripDAO);

            var trips = sut.GetTripsByUser(user);

            Assert.Collection(trips, x => Assert.True(x.Name == trip.Name));
        }

        private static IUserSession StubUserSession(User userToReturn = null)
        {
            var stub = Substitute.For<IUserSession>();
            if (userToReturn == null)
            {
                stub.GetLoggedUser().ReturnsNull();
            }
            else
            {
                stub.GetLoggedUser().Returns(userToReturn);
            }
            return stub;
        }

        private static TripService CreateSut(IUserSession userSession, ITripDAO tripDAO = null)
        {
            return new TripService(userSession, tripDAO ?? Substitute.For<ITripDAO>());
        }
    }
}

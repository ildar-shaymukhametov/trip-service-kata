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
            var sut = CreateSut(StubUserSession());
            Assert.Throws<UserNotLoggedInException>(() => sut.GetTripsByUser(new User()));
        }

        [Fact]
        public void Returns_no_trips_if_user_not_friend()
        {
            var stub = StubUserSession(new User());
            var trips = CreateSut(stub).GetTripsByUser(new User());
            Assert.Empty(trips);
        }

        [Fact]
        public void Returns_trips_if_user_is_friend_of_logged_in_user()
        {
            var loggedInUser = new User();
            var stubUserSession = StubUserSession(loggedInUser);
            var user = new User();
            user.AddFriend(loggedInUser);
            var trip = new Trip("foo");
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

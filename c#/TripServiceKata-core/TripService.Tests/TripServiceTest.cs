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
            var stub = Substitute.For<IUserSession>();
            stub.GetLoggedUser().ReturnsNull();
            var sut = CreateSut(stub);
            Assert.Throws<UserNotLoggedInException>(() => CreateSut(stub).GetTripsByUser(new User()));
        }

        private static TripService CreateSut(IUserSession userSession, ITripDAO tripDAO = null)
        {
            return new TripService(userSession, tripDAO ?? Substitute.For<ITripDAO>());
        }

        [Fact]
        public void Returns_no_trips_if_user_not_friend()
        {
            var stub = Substitute.For<IUserSession>();
            stub.GetLoggedUser().Returns(new User());
            var user = new User();
            user.AddTrip(new Trip("foo"));
            var sut = CreateSut(stub);
            var trips = sut.GetTripsByUser(user);
            Assert.Empty(trips);
        }

        [Fact]
        public void Returns_trips_if_user_is_friend()
        {
            var stubUserSession = Substitute.For<IUserSession>();
            var loggedInUser = new User();
            stubUserSession.GetLoggedUser().Returns(loggedInUser);
            var user = new User();
            user.AddFriend(loggedInUser);
            var trip = new Trip("foo");
            var stubTripDAO = Substitute.For<ITripDAO>();
            stubTripDAO.GetTripsBy(user).Returns(new List<Trip> { trip });
            var sut = CreateSut(stubUserSession, stubTripDAO);
            var trips = sut.GetTripsByUser(user);
            Assert.Collection(trips, x => Assert.True(x.Name == trip.Name));
        }
    }
}

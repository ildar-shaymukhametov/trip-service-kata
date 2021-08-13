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
            var stub = Substitute.For<IUserProvider>();
            stub.GetLoggedUser().ReturnsNull();
            var sut = new TripService(stub);
            Assert.Throws<UserNotLoggedInException>(() => sut.GetTripsByUser(new User()));
        }

        [Fact]
        public void Returns_no_trips_if_user_not_friend()
        {
            var stub = Substitute.For<IUserProvider>();
            stub.GetLoggedUser().Returns(new User());
            var user = new User();
            user.AddTrip(new Trip("foo"));
            var sut = new TripService(stub);
            var trips = sut.GetTripsByUser(user);
            Assert.Empty(trips);
        }
    }
}

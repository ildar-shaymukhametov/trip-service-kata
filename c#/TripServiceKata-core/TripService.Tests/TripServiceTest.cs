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
    }
}

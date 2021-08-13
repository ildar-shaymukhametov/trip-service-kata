using System.Collections.Generic;
using System.Linq;
using ContosoTrips.Exceptions;
using ContosoTrips.Users;

namespace ContosoTrips.Trips
{
    public class TripService
    {
        private readonly IUserSession userSession;
        private readonly ITripDAO tripDAO;

        public TripService(IUserSession userSession, ITripDAO tripDAO)
        {
            this.userSession = userSession;
            this.tripDAO = tripDAO;
        }

        public List<Trip> GetTripsByUser(User user)
        {
            User loggedUser = userSession.GetLoggedUser();
            if (loggedUser == null)
            {
                throw new UserNotLoggedInException();
            }

            bool isFriend = user.GetFriends().Any(x => x.Equals(loggedUser));
            List<Trip> tripList = new List<Trip>();
            if (isFriend)
            {
                tripList = tripDAO.GetTripsBy(user);
            }
            return tripList;
        }
    }
}

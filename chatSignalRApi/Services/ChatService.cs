namespace chatSignalRApi.Services
{
    public class ChatService
    {
        //Key,Value e.g: { {"amad","random value"}, {"naseem","random value for user"}}
        // the second value is going to use as connection ID and signalR is using connection Id
        //for keeping the track of, who is connected and not connected.
        private static readonly Dictionary<string,string>Users=new Dictionary<string, string>();

        //HERE WE WILL SERVER FOR STORING VALUES    
        public bool AddUserToList(string userToAdd)
        {
            //========HERE I AM LOCKED THE MY MEMORY: two users connected simultaneously 
            //then we locked the user and no other user can connected
            lock (Users)
            {
                foreach (var user in Users)
                {
                    //===============username should be unique thats why we are comparing in lower case.
                    if (user.Key.ToLower()==userToAdd.ToLower())
                    {
                        return false;
                    }
                }
                //========IF i found no user present in list then i add new key of user in list.
                Users.Add(userToAdd, null);
                return true;
            }
        }

        //========THIS METHOD WILL TRIGGER INSIDE OUR CHAT HUB======
        public void AddUserConnectionId(string user, string connectionId)
        {
            lock (Users)
            {
                //===========if user is contain in Users dictionary then i will add connectionId or
                // value in the user index
                if (Users.ContainsKey(user))
                {
                    Users[user] = connectionId;
                }
            }

        }

        public string GetUserByConnectionId(string connectionId)
        {
            //==========Any time i want to do something with dictionary then i locking the user
            lock(Users)
            {
                //===========this will give me user by going to passing connection Id
                return Users.Where(x => x.Value == connectionId).Select(x => x.Key).FirstOrDefault();
            }
        }

        public string GetConnectionIdByUser(string user)
        {
            //==========Any time i want to do something with dictionary then i locking the user
            lock (Users)
            {
                return Users.Where(x => x.Key == user).Select(x => x.Value).FirstOrDefault();
            }
        }


        public void RemoveUserFromList(string user)
        {
            lock (Users)
            {
                // value in the user index
                if (Users.ContainsKey(user))
                {
                    Users.Remove(user);
                }
            }
        }

        public string[] GetOnlineUsers()
        {
            lock (Users)
            {
                //===========we will get username that is key and we are not intrested in connectionId or value
                return Users.OrderBy(x => x.Key).Select(x=>x.Key).ToArray();
            }
        }
    }
}

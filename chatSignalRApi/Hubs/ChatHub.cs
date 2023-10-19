using chatSignalRApi.Dtos;
using chatSignalRApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatSignalRApi.Hubs
{
    public class ChatHub:Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        //============whenever user going to connect this hub then this function will triger
        public override async Task OnConnectedAsync()
        {
            //===========WHENEVER USER IS GOING TO CONNECTED THEN IT HAVE A CONNECTION ID
            // WE ADD THIS ID TO GROUP AND NAME OF THE GROUP
            await Groups.AddToGroupAsync(Context.ConnectionId, "LetsDoTalk");
            await Clients.Caller.SendAsync("UserConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "LetsDoTalk");
            //============now we will remove user from the list==========
            var user=_chatService.GetUserByConnectionId(Context.ConnectionId);
            _chatService.RemoveUserFromList(user);
            await DisplayOnlineUsers();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddUserConnectionId(string name)
        {
            //========FOR EVERY CONNECTION THERE WILL BE UNIQUE CONNECTION ID
            //===WE WILL SAVE CONNECITON ID INSIDE OUR DICTIONARY
            _chatService.AddUserConnectionId(name,Context.ConnectionId);
            await DisplayOnlineUsers();

        }

        public async Task ReceiveMessage(MessageDto message)
        {
            await Clients.Group("LetsDoTalk").SendAsync("NewMessage",message);
        }

        //============THIS IS METHOD FOR create  PRIVATE CHAT =============
        public async Task CreatePrivateChat(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From,message.To);
            await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId=_chatService.GetConnectionIdByUser(message.To);
            await Groups.AddToGroupAsync(toConnectionId, privateGroupName);


            //=====opening private chatbox for the other end user
            //this is only for singal client
            await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat",message);
        }

        //=======THIS IS FOR RECIEVED PRIVATE MESSAGE======
        public async Task ReceivedPrivateMessage(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Clients.Group(privateGroupName).SendAsync("NewPrivateMessage",message);
        }

        //==============IN CASE OF REMOVE PRIVATE CHAT===========
        public async Task RemovePrivateChat(string from,string to)
        {
            string privateGroupName = GetPrivateGroupName(from,to);
            await Clients.Group(privateGroupName).SendAsync("ClosePrivateChat");

            //========NOW I WILL REMOVE BOTH USERS FROM THE GROUP NAME
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);

            //==========NOW RETRIVED FROM AND TO ARE TWO SEPERATE USER CONNECTION ID.
            //AND REMOVING FROM THE PRIVATE GROUP NAME
            var toConnectionId = _chatService.GetConnectionIdByUser(to);
            await Groups.RemoveFromGroupAsync(toConnectionId,privateGroupName);




        }


        private async Task DisplayOnlineUsers()
        {
            var onlineUsers = _chatService.GetOnlineUsers();
            //===========THIS WILL BE ANOTHER FUNCTION THAT WE WILL INVOKE FROM ANGULAR
            await Clients.Groups("LetsDoTalk").SendAsync("OnlineUsers", onlineUsers);
        }

        //=======PRIVATE GROUP IS THE NAME OF FROM AND TO.
        private string GetPrivateGroupName(string from, string to)
        {
            //== from:ammad, to:arfat then string is "ammad-arfat"
            var stringCompare=string.CompareOrdinal(from,to)<0;
            return stringCompare ? $"{from}-{to}" : $"{to}-{from}";
        }


    }
}

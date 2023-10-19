using chatSignalRApi.Dtos;
using chatSignalRApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatSignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        //===========NOW NEED TO INJECT CHAT SERVICE HERE is dependency injection for chat service====
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(UserDto userModel)
        {
            if (_chatService.AddUserToList(userModel.Name)) {
              //=========204 status code success=====
              return NoContent();
            }
            return BadRequest("This name is taken aleady, please choose another name");
        }
    }
}

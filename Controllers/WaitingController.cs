using Agar.io_Alpfa.Models;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Entities;
using System.Text;
using Agar.io_Alpfa.Entities.ModelsForControllers;
using System.Numerics;

namespace Agar.io_Alpfa.Controllers
{
    public class WaitingController : Controller
    {
        static ConcurrentList<WebSocket> connections = new();

        [HttpGet]
        public IActionResult Waiting(string Name)
        {
            ViewBag.Name = Name;

            if (++Rules.HowManyPeoplesVote == Rules.NamesOfVoters.Count)
            {
                RedirectAll();
                return RedirectToAction("Game", "Game", new { name = Name });
            }

            return View();//create ws connection
        }

        [HttpPost]
        public IActionResult Waiting([FromBody] WaitingModel model)
        {
            return RedirectToAction("Game", "Game", new { name = model.Name });
        }

        [Route("/ws/waiting")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                connections.Add(webSocket);
                await Task.Delay(30000);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        public void RedirectAll()
        {
            foreach (var connection in connections)
            {
                string s = "StartGame"; 
                connection.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                connection.Dispose();
            }
            connections.Clear();
        }
    }
}

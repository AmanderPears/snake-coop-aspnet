using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {

        public static Game game;

        public GameHub()
        {
            Console.WriteLine("GameHub -was created");
            game = Game.Instance;

        }

        public async Task JoinGame(string id)
        {
            // Console.WriteLine("joingame- " + this.Context.User.);

            game.AddSnake(this.Context.ConnectionId);
            GameData.players.Add(this.Context.ConnectionId);

            await Clients.All.SendAsync("info", GameData.players.Count);
        }

        public void Input(string direction)
        {
            // Console.WriteLine(this.Context.ConnectionId + ", " + direction);
            game.ChangeDirection(this.Context.ConnectionId, direction);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            Console.WriteLine("removed " + this.Context.ConnectionId);

            game.RemoveSnake(this.Context.ConnectionId);
            GameData.RemovePlayer(this.Context.ConnectionId);


            await Clients.All.SendAsync("info", GameData.players.Count);

            // await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
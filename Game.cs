using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using System;
using System.Text.Json;
using System.Drawing;
using Microsoft.AspNetCore.Http;
public sealed class Game
{

    private static readonly Lazy<Game> lazy = new Lazy<Game>(() => new Game());

    public static Game Instance { get { return lazy.Value; } }

    public IHubContext<GameHub> _hubContext;
    public HttpContext _context;

    public int gameBoardwidth { get; set; }
    public int gameBoardheight { get; set; }

    public List<Snake> snakes { get; set; }

    public System.Timers.Timer gameTimer;

    public Cell prey { get; set; }

    public Game()
    {
        this.gameBoardheight = 20;
        this.gameBoardwidth = 30;
        this.snakes = new List<Snake>();

        this.gameTimer = new System.Timers.Timer(700);
        this.gameTimer.Elapsed += Tick;
        this.gameTimer.AutoReset = true;
        this.gameTimer.Enabled = true;
        Console.WriteLine("Game- was created");

        prey = new Cell(0, 0);
        CreateNewPrey(prey);
    }

    public void Tick(object source, ElapsedEventArgs e)
    {
        //move snakes
        MoveSnakes();


        var data = JsonSerializer.Serialize(this);
        // if (_hubContext != null)
        _hubContext.Clients.All.SendAsync("tick", data);
    }

    public void AddSnake(string id)
    {
        this.snakes.Add(new Snake(id));
        Console.WriteLine("snakeAdded");
    }

    public void RemoveSnake(string id)
    {
        var snake = this.snakes.Find(s => s.playerId == id);
        this.snakes.Remove(snake);
    }

    public void MoveSnakes()
    {
        this.snakes.ForEach(s =>
        {
            var head = new Cell(s.cells[s.cells.Count - 1]);

            if (s.direction == "right")
            {
                head.x++;
            }
            if (s.direction == "left")
            {
                head.x--;
            }
            if (s.direction == "up")
            {
                head.y--;
            }
            if (s.direction == "down")
            {
                head.y++;
            }

            //wrap arround
            if (head.x > (gameBoardwidth - 1))
                head.x = 0;

            if (head.y > (gameBoardheight - 1))
                head.y = 0;

            if (head.x < 0)
                head.x = (gameBoardwidth - 1);

            if (head.y < 0)
                head.y = (gameBoardheight - 1);

            s.cells.RemoveAt(0);
            s.cells.Add(head);

            //caught prey?
            if (head.x == prey.x && head.y == prey.y)
            {
                s.cells.Add(new Cell(prey));
                CreateNewPrey(prey);
            }



            if (!s.inputEnabled) s.inputEnabled = true;

            _hubContext.Clients.Client(s.playerId).SendAsync("InputEnabled", s.inputEnabled);

        });
    }

    public void ChangeDirection(string id, string direction)
    {
        Console.WriteLine(id + ", " + direction);
        var snake = snakes.Find(s => s.playerId == id);
        if (snake.direction != direction)
        {
            if ((snake.direction == "right" && direction != "left") ||
            (snake.direction == "left" && direction != "right") ||
            (snake.direction == "up" && direction != "down") ||
            (snake.direction == "down" && direction != "up"))
            {
                snake.inputEnabled = false;
                snake.previousDirection = snake.direction;
                snake.direction = direction;
            }
        }
    }

    public void CreateNewPrey(Cell prey)
    {
        var availableSpots = new List<Cell>();
        for (int x = 0; x < gameBoardwidth; x++)
        {
            for (int y = 0; y < gameBoardheight; y++)
            {
                availableSpots.Add(new Cell(x, y));
            }
        }

        snakes.ForEach(s =>
        {
            s.cells.ForEach(c =>
            {
                var found = availableSpots.Find(a => a.x == c.x && a.y == c.y);
                if (found != null)
                {
                    availableSpots.Remove(found);
                }
            });
        });

        var rnd = new Random();
        int randomIndex = rnd.Next(0, availableSpots.Count);

        prey.x = availableSpots[randomIndex].x;
        prey.y = availableSpots[randomIndex].y;
    }

}

public class Cell
{
    public int x { get; set; }
    public int y { get; set; }

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Cell(Cell other)
    {
        x = other.x;
        y = other.y;
    }

    public override string ToString()
    {
        return base.ToString() + ": (" + x + ", " + y + ")";
    }
}

public class Snake
{
    public string playerId { get; set; }
    public bool invulnerable { get; set; }
    public string direction { get; set; }
    public string previousDirection { get; set; }
    public List<Cell> cells { get; set; }
    public string color { get; set; }
    public bool inputEnabled { get; set; }

    private Random random;

    public Snake(string id)
    {
        this.playerId = id;
        this.invulnerable = true;
        this.direction = "right";
        this.cells = new List<Cell>();
        for (int i = 0; i < 3; i++)
            this.cells.Add(new Cell(i, 0));

        //color
        random = new Random();
        Color c = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        color = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        //input 
        inputEnabled = true;
    }

    public override string ToString()
    {
        string s = "";

        cells.ForEach(c =>
        {
            s = s + c.ToString();
        });

        return base.ToString() + ": " + s;
    }
}
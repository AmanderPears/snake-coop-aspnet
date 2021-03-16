"use strict";

var changeSizeOnce = false;

var gameConnection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();

gameConnection.on("info", function (count) {
    let ele = document.getElementById('playersConnected');
    ele.innerText = count;
});

gameConnection.on("tick", function (json) {
    // console.log(json);
    renderGame(json);
});

gameConnection.on("InputEnabled", function (state) {
    // console.log("inputEnable", state);
    inputEnabled = state;
});

gameConnection.start().then(function () {
    // console.log(gameConnection);

    //send join game signal
    let id = Math.round(Math.random() * Date.now()).toString();
    gameConnection.invoke("JoinGame", id).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});




// document.getElementById("sendButton").addEventListener("click", function (event) {
//     var user = document.getElementById("userInput").value;
//     var message = document.getElementById("messageInput").value;
//     connection.invoke("SendMessage", user, message).catch(function (err) {
//         return console.error(err.toString());
//     });
//     event.preventDefault();
// });

function renderGame(jsonString) {
    let data = JSON.parse(jsonString);
    // console.log(data);

    let sizeMult = 20;

    let gameBoard = document.getElementById("gameBoard");
    let ctx = gameBoard.getContext('2d');

    if (!changeSizeOnce) {
        gameBoard.height = `${data.gameBoardheight * sizeMult}`;
        gameBoard.width = `${data.gameBoardwidth * sizeMult}`;
        changeSizeOnce = true;
    }
    //paint bg
    ctx.fillStyle = '#86b300';
    ctx.fillRect(0, 0, gameBoard.width, gameBoard.height);

    //paint prey
    let preyx = parseInt(data.prey.x) * sizeMult,
        preyy = parseInt(data.prey.y) * sizeMult;
    ctx.fillStyle = 'red';
    ctx.fillRect(preyx, preyy, sizeMult, sizeMult);
    ctx.strokeStyle = '#86b300';
    ctx.strokeRect(preyx, preyy, sizeMult, sizeMult);

    //paint snakes
    for (let i = 0; i < data.snakes.length; i++) {
        let snake = data.snakes[i];
        // for (const snake of data.snakes) {
        for (const cell of snake.cells) {
            let x = parseInt(cell.x) * sizeMult,
                y = parseInt(cell.y) * sizeMult;
            ctx.fillStyle = snake.color;
            ctx.fillRect(x, y, sizeMult, sizeMult);
            ctx.strokeStyle = '#86b300';
            ctx.strokeRect(x, y, sizeMult, sizeMult);
            // ctx.font = "20px Courier Sans";
            // ctx.fillStyle = "white";
            // ctx.fillText(i, 5, 15, sizeMult);
        }
    }

    //cosmetic
    setPlayerColorInfo(data);
}

var inputEnabled = false;
//key press
document.addEventListener('keydown', event => {
    if (inputEnabled) {
        if (event.key == "ArrowLeft") {
            event.preventDefault();
            gameConnection.invoke("Input", "left").catch(function (err) {
                return console.error(err.toString());
            });
        }

        if (event.key == "ArrowRight") {
            event.preventDefault();
            gameConnection.invoke("Input", "right").catch(function (err) {
                return console.error(err.toString());
            });
        }

        if (event.key == "ArrowUp") {
            event.preventDefault();
            gameConnection.invoke("Input", "up").catch(function (err) {
                return console.error(err.toString());
            });
        }

        if (event.key == "ArrowDown") {
            event.preventDefault();
            gameConnection.invoke("Input", "down").catch(function (err) {
                return console.error(err.toString());
            });
        }
    }
});


function setPlayerColorInfo(data) {
    let ele = document.getElementById('playerColor');
    ele.style.background = data.snakes.find(s => s.playerId == gameConnection.connectionId).color;
}


// right directionctx.beginPath();
// ctx.moveTo(x+mult, y);
// ctx.lineTo(x+mult, y+mult);

// ctx.lineTo(x+mult*.5, y+mult);

// //left
// ctx.lineTo(x, y+mult*.7);
// ctx.lineTo(x, y+mult*.3);

// ctx.lineTo(x+mult*.5, y);

// ctx.closePath();
// ctx.fill();
// ctx.stroke();



//incompletelet x = 0;
// let y = 0;
// let mult = 50;

// ctx.beginPath();
// ctx.moveTo(x, y);
// ctx.lineTo(x+mult*.6, y);

// ctx.lineTo(x+mult, y+mult*.2);
// ctx.lineTo(x+mult, y+mult*.4);

// ctx.lineTo(x+mult*.6, y+mult*.4);
// ctx.lineTo(x+mult*.6, y+mult*.1);
// ctx.lineTo(x+mult*.2, y+mult*.1);
// ctx.lineTo(x+mult*.2, y+mult*.45);

// ctx.lineTo(x+mult, y+mult*.45);
// ctx.lineTo(x+mult, y+mult*.79);

// ctx.lineTo(x+mult*.2, y+mult*.85);
// ctx.lineTo(x+mult*.2, y+mult*.9);
// ctx.lineTo(x+mult*.6, y+mult*.9);
// ctx.lineTo(x+mult*.6, y+mult*.8);



// ctx.closePath();
// ctx.fill();
// ctx.stroke();
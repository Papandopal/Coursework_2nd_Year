const scene = document.createElement("canvas")
scene.width = 5000
scene.height = 5000
const stx = scene.getContext("2d") 

const canvas = document.createElement("canvas")
canvas.width = window.innerWidth
canvas.height = window.innerHeight

const ctx = canvas.getContext("2d")
var closeButton = document.getElementById("close");

const gameState = {
  /** @type{{ x: Number, y: Number, user_id: Number ,size: Number, speed: Number, type: string, mouse_x: Number, mouse_y: Number }[]} */
  points: []
}
const colors = ["#ff0000", "#0000ff", "#c0c0c0", "#800080", "#008000", "#000080"]

/** @type{{ x: Number, y: Number, size: Number, color: string }[]} */
let circles = [];

const ws = new WebSocket("/ws")

window.addEventListener('beforeunload', function () {
    ws.send('disconnection');
    ws.close();
    gameState.points.splice(gameState.points.indexOf(cur_player), 1)
});

let cur_player = {
    x: 1, y: 1, size: 1, user_id:0, speed: 1, type: 'pos', mouse_x: 1, mouse_y: 1
}


ws.addEventListener('message', (msg) => {
    str = String(msg.data)
    if (str.includes("UpdateCurrentPlayer ")) {
        
        str = str.replace("UpdateCurrentPlayer ", '')
        const data = JSON.parse(str)
        cur_player = data

        cur_player.x = Math.max(Math.min(data.x, scene.width), 0)
        cur_player.y = Math.max(Math.min(data.y, scene.height), 0)
        gameState.points[data.user_id] = cur_player
    }
    else if (str.includes("UpdateMap ")) {
        str = str.replace("UpdateMap ", '')
        new_circle = JSON.parse(str)
        circles.push(new_circle)
    }
    else if (str.includes("LoadMap ")) {
        str = str.replace("LoadMap ", '')
        circles = JSON.parse(str)
    }
    else if (str.includes("DeleteFood ")) {
        str = str.replace("DeleteFood ", '')
        index = JSON.parse(str)
        circles.splice(index, 1)
    }
    else
    {
        const data = JSON.parse(str)
        gameState.points = data;
    }
 
})

function draw() {
    
    ctx.clearRect(0, 0, canvas.width, canvas.height)  
    ctx.font = "20px Arial"
    ctx.fillStyle = "black"
    ctx.fillText(`Radius: ${cur_player.size}`, 10, 30, 1000)
    ctx.fillText(`X: ${cur_player.x}, Y: ${cur_player.y}`, 10, 60, 1000)
    ctx.fillText(`Mouse_x: ${cur_player.mouse_x}, Mouse_y: ${cur_player.mouse_y}`, 10, 90, 1000)


    for (let circle of circles) {
        const screenX = circle.x - cur_player.x + canvas.width / 2;
        const screenY = circle.y - cur_player.y + canvas.height / 2;

        if (Math.pow(circle.x - cur_player.x, 2) + Math.pow(circle.y - cur_player.y, 2) <= cur_player.size * cur_player.size) {
            ws.send('new_size ' + cur_player.user_id.toString())
            ws.send('eat_food ' + circles.indexOf(circle).toString())
            continue
        }
        
        if (
            screenX + circle.size > 0 &&
            screenY + circle.size > 0 &&
            screenX - circle.size < canvas.width &&
            screenY - circle.size < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, circle.size, 0, Math.PI * 2);
            ctx.fillStyle = circle.color;
            ctx.fill();
        }
    }
    
    for (let point of gameState.points)
    {
        const screenX = point.x - cur_player.x + canvas.width / 2;
        const screenY = point.y - cur_player.y + canvas.height / 2;
       
        if (point != cur_player && point.size > cur_player.size && Math.sqrt(Math.pow(point.x - cur_player.x, 2) + Math.pow(point.y - cur_player.y, 2)) * 1.3 < point.size) {
            alert('Ты съеден')
        }
        
        if (
            screenX + point.size > 0 &&
            screenY + point.size > 0 &&
            screenX - point.size < canvas.width &&
            screenY - point.size < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, point.size, 0, Math.PI * 2);
            ctx.fillStyle = colors[point.user_id % colors.length]
            ctx.fill();
        }
    }
    
  requestAnimationFrame(() => draw())
}
draw();
document.body.append(canvas)


document.addEventListener('mousemove', e => {
    let data_x, data_y
    data_x = cur_player.x + e.clientX - canvas.width / 2;
    data_y = cur_player.y + e.clientY - canvas.height / 2;
    ws.send([`move`, 'index:', cur_player.user_id.toString(), 'X:', data_x.toString(), 'Y:', data_y.toString()].join(" "))
})
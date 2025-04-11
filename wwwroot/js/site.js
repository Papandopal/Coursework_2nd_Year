const scene = document.createElement("canvas")
scene.width = 5000
scene.height = 5000
const stx = scene.getContext("2d") 

const circles = [];

for (let i = 0; i < 200; i++) {
    circle = {
        x: Math.random() * scene.width,
        y: Math.random() * scene.height,
        r: 15,
        color: "#00ff00"
    };
    //stx.arc(circle.x, circle.y, 15, 0, 2 * Math.PI)
    circles.push(circle)
}

const canvas = document.createElement("canvas")
canvas.width = 1500
canvas.height = 600
canvas.style.border = "1px solid black"

const ctx = canvas.getContext("2d")

const gameState = {
  points: []
}
const colors = ["#ff0000", "#00ff00", "#0000ff"]
let cur_x, cur_y

const ws = new WebSocket("/ws")

ws.addEventListener('message', (msg) => {
    str = String(msg.data)
    if (str.includes("UpdateCurrentUserScreen ")) {
        
        str = str.replace("UpdateCurrentUserScreen ", '')
        const data = JSON.parse(str)

        cur_x = Math.max(Math.min(data.x, scene.width), 0)
        cur_y = Math.max(Math.min(data.y, scene.height), 0)
        size = data.size
        gameState.points[data.user].x = cur_x
        gameState.points[data.user].y = cur_y
        gameState.points[data.user].size = size
    }
    else
    {
        const data = JSON.parse(str)
        gameState.points = data;
    }
 
})

function draw() {
    
    ctx.clearRect(0, 0, canvas.width, canvas.height)  

    for (let circle of circles) {
        const screenX = circle.x - cur_x + canvas.width / 2;
        const screenY = circle.y - cur_y + canvas.height / 2;

        if (
            screenX + circle.r > 0 &&
            screenY + circle.r > 0 &&
            screenX - circle.r < canvas.width &&
            screenY - circle.r < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, circle.r, 0, Math.PI * 2);
            ctx.fillStyle = circle.color;
            ctx.fill();
        }
    }
    
    for (let point of gameState.points)
    {
        const screenX = point.x - cur_x + canvas.width / 2;
        const screenY = point.y - cur_y + canvas.height / 2;

        if (
            screenX + point.size > 0 &&
            screenY + point.size > 0 &&
            screenX - point.size < canvas.width &&
            screenY - point.size < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, point.size, 0, Math.PI * 2);
            ctx.fillStyle = colors[point.user % colors.length]
            ctx.fill();
        }
    }

  requestAnimationFrame(() => draw())
}
draw();
document.body.append(canvas)

document.addEventListener('mousemove', e => {
    let data_x, data_y
    data_x = cur_x + e.clientX - canvas.width / 2;
    data_y = cur_y + e.clientY - canvas.height / 2;
    ws.send(`move ` + 'X:' + data_x.toString() + ' ' + 'Y:' + data_y.toString())
})
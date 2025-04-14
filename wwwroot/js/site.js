const scene = document.createElement("canvas")
scene.width = 5000
scene.height = 5000
const stx = scene.getContext("2d") 

const canvas = document.createElement("canvas")
canvas.width = 1500
canvas.height = 600
canvas.style.border = "1px solid black"

const ctx = canvas.getContext("2d")

const gameState = {
  points: []
}
const colors = ["#ff0000", "#00ff00", "#0000ff"]




const circles = [];

for (let i = 0; i < 200; i++) {
    circle = {
        x: Math.random() * scene.width,
        y: Math.random() * scene.height,
        r: 15,
        color: colors[Math.floor(Math.random() * colors.length)]
    };
    //stx.arc(circle.x, circle.y, 15, 0, 2 * Math.PI)
    circles.push(circle)
}

const ws = new WebSocket("/ws")
let cur_pos = {
    x: 1, y: 1, size: 1, speed: 1, type: 'pos', mouse_x: 1, mouse_y:1
}

ws.addEventListener('message', (msg) => {
    str = String(msg.data)
    if (str.includes("UpdateCurrentUserScreen ")) {
        
        str = str.replace("UpdateCurrentUserScreen ", '')
        const data = JSON.parse(str)
        cur_pos = data

        cur_pos.x = Math.max(Math.min(data.x, scene.width), 0)
        cur_pos.y = Math.max(Math.min(data.y, scene.height), 0)
        gameState.points[data.user] = cur_pos
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
        const screenX = circle.x - cur_pos.x + canvas.width / 2;
        const screenY = circle.y - cur_pos.y + canvas.height / 2;
        
        if (Math.pow(circle.x - cur_pos.x, 2) + Math.pow(circle.y - cur_pos.y, 2) <= cur_pos.size * cur_pos.size) {
            cur_pos.size += 5
            gameState.points[cur_pos.user].size = cur_pos.size
            ws.send('new_size ' + cur_pos.user.toString() + ' ' + cur_pos.size.toString())
            circles.splice(circles.indexOf(circle), 1)
            continue
        }
        
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
        const screenX = point.x - cur_pos.x + canvas.width / 2;
        const screenY = point.y - cur_pos.y + canvas.height / 2;

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
    data_x = cur_pos.x + e.clientX - canvas.width / 2;
    data_y = cur_pos.y + e.clientY - canvas.height / 2;
    ws.send(`move ` + 'X:' + data_x.toString() + ' ' + 'Y:' + data_y.toString())
})
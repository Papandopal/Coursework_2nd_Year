const scene = document.createElement("canvas")
scene.width = MAPWIDTH
scene.height = MAPHEIGHT
const stx = scene.getContext("2d")

const canvas = document.createElement("canvas")
canvas.width = window.innerWidth
canvas.height = window.innerHeight

const ctx = canvas.getContext("2d")
var closeButton = document.getElementById("close");

const gameState = {

    points: []
}
const colors = ["#ff0000", "#0000ff", "#c0c0c0", "#800080", "#008000", "#000080"]

let circles = [];

let size_of_map = 200
let size_of_point_on_map = 2


const ws = new WebSocket("/ws")
ws.addEventListener("open", e => {
    ws.send(['MyNameIs:', NAME].join(' '))
})

window.addEventListener('beforeunload', function () {
    ws.send(['disconnect', 'id:', cur_player.user_id.toString()].join(' '));
    ws.close();
    gameState.points.splice(gameState.points.indexOf(cur_player), 1)
});

let cur_player = {
    x: 1, y: 1, user_id: -1, size: 1, speed: 1, name: NAME
}
let visibility = 1.0
let IDied = false

let ViewMap = false

ws.addEventListener('message', (msg) => {
    str = String(msg.data)
    if (str.includes("UpdateCurrentPlayer ")) {

        str = str.replace("UpdateCurrentPlayer ", '')
        const data = JSON.parse(str)
        //cur_player = data

        cur_player.x = Math.max(Math.min(data.x, scene.width), 0)
        cur_player.y = Math.max(Math.min(data.y, scene.height), 0)
        cur_player.user_id = data.user_id
        cur_player.size = data.size;
        cur_player.speed = data.speed
        cur_player.name = data.name

        for (let point in gameState.points) {
            if (point.user_id == cur_player.user_id) {
                point = cur_player;
                break;
            }
        }

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
    else if (str.includes("Reset ")) {
        str = str.replace("Reset ", '')
        window.location.pathname = str

    }
    else {
        const data = JSON.parse(str)
        gameState.points = data;


        for (let point of gameState.points) {

            if (point.user_id == cur_player.user_id) {
                cur_player.x = Math.max(Math.min(point.x, scene.width), 0)
                cur_player.y = Math.max(Math.min(point.y, scene.height), 0)
                cur_player.size = point.size;
                cur_player.speed = data.speed
                cur_player.name = data.name
                break
            }
        }

    }

})

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    //ctx.font = "20px Arial"
    //ctx.fillStyle = "black"
    //ctx.fillText(`Radius: ${cur_player.size}`, 10, 30, 1000)
    //ctx.fillText(`X: ${cur_player.x}, Y: ${cur_player.y}`, 10, 60, 1000)


    for (let circle of circles) {
        const screenX = (circle.x - cur_player.x) * visibility + canvas.width / 2;
        const screenY = (circle.y - cur_player.y) * visibility + canvas.height / 2;

        if (!circle.is_eated && Math.pow(circle.x - cur_player.x, 2) + Math.pow(circle.y - cur_player.y, 2) <= cur_player.size * cur_player.size) {
            ws.send(['new_size', cur_player.user_id.toString()].join(' '))
            ws.send(['eat_food', 'index_of_circle', circles.indexOf(circle).toString()].join(' '))
            circle.is_eated = true
            continue
        }

        if (
            screenX + circle.size * visibility > 0 &&
            screenY + circle.size * visibility > 0 &&
            screenX - circle.size * visibility < canvas.width &&
            screenY - circle.size * visibility < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, circle.size * visibility, 0, Math.PI * 2);
            ctx.fillStyle = circle.color;
            ctx.fill();
        }
    }

    for (let point of gameState.points) {

        if (IDied) continue

        const screenX = (point.x - cur_player.x) * visibility + canvas.width / 2;
        const screenY = (point.y - cur_player.y) * visibility + canvas.height / 2;

        if (point != cur_player && IDied == false && point.size * visibility > cur_player.size * visibility && Math.sqrt(Math.pow(point.x - cur_player.x, 2) + Math.pow(point.y - cur_player.y, 2)) * 1.3 < point.size * visibility) {
            ws.send(['kill', 'user_id:', cur_player.user_id.toString(), 'killer_id:', point.user_id.toString()].join(' '))
            IDied = true
        }

        if (
            screenX + point.size * visibility > 0 &&
            screenY + point.size * visibility > 0 &&
            screenX - point.size * visibility < canvas.width &&
            screenY - point.size * visibility < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, point.size * visibility, 0, Math.PI * 2);
            ctx.fillStyle = colors[point.user_id % colors.length]
            ctx.fill();
        }
    }

    if (ViewMap) {

        ctx.clearRect(1,1, size_of_map, size_of_map)
        ctx.strokeRect(1, 1, size_of_map, size_of_map)

        for (let point of gameState.points) {
            ctx.beginPath()
            ctx.arc((point.x / MAPWIDTH) * size_of_map, (point.y / MAPHEIGHT) * size_of_map, size_of_point_on_map, 0, Math.PI * 2)
            ctx.fillStyle = colors[point.user_id % colors.length]
            ctx.fill();
        }
    }

    if (cur_player.size > 200 && visibility > 0.25) visibility -= 0.01
    else if (cur_player.size > 100 && visibility > 0.5) visibility -= 0.01
    else if (cur_player.size > 50 && visibility > 0.75) visibility -= 0.01
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

document.addEventListener('keypress', e => {
    if (e.code == 'KeyV' && !e.ctrlKey && !e.metaKey && !e.shiftKey) {
        ViewMap = true
    }
})

document.addEventListener('keyup', e => {
    if (e.code == 'KeyV' && !e.ctrlKey && !e.metaKey && !e.shiftKey) {
        ViewMap = false
    }
})
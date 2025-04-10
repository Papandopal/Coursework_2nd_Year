﻿const canvas = document.createElement("canvas")
canvas.width = 1200
canvas.height = 600
canvas.style.border = "1px solid black"

const ctx = canvas.getContext("2d")

const gameState = {
  points: []
}
const colors = ["#ff0000", "#00ff00", "#0000ff"]

const ws = new WebSocket("/ws")
ws.addEventListener('message', (msg)  => {
  const data = JSON.parse(msg.data)
  gameState.points = data;
})

function draw() {
  // console.log(canvas)
    ctx.fillStyle = '#ffffff'
    ctx.fillRect(0, 0, canvas.width, canvas.height)
  for(let point of gameState.points) {
    console.log(point)
    ctx.fillStyle = colors[point.user % colors.length]
    ctx.beginPath();
    ctx.arc(point.x, point.y, point.size, 0, 2 * 3.141592);
    ctx.fill();
  }

  requestAnimationFrame(() => draw())
}
draw();
document.body.append(canvas)

document.addEventListener('mousemove', e => {
    ws.send(`move `+'X:' + e.clientX.toString()+' '+'Y:'+e.clientY.toString())
  // console.log('keydown')
})
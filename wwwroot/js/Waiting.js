const ws = new WebSocket("/ws/waiting")

ws.addEventListener('message', (msg) => {
    
    str = String(msg.data)
    if (str.includes("StartGame")) {
        document.getElementById('RedirectToGame').submit()
        ws.close()
    }
})
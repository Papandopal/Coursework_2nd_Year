const ws = new WebSocket("/ws/waiting")

ws.addEventListener('message', (msg) => {
    
    str = String(msg.data)
    if (str.includes("StartGame")) {
        document.getElementById('RedirectToGame').submit()
        /*
        fetch('/Waiting/Waiting', {
            method: 'POST',
            headers: { 'content-type': 'application / json' },
            body: JSON.stringify({ Name:window.NAME })
        })

        fetch('/Waiting/')
        */
        ws.close()
    }
})
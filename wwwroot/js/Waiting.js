const ws = new WebSocket("/ws/waiting")
console.log('kjbh')
//ws.addEventListener('open', () => {
console.log('open', ws)
//setTimeout(function () { document.getElementById('RedirectToGame').submit() }, 5000)

ws.addEventListener('message', (msg) => {
    console.log("jbshbcbsucb")
    str = String(msg.data)
    if (str.includes("StartGame")) {
        document.getElementById('RedirectToGame').submit()
        ws.close()
    }
})

        //fetch(`/Game/Game?name=${encodeURIComponent('@Name')}`).then(response=>response.json()).then(data=>console.log(data))
        //window.location.assign(`/Game/Game?name=${encodeURIComponent('@Name')}`)
        //window.location.href = '/Game/Game';
        //ws.close();
        /*
        fetch('/Waiting/RedirectToGame', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json'},
            body: JSON.stringify()
        }).then(response => {
            if (!response.ok) {
                throw new Error('Ошибка при отправке запроса');
            }
            return response.json();
        }).then(data => {
            console.log('Успех:', data);
        	
        }).catch(error => {
            console.error('Ошибка:', error);
        });
        */


    //})
//})
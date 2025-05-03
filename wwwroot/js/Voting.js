document.getElementById('voteButton').addEventListener('click', () => {

    let mapSize = document.getElementById('mapSize').value;
    let speed = document.getElementById('speed').value;

    fetch('/Voting/Voting', {
        method: 'POST',
        headers: { 'content-type': 'application / json' },
        body: JSON.stringify({ SelectedMapSize: mapSize, SelectedSpeed: speed })
    })
        
 })
   
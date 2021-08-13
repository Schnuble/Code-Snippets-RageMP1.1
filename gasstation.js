//Props Hashes für die Tanksäulen
const gas_hashes = [
    {Name: 'prop_gas_pump_1a'},
    {Name: 'prop_gas_pump_1b'},
    {Name: 'prop_gas_pump_1c'},
    {Name: 'prop_gas_pump_1d'},
    {Name: 'prop_gas_pump_old3'},
    {Name: 'prop_gas_pump_old2'}
];

//Taste "E" looped durch alle Tanksäulen durch und fragt, ob sich der Spieler mit einem Fahrzeug an einer Tanksäule befindet
mp.keys.bind(0x45, true, function () {
    if (mp.gui.cursor.visible) return;
    var playerpos = mp.players.local.position;
    var maxdistance = 2.0;

    gas_hashes.forEach((gas) => {
        var object = mp.game.object.getClosestObjectOfType(playerpos.x, playerpos.y, playerpos.z, maxdistance, mp.game.joaat(gas.Name), false, false, false);
        if(object != 0) {
            if (mp.players.local.vehicle) return; //Ist der Spieler noch im Fahrzeug?
            var result = mp.vehicles.atHandle(mp.game.vehicle.getClosestVehicle(playerpos.x, playerpos.y, playerpos.z, 2, 0, 70)); //Suche das nähste Fahrzeug
            mp.events.callRemote("EventName", result); //Fahrzeug zum Server senden und tanken lassen.
            return;
        }
    });
});

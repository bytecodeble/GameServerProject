var express = require('express');
var mysql = require('mysql');

var app = express();
var port = process.env.port || 3000;

function getConnection() {
    return mysql.createConnection({
        host: "localhost",
        user: "root",
        password: "",
        database: "gameserverprog"
    });
}

// 1. fetch player data (name and health)
app.get('/getPlayerData/:id', function(req, res) {
    var con = getConnection();
    con.connect();
    con.query("SELECT firstname, health FROM players WHERE id = ?", [req.params.id], function(err, rows) {
        if (err) throw err;
        if (rows.length > 0) {
            res.json(rows[0]);
        } else {
            res.status(404).send("Not Found");
        }
        con.end();
    });
});

// 2. fetch inventory for the player
app.get('/getInventory/:id', function(req, res) {
    var con = getConnection();
    con.connect();
    con.query("SELECT item_name, quantity FROM inventory WHERE player_id = ?", [req.params.id], function(err, rows) {
        if (err) throw err;
        res.json(rows); // return an array of items with their quantities
        con.end();
    });
});

// 3. open chest with health potion
app.get('/openChest/:id', function(req, res) {
    var con = getConnection();
    con.connect();
    // if the player already has a Health Potion, increase the quantity by 1; otherwise, insert a new record
    var query = "INSERT INTO inventory (player_id, item_name, quantity) VALUES (?, 'Health Potion', 1) ON DUPLICATE KEY UPDATE quantity = quantity + 1";
    
    con.query(query, [req.params.id], function(err, result) {
        if (err) throw err;
        console.log("Chest opened for player " + req.params.id);
        res.json({ message: "You found a Health Potion!", item: "Health Potion" });
        con.end();
    });
});

// 4. Use Potion (Decrease quantity and Increase health)
app.get('/useItem/:id', function(req, res) {
    var con = getConnection();
    con.connect();
    
    // 1. Subtract 1 from quantity
    con.query("UPDATE inventory SET quantity = quantity - 1 WHERE player_id = ? AND item_name = 'Health Potion' AND quantity > 0", [req.params.id], function(err, result) {
        if (err) throw err;
        
        if (result.affectedRows > 0) {
            // 2. Increase health
            con.query("UPDATE players SET health = health + 10 WHERE id = ?", [req.params.id], function(err2, result2) {
                // 3. Clean up: Delete items with 0 quantity
                con.query("DELETE FROM inventory WHERE quantity <= 0");
                res.json({ message: "Potion used! +10 HP" });
                con.end();
            });
        } else {
            res.json({ message: "No potions to use!" });
            con.end();
        }
    });
});

// 5. Discard Potion (Just delete the item)
app.get('/discardItem/:id', function(req, res) {
    var con = getConnection();
    con.connect();
    con.query("UPDATE inventory SET quantity = quantity - 1 WHERE player_id = ? AND item_name = 'Health Potion' AND quantity > 0", [req.params.id], function(err, result) {
        if (err) throw err;
        // Clean up
        con.query("DELETE FROM inventory WHERE quantity <= 0");
        res.json({ message: "One item discarded." });
        con.end();
    });
});

app.listen(port, () => {
    console.log("Server running on port " + port);
});
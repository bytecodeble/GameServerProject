CREATE DATABASE IF NOT EXISTS gameserverprog;
USE gameserverprog;

CREATE TABLE IF NOT EXISTS players (
    id INT(5) AUTO_INCREMENT PRIMARY KEY,
    firstname VARCHAR(20),
    lastname VARCHAR(20),
    health INT(10)
);


CREATE TABLE IF NOT EXISTS inventory (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT,
    item_name VARCHAR(50),
    quantity INT,
    UNIQUE KEY player_item (player_id, item_name)
);


INSERT INTO players (id, firstname, lastname, health) 
VALUES (1, 'Marie', 'Scientist', 20)
ON DUPLICATE KEY UPDATE firstname='Marie';
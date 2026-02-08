# Unity-MySQL State Sync
A lightweight Unity-to-MySQL synchronization system using Node.js to manage inventory transactions and proximity-based interaction.

## How to Setup
1. Open phpMyAdmin (usually http://localhost/phpmyadmin)
2. Click the Import tab at the top
3. Choose the database.sql file you just downloaded
4. Click Go/Execute
5. Run npm install and node app.js
6. Open Unity and press Play

## Technical Stack
- **Client:** Unity 2022+ (C#)
- **Backend:** Node.js (Express)
- **Database:** MySQL (XAMPP/Localhost)
- **Protocol:** HTTP/REST
- **Data Format:** JSON (using SimpleJSON for Unity)

## Core Features
- **State Persistence:** Real-time synchronization of player stats (Health) and inventory actions (pickup, use, discard) with MySQL.
- **Proximity Interaction:** Distance-based validation (2.5f units) for environmental triggers like chest looting.
- **Server-Side Authority:** Atomic stat calculations (e.g., HP recovery) handled by Node.js to prevent client-side data tampering.
- **Dynamic Spawning:** Fetches player coordinates and attributes from the database to instantiate the player in the scene.

## Project Structure
- `ServerControl.cs`: Manages Unity-to-Server communication, JSON parsing, and UI binding. 
- `app.js`: Express.js backend processing Unity REST requests and managing SQL transactions with dynamic connection handling. 
- `database.sql`: Create the database with the players table and inventory table with initial testing data. 

## Key API Endpoints
- `GET /getPlayerData/:id`: Fetches player name and health.
- `GET /getInventory/:id`: Retrieves current items in the player's bag.
- `GET /openChest/:id`: Adds a Health Potion to the database.
- `GET /useItem/:id`: Decrements item count and increments player health atomically.

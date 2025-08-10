/ Desktop/GamingLauncher/src/main.js
// Purpose: Electron main process - manages application lifecycle and windows
const { app, BrowserWindow, ipcMain, dialog, shell } = require('electron');
const path = require('path');
const axios = require('axios');
const Store = require('electron-store');

// Initialize persistent storage
const store = new Store();

// Configuration
const CONFIG = {
    API_BASE_URL: 'http://localhost:5000/api/v1',
    WINDOW_SETTINGS: {
        width: 1200,
        height: 800,
        minWidth: 1000,
        minHeight: 600
    }
};

class GamingLibraryLauncher {
    constructor() {
        this.mainWindow = null;
        this.terminalWindow = null;
        this.currentPlayer = null;
        
        this.setupEventHandlers();
    }

    setupEventHandlers() {
        // App event handlers
        app.whenReady().then(() => this.onReady());
        app.on('window-all-closed', () => this.onWindowAllClosed());
        app.on('activate', () => this.onActivate());

        // IPC handlers for communication with renderer process
        ipcMain.handle('get-player-data', () => this.getPlayerData());
        ipcMain.handle('create-player', (event, playerData) => this.createPlayer(playerData));
        ipcMain.handle('launch-game', (event, gameData) => this.launchGame(gameData));
        ipcMain.handle('check-api-status', () => this.checkApiStatus());
        ipcMain.handle('get-leaderboard', () => this.getLeaderboard());
    }

    async onReady() {
        console.log('🚀 Gaming Library Launcher starting...');
        
        // Check if API is running
        const apiStatus = await this.checkApiStatus();
        if (!apiStatus.isRunning) {
            console.warn('⚠️  Backend API is not running');
        }

        this.createMainWindow();
        
        // Load or create player profile
        await this.initializePlayer();
    }

    createMainWindow() {
        console.log('🖥️  Creating main window...');
        
        this.mainWindow = new BrowserWindow({
            ...CONFIG.WINDOW_SETTINGS,
            webPreferences: {
                nodeIntegration: false,
                contextIsolation: true,
                preload: path.join(__dirname, 'preload.js')
            },
            icon: path.join(__dirname, 'assets/images/icon.png'),
            title: 'Gaming Library - Portfolio Showcase',
            titleBarStyle: 'default',
            show: false // Don't show until ready
        });

        // Load the main desktop interface
        this.mainWindow.loadFile(path.join(__dirname, 'index.html'));

        // Show window when ready
        this.mainWindow.once('ready-to-show', () => {
            console.log('✅ Main window ready');
            this.mainWindow.show();
            
            // Open DevTools in development
            if (process.env.NODE_ENV === 'development') {
                this.mainWindow.webContents.openDevTools();
            }
        });

        this.mainWindow.on('closed', () => {
            this.mainWindow = null;
        });
    }

    createTerminalWindow(gameType) {
        console.log(`🖥️  Creating terminal window for ${gameType}...`);
        
        this.terminalWindow = new BrowserWindow({
            width: 800,
            height: 600,
            parent: this.mainWindow,
            modal: true,
            webPreferences: {
                nodeIntegration: false,
                contextIsolation: true,
                preload: path.join(__dirname, 'preload.js')
            },
            title: `Loading ${gameType}...`,
            resizable: false,
            frame: false, // Frameless for terminal look
            backgroundColor: '#000000'
        });

        this.terminalWindow.loadFile(path.join(__dirname, 'terminal.html'), {
            query: { game: gameType }
        });

        this.terminalWindow.on('closed', () => {
            this.terminalWindow = null;
        });

        return this.terminalWindow;
    }

    async initializePlayer() {
        console.log('👤 Initializing player...');
        
        // Try to load existing player from storage
        const savedPlayer = store.get('currentPlayer');
        
        if (savedPlayer) {
            console.log(`👤 Found saved player: ${savedPlayer.name}`);
            this.currentPlayer = savedPlayer;
        } else {
            console.log('👤 No saved player found - will prompt for creation');
            // Player creation will be handled by the UI
        }
    }

    async checkApiStatus() {
        try {
            console.log('🔍 Checking API status...');
            const response = await axios.get(`${CONFIG.API_BASE_URL}/health`, { timeout: 5000 });
            console.log('✅ API is running');
            return { isRunning: true, status: response.status };
        } catch (error) {
            console.warn('❌ API is not accessible:', error.message);
            return { isRunning: false, error: error.message };
        }
    }

    async createPlayer(playerData) {
        try {
            console.log(`👤 Creating player: ${playerData.name}`);
            
            const response = await axios.post(`${CONFIG.API_BASE_URL}/players`, {
                name: playerData.name,
                email: playerData.email
            });

            const newPlayer = response.data;
            
            // Save to local storage
            store.set('currentPlayer', newPlayer);
            this.currentPlayer = newPlayer;
            
            console.log(`✅ Player created: ${newPlayer.playerId}`);
            return { success: true, player: newPlayer };
            
        } catch (error) {
            console.error('❌ Failed to create player:', error.response?.data || error.message);
            return { 
                success: false, 
                error: error.response?.data || 'Failed to create player' 
            };
        }
    }

    getPlayerData() {
        return this.currentPlayer;
    }

    async getLeaderboard() {
        try {
            console.log('🏆 Fetching leaderboard...');
            const response = await axios.get(`${CONFIG.API_BASE_URL}/players/leaderboard?count=10`);
            return { success: true, leaderboard: response.data };
        } catch (error) {
            console.error('❌ Failed to fetch leaderboard:', error.message);
            return { success: false, error: error.message };
        }
    }

    async launchGame(gameData) {
        try {
            console.log(`🎮 Launching game: ${gameData.gameType}`);
            
            if (!this.currentPlayer) {
                return { success: false, error: 'No player profile found' };
            }

            // Create terminal loading window
            const terminalWindow = this.createTerminalWindow(gameData.gameType);
            
            // Start game session on backend
            const sessionResponse = await axios.post(`${CONFIG.API_BASE_URL}/gamesessions/start`, {
                playerId: this.currentPlayer.playerId,
                gameType: gameData.gameType === 'Deploy the Cat' ? 1 : 2
            });

            const session = sessionResponse.data;
            console.log(`✅ Game session started: ${session.sessionId}`);

            // Simulate terminal loading sequence
            setTimeout(() => {
                terminalWindow.webContents.send('terminal-message', '> Initializing game engine...');
            }, 500);

            setTimeout(() => {
                terminalWindow.webContents.send('terminal-message', '> Loading game assets...');
            }, 1500);

            setTimeout(() => {
                terminalWindow.webContents.send('terminal-message', '> Connecting to game server...');
            }, 2500);

            setTimeout(() => {
                terminalWindow.webContents.send('terminal-message', '> Authentication successful...');
            }, 3500);

            setTimeout(() => {
                terminalWindow.webContents.send('terminal-message', `> Launching ${gameData.gameType}...`);
            }, 4500);

            // Close terminal and launch actual game after 6 seconds
            setTimeout(() => {
                terminalWindow.close();
                this.launchUnityGame(gameData.gameType, session.sessionId);
            }, 6000);

            return { success: true, sessionId: session.sessionId };
            
        } catch (error) {
            console.error('❌ Failed to launch game:', error.response?.data || error.message);
            return { 
                success: false, 
                error: error.response?.data || 'Failed to launch game' 
            };
        }
    }

    launchUnityGame(gameType, sessionId) {
        console.log(`🎮 Launching Unity game: ${gameType}`);
        
        // In a real implementation, this would launch the Unity executable
        // For now, we'll show a placeholder or open a web-based version
        
        const gameUrls = {
            'Deploy the Cat': 'http://localhost:3000/deploy-the-cat',
            'Git Blaster': 'http://localhost:3000/git-blaster'
        };

        const gameUrl = gameUrls[gameType];
        if (gameUrl) {
            // Open game in default browser with session ID
            shell.openExternal(`${gameUrl}?sessionId=${sessionId}`);
        } else {
            dialog.showMessageBox(this.mainWindow, {
                type: 'info',
                title: 'Game Launch',
                message: `${gameType} is launching...`,
                detail: `Session ID: ${sessionId}\n\nIn a full implementation, this would launch the Unity game executable.`
            });
        }
    }

    onWindowAllClosed() {
        if (process.platform !== 'darwin') {
            app.quit();
        }
    }

    onActivate() {
        if (BrowserWindow.getAllWindows().length === 0) {
            this.createMainWindow();
        }
    }
}

// Initialize the application
const launcher = new GamingLibraryLauncher();

// Handle uncaught exceptions
process.on('uncaughtException', (error) => {
    console.error('Uncaught Exception:', error);
    dialog.showErrorBox('Unexpected Error', error.message);
});
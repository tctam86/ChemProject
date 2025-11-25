#!/usr/bin/env python3
import asyncio
import json
import queue
import sounddevice as sd
import threading
import vosk
import websockets

# --- Cấu hình ---
MODEL_PATH = "model"
SAMPLE_RATE = 16000
WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8765

# --- Ánh xạ lệnh (chỉ tiếng Anh) ---
COMMAND_MAPPINGS = {
    "left": "MOVE_LEFT",
    "right": "MOVE_RIGHT",
    "up": "MOVE_UP",
    "down": "MOVE_DOWN",
    "jump": "JUMP",
    "go left": "MOVE_LEFT",
    "go right": "MOVE_RIGHT",
    "go up": "MOVE_UP",
    "go down": "MOVE_DOWN",
}

# --- Phần lõi của Server ---

audio_queue = queue.Queue()
clients = set()

def audio_callback(indata, frames, time, status):
    if status:
        print(f"Audio callback status: {status}")
    audio_queue.put(bytes(indata))

def find_command(text):
    for keyword, command in COMMAND_MAPPINGS.items():
        if keyword in text:
            return command
    return None

async def broadcast_command(command):
    if clients:
        message = json.dumps({"type": "command", "data": command})
        for client in list(clients):
            try:
                await client.send(message)
            except websockets.exceptions.ConnectionClosed:
                clients.remove(client)

def recognition_thread(loop):
    print("Loading English speech recognition model...")
    try:
        model = vosk.Model(MODEL_PATH)
        recognizer = vosk.KaldiRecognizer(model, SAMPLE_RATE)
        print("Model loaded. Server is ready.")
    except Exception as e:
        print(f"Error loading model: {e}")
        return

    while True:
        data = audio_queue.get()
        if recognizer.AcceptWaveform(data):
            result = json.loads(recognizer.Result())
            text = result.get('text', '').lower().strip()

            if text:
                print(f"Recognized: '{text}'")
                command = find_command(text)
                if command:
                    print(f"==> Command sent: {command}")
                    asyncio.run_coroutine_threadsafe(broadcast_command(command), loop)
                else:
                    print("--> No corresponding command found.")

# ===== HÀM ĐÃ SỬA LỖI =====
# Phải có cả 'websocket' và 'path'
async def handle_client(websocket, path):
    print(f"Unity client connected from {websocket.remote_address}")
    clients.add(websocket)
    try:
        # Giữ kết nối mở và chờ client ngắt kết nối
        await websocket.wait_closed()
    finally:
        print(f"Unity client {websocket.remote_address} disconnected.")
        clients.remove(websocket)
# ==========================

async def start_server():
    print(f"WebSocket Server listening on ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}")
    async with websockets.serve(handle_client, WEBSOCKET_HOST, WEBSOCKET_PORT):
        await asyncio.Future()

if __name__ == "__main__":
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)

    threading.Thread(target=recognition_thread, args=(loop,), daemon=True).start()

    try:
        with sd.RawInputStream(samplerate=SAMPLE_RATE, blocksize=8000, dtype='int16',
                               channels=1, callback=audio_callback):
            print("Listening to microphone...")
            loop.run_until_complete(start_server())
    except KeyboardInterrupt:
        print("\nShutting down server.")
    except Exception as e:
        print(f"Error: {e}")

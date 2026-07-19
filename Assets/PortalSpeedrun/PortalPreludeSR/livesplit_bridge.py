"""
Bridge: Python memory watcher -> LiveSplit, using LiveSplit's built-in "Server" component.

SETUP IN LIVESPLIT:
    1. Right-click the layout -> Edit Layout... -> "+" -> Control -> Server.
       (If you don't see "Server", update LiveSplit; it's built in on modern versions.)
    2. Right-click the timer -> Control -> Start Server (default port 16834).
       You should see "Server started" somewhere in the LiveSplit log/status.

WHAT THIS SCRIPT DOES:
    Connects to that TCP server, then polls one or more memory addresses you already
    found with mem_scanner.py. When a trigger condition is met, it sends the matching
    command over the socket:

        starttimer
        split
        reset
        pause
        unpause
        setgametime <time>
        ...

    Full command list: search "LiveSplit Server component commands" in the LiveSplit docs,
    or right click Server component -> Layout Settings to see the exact command set for
    your version.

Requires: pip install pymem
"""

import socket
import time
import struct
import pymem

PROCESS_NAME = "Wabbitemu.exe"
LIVESPLIT_HOST = "localhost"
LIVESPLIT_PORT = 16834

# Fill these in with what you found using mem_scanner.py.
# Use a POINTER PATH (module base + offsets) if you found one, not a raw address,
# or this will break every time the game restarts.
LEVEL_ADDRESS = 0x2af06082dcc     # placeholder - replace with your real address/pointer result
VALUE_TYPE_FMT = "h"           # struct format matching your value type (i = int32, f = float, etc.)
VALUE_SIZE = 2


class LiveSplitClient:
    def __init__(self, host=LIVESPLIT_HOST, port=LIVESPLIT_PORT):
        self.sock = socket.create_connection((host, port))
        self.sock_file = self.sock.makefile("r")

    def send(self, command):
        self.sock.sendall((command + "\r\n").encode("utf-8"))

    def send_and_read(self, command):
        self.send(command)
        return self.sock_file.readline().strip()

    def close(self):
        self.sock.close()


def read_value(pm, address):
    data = pm.read_bytes(address, VALUE_SIZE)
    return struct.unpack("<" + VALUE_TYPE_FMT, data)[0]


def main():
    pm = pymem.Pymem(PROCESS_NAME)
    ls = LiveSplitClient()
    print("Connected to LiveSplit server.")

    last_value = None
    started = False

    try:
        while True:
            try:
                current = read_value(pm, LEVEL_ADDRESS)
            except Exception as e:
                print("Read failed:", e)
                time.sleep(0.5)
                continue

            if current != last_value:
                print(f"Value changed: {last_value} -> {current}")

                # Example trigger logic - adjust to your game's actual level/state values
                if current == 1:
                    ls.send("starttimer")
                    started = True
                else:
                    ls.send("split")
                    if current in (6, 11, 18, 25, 31, 39):   # trigger a split whenever level advances
                        ls.send("split")

                last_value = current

            time.sleep(0.05)  # poll rate - tune for responsiveness vs CPU usage

    except KeyboardInterrupt:
        pass
    finally:
        ls.close()


if __name__ == "__main__":
    main()

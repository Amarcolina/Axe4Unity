"""
Simple progressive memory scanner for finding LiveSplit autosplitter addresses.

Workflow:
    1. scan(value)          -> initial scan, finds all addresses matching `value`
    2. refine(new_value)    -> filters previous results to addresses now equal to `new_value`
    3. refine_changed()     -> filters to addresses whose value changed since last scan
    4. refine_unchanged()   -> filters to addresses whose value stayed the same
    5. repeat 2-4 until you're down to a small candidate list

Requires: pip install pymem
Run as the SAME bitness as the target process (32-bit python for 32-bit game, etc.)
May need to run as Administrator depending on the process.
"""

import struct
import pymem
import pymem.process

# ---- CONFIG ----
PROCESS_NAME = "WabbitEmu.exe"          # change to your target
VALUE_TYPE = "short"                 # "int", "float", "double", "byte", "short", "int64"
SCAN_MODULES_ONLY = False          # True = only scan readable/writable regions of loaded modules (faster, less thorough)

TYPE_MAP = {
    "byte":   ("B", 1),
    "short":  ("h", 2),
    "int":    ("i", 4),
    "int64":  ("q", 8),
    "float":  ("f", 4),
    "double": ("d", 8),
}


class MemScanner:
    def __init__(self, process_name, value_type="int"):
        self.pm = pymem.Pymem(process_name)
        self.fmt, self.size = TYPE_MAP[value_type]
        self.candidates = {}  # address -> last known value

    def _readable_regions(self):
        """Yield (base_address, region_size) for committed, readable, non-guarded pages."""
        MEM_COMMIT = 0x1000
        PAGE_GUARD = 0x100
        PAGE_NOACCESS = 0x01
        address = 0
        max_address = 0x7FFFFFFF0000
        while address < max_address:
            try:
                mbi = pymem.memory.virtual_query(self.pm.process_handle, address)
            except Exception:
                break
            base = mbi.BaseAddress
            size = mbi.RegionSize
            state = mbi.State
            protect = mbi.Protect
            if state == MEM_COMMIT and not (protect & PAGE_GUARD) and protect != PAGE_NOACCESS:
                yield base, size
            address = base + size

    def _pack(self, value):
        return struct.pack("<" + self.fmt, value)

    def scan(self, value):
        """Initial scan across process memory for an exact value."""
        target = self._pack(value)
        found = {}
        for base, size in self._readable_regions():
            try:
                chunk = self.pm.read_bytes(base, size)
            except Exception:
                continue
            offset = chunk.find(target)
            while offset != -1:
                addr = base + offset
                found[addr] = value
                offset = chunk.find(target, offset + 1)
        self.candidates = found
        print(f"[scan] {len(self.candidates)} candidate addresses found")
        return self.candidates

    def refine(self, value):
        """Filter existing candidates to those now equal to `value`."""
        target = self._pack(value)
        still_valid = {}
        for addr in self.candidates:
            try:
                current = self.pm.read_bytes(addr, self.size)
            except Exception:
                continue
            if current == target:
                still_valid[addr] = value
        self.candidates = still_valid
        print(f"[refine ==] {len(self.candidates)} candidates remain")
        return self.candidates

    def refine_changed(self):
        """Filter to addresses whose value is different from last recorded value."""
        still_valid = {}
        for addr, old_value in self.candidates.items():
            try:
                current_bytes = self.pm.read_bytes(addr, self.size)
                current = struct.unpack("<" + self.fmt, current_bytes)[0]
            except Exception:
                continue
            if current != old_value:
                still_valid[addr] = current
        self.candidates = still_valid
        print(f"[refine changed] {len(self.candidates)} candidates remain")
        return self.candidates

    def refine_unchanged(self):
        """Filter to addresses whose value is the same as last recorded value."""
        still_valid = {}
        for addr, old_value in self.candidates.items():
            try:
                current_bytes = self.pm.read_bytes(addr, self.size)
                current = struct.unpack("<" + self.fmt, current_bytes)[0]
            except Exception:
                continue
            if current == old_value:
                still_valid[addr] = current
        self.candidates = still_valid
        print(f"[refine unchanged] {len(self.candidates)} candidates remain")
        return self.candidates

    def show(self, limit=50):
        for addr, val in list(self.candidates.items())[:limit]:
            print(hex(addr), "=", val)


if __name__ == "__main__":
    scanner = MemScanner(PROCESS_NAME, VALUE_TYPE)

    # --- Example session ---
    print("Play until value is 1 (e.g. Level 1), then press Enter...")
    input()
    scanner.scan(1)

    print("Advance to Level 2, then press Enter...")
    input()
    scanner.refine(2)

    print("Advance to Level 3, then press Enter...")
    input()
    scanner.refine(3)
    
    print("Advance to Level 4, then press Enter...")
    input()
    scanner.refine(4)
    
    print("Advance to Level 5, then press Enter...")
    input()
    scanner.refine(5)

    scanner.show()

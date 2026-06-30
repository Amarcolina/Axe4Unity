using System;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Axe4Unity {

  public struct MachineStateNative {

    public NativeArray<byte> Memory;

    public NativeHashMap<FixedString32Bytes, UnsafeList<byte>> ArchiveFiles;
    public NativeList<FileMetadata> FileMetadata;

    //Maps file handles to the file names they point to
    public NativeHashMap<ushort, FixedString32Bytes> MountedFiles;

    public int CallStackTop;
    public NativeArray<ProgramCounter> CallStack;
    public ProgramCounter PC {
      get => CallStack[CallStackTop - 1];
      set => CallStack[CallStackTop - 1] = value;
    }

    public BitFont.Native LargeFont, SmallFont;

    public int ArgStackTop;
    public NativeArray<ushort> ArgStack;

    public ushort HL;

    public NativeArray<bool> PressedKeys;
    public int LastKeyPressed;

    public Unity.Mathematics.Random Random {
      get {
        uint state = (uint)(Read_U16(Machine.ADDR_RANDOM_STATE) + Read_U16(Machine.ADDR_RANDOM_STATE + 2) << 16);

        var r = new Unity.Mathematics.Random();
        r.InitState(state);

        return r;
      }
      set {
        Write_U16(Machine.ADDR_RANDOM_STATE, (ushort)(value.state & 0xFFFF));
        Write_U16(Machine.ADDR_RANDOM_STATE + 2, (ushort)(value.state >> 16));
      }
    }

    public bool LargeSizeFont {
      get => Read_U8(Machine.ADDR_FLAG_LARGE_FONT) != 0;
      set => Write_U8(Machine.ADDR_FLAG_LARGE_FONT, (ushort)(value ? 1 : 0));
    }

    public bool DrawTextInvert {
      get => Read_U8(Machine.ADDR_FLAG_INVERT_TEXT) != 0;
      set => Write_U8(Machine.ADDR_FLAG_INVERT_TEXT, (ushort)(value ? 1 : 0));
    }

    public bool TextToBuffer {
      get => Read_U8(Machine.ADDR_FLAG_TEXT_TO_BUFFER) != 0;
      set => Write_U8(Machine.ADDR_FLAG_TEXT_TO_BUFFER, (ushort)(value ? 1 : 0));
    }

    public bool TextNoScroll {
      get => Read_U8(Machine.ADDR_FLAG_TEXT_NO_SCROLL) != 0;
      set => Write_U8(Machine.ADDR_FLAG_TEXT_NO_SCROLL, (ushort)(value ? 1 : 0));
    }

    public bool LowercaseEnabled {
      get => Read_U8(Machine.ADDR_FLAG_LOWERCASE_ENABLED) != 0;
      set => Write_U8(Machine.ADDR_FLAG_LOWERCASE_ENABLED, (ushort)(value ? 1 : 0));
    }

    public bool IsFullSpeed {
      get => Read_U8(Machine.ADDR_FLAG_IS_FULL_SPEED) != 0;
      set => Write_U8(Machine.ADDR_FLAG_IS_FULL_SPEED, (ushort)(value ? 1 : 0));
    }

    public int TextCursorX {
      get => Read_U8(Machine.ADDR_PEN_X);
      set => Write_U8(Machine.ADDR_PEN_X, (byte)value);
    }

    public int TextCursorY {
      get => Read_U8(Machine.ADDR_PEN_Y);
      set => Write_U8(Machine.ADDR_PEN_Y, (byte)value);
    }

    public int DispCursorX {
      get => Read_U8(Machine.ADDR_DISP_X);
      set => Write_U8(Machine.ADDR_DISP_X, (byte)value);
    }

    public int DispCursorY {
      get => Read_U8(Machine.ADDR_DISP_Y);
      set => Write_U8(Machine.ADDR_DISP_Y, (byte)value);
    }

    public int MemKitIndex {
      get => Read_U16(Machine.ADDR_MEMKIT_CURR);
      set => Write_U16(Machine.ADDR_MEMKIT_CURR, (ushort)value);
    }

    public void Init(BitFont largeFont, BitFont smallFont) {
      Memory = new NativeArray<byte>(65536, Allocator.Persistent);
      MountedFiles = new NativeHashMap<ushort, FixedString32Bytes>(256, Allocator.Persistent);
      ArchiveFiles = new NativeHashMap<FixedString32Bytes, UnsafeList<byte>>(256, Allocator.Persistent);
      FileMetadata = new NativeList<FileMetadata>(Allocator.Persistent);

      if (largeFont != null) {
        LargeFont = largeFont.ToNative();
      }

      if (smallFont != null) {
        SmallFont = smallFont.ToNative();
      }

      CallStack = new NativeArray<ProgramCounter>(128, Allocator.Persistent);
      ArgStack = new NativeArray<ushort>(128, Allocator.Persistent);

      PressedKeys = new NativeArray<bool>(256, Allocator.Persistent);
    }

    public void Dispose() {
      ResetAllFiles();

      if (ArchiveFiles.IsCreated) ArchiveFiles.Dispose();
      if (FileMetadata.IsCreated) FileMetadata.Dispose();
      if (MountedFiles.IsCreated) MountedFiles.Dispose();
      if (Memory.IsCreated) Memory.Dispose();

      if (LargeFont.Data.IsCreated) LargeFont.Dispose();
      if (SmallFont.Data.IsCreated) SmallFont.Dispose();

      if (CallStack.IsCreated) CallStack.Dispose();
      if (ArgStack.IsCreated) ArgStack.Dispose();

      if (PressedKeys.IsCreated) PressedKeys.Dispose();
    }

    public void ResetAllFiles() {
      if (ArchiveFiles.IsCreated) {
        foreach (KVPair<FixedString32Bytes, UnsafeList<byte>> pair in ArchiveFiles) {
          pair.Value.Dispose();
        }
        ArchiveFiles.Clear();
      }

      if (MountedFiles.IsCreated) MountedFiles.Clear();
      if (FileMetadata.IsCreated) FileMetadata.Clear();
    }

    public void Reset(Machine machine) {
      CallStackTop = 0;
      ArgStackTop = 0;

      FileMetadata.Clear();
      MountedFiles.Clear();

      for (int i = 0; i < Memory.Length; i++) {
        Memory[i] = 255;
      }

      TextCursorX = 0;
      TextCursorY = 0;
      DispCursorX = 0;
      DispCursorY = 0;

      foreach ((var addr, var size, var name) in Machine.BuiltInMemoryLocations) {
        for (int i = 0; i < size; i++) {
          Memory[addr + i] = 0;
        }
      }

      for (int i = 0; i < machine.Program.Data.Count; i++) {
        Memory[i + Machine.ADDR_PROGRAM_MEMORY] = machine.Program.Data[i];
      }

      //Make sure to init random to non-zero state
      Random = new Unity.Mathematics.Random(123456);

      CallStack[CallStackTop++] = new ProgramCounter() {
        LineIndex = 0,
        OpIndex = 0
      };
    }

    public void PushArg() {
      ArgStack[ArgStackTop++] = HL;
    }

    public void PushArg(ushort value) {
      ArgStack[ArgStackTop++] = value;
    }

    public ushort PopArg() {
      return ArgStack[--ArgStackTop];
    }

    public void Jump(int line, int op = 0) {
      PC = new ProgramCounter() {
        LineIndex = line,
        OpIndex = op
      };
    }

    public void Jump(ProgramCounter location) {
      PC = location;
    }

    public bool GetKey(int code) {
      return PressedKeys[code];
    }

    public ushort Read_U16(int addr) {
      int l = Memory[addr];
      int h = Memory[addr + 1];
      return (ushort)(h << 8 | l);
    }

    public short Read_S16(int addr) {
      int l = Memory[addr];
      int h = Memory[addr + 1];
      return (short)(h << 8 | l);
    }

    public ushort Read_U8(int addr) {
      return Memory[addr];
    }

    public short Read_S8(int addr) {
      return (sbyte)Memory[addr];
    }

    public void Write_U8(int addr, ushort value) {
      if (addr >= Machine.ADDR_PROGRAM_MEMORY && addr < (Machine.ADDR_PROGRAM_MEMORY + 0x4000)) {
        //Debug.LogError("Writing to program memory!");
      }

      Memory[addr] = (byte)(value & 0xFF);
    }

    public void Write_U16(int addr, ushort value) {
      int l = value & 0xFF;
      int h = (value >> 8) & 0xFF;

      if (addr >= Machine.ADDR_PROGRAM_MEMORY && addr < (Machine.ADDR_PROGRAM_MEMORY + 0x4000)) {
        //Debug.LogError("Writing to program memory!");
      }

      Memory[addr] = (byte)l;
      Memory[addr + 1] = (byte)h;
    }

    public ushort ReadFile_U16(ushort fileId, int addr) {
      var data = GetArchiveFileData(fileId);
      if (addr < 0 || addr >= data.Length) {
        return ushort.MaxValue;
      }
      return (ushort)(data[addr] | (data[addr + 1] << 8));
    }
    public ushort ReadFile_U8(ushort fileId, int addr) {
      var data = GetArchiveFileData(fileId);
      if (addr < 0 || addr >= data.Length) {
        return ushort.MaxValue;
      }
      return data[addr];
    }

    public UnsafeList<byte> GetArchiveFileData(ushort fileId) {
      if (!MountedFiles.TryGetValue(fileId, out var fileName)) {
        throw new InvalidOperationException($"Could not load file at index {fileId}, was it loaded?");
      }

      return GetArchiveFileData(fileName);
    }

    public UnsafeList<byte> GetArchiveFileData(FixedString32Bytes fileName) {
      foreach (var metaData in FileMetadata) {
        if (metaData.Name == fileName) {
          if (metaData.IsArchived) {
            if (ArchiveFiles.TryGetValue(fileName, out var data)) {
              return data;
            } else {
              throw new InvalidOperationException($"Tried to get data for archived file {fileName} but it was missing!");
            }
          } else {
            unsafe {
              return new UnsafeList<byte>((byte*)Memory.GetUnsafePtr(), Memory.Length);
            }
          }
        }
      }

      throw new InvalidOperationException($"Tried to get file with name {fileName} but it wasn't found!");
    }

    public NativeSlice<byte> GetBuffer(int addr, int size) {
      return Memory.Slice(addr, size);
    }

    public UnsafeList<byte> CreateArchiveFile(FixedString32Bytes name, int size) {
      UnsafeList<byte> file = new UnsafeList<byte>(size, Allocator.Persistent);
      file.Length = size;
      ArchiveFiles[name] = file;

      FileMetadata.Add(new FileMetadata() {
        Name = name,
        Address = 0,
        Size = (ushort)size,
        IsArchived = true
      });
      return file;
    }

    public bool TryCreateRAMFile(FixedString32Bytes name, int size, out int addr) {
      const int HEADER_SIZE = 2;

      int sizeWithHeader = size + HEADER_SIZE;

      int addrStart = Machine.ADDR_FREE_RAM;
      int addrEnd = addrStart + sizeWithHeader;

      while (true) {
        bool isSpaceFree = true;

        foreach (var entry in FileMetadata) {
          if (entry.IsArchived) {
            continue;
          }

          var entryStart = entry.Address;
          var entryEnd = entryStart + entry.Size;

          if (entryStart < addrEnd && entryEnd > addrStart) {
            isSpaceFree = false;
            break;
          }
        }

        if (isSpaceFree) {
          break;
        }

        int newAddr = int.MaxValue;
        foreach (var entry in FileMetadata) {
          var candidateAddr = entry.Address + entry.Size;

          if (candidateAddr <= addrStart) {
            continue;
          }

          if (candidateAddr < newAddr) {
            newAddr = candidateAddr;
          }
        }

        if (newAddr == int.MaxValue) {
          addr = 0;
          return false;
        }

        addrStart = newAddr;
        addrEnd = addrStart + sizeWithHeader;
      }

      Write_U16(addrStart, (ushort)size);

      FileMetadata.Add(new FileMetadata() {
        Name = name,
        Address = (ushort)(addrStart + HEADER_SIZE),
        Size = (ushort)size,
        IsArchived = false
      });

      addr = (addrStart + HEADER_SIZE);
      return true;
    }

    public int IndexOfFile(FixedString32Bytes name) {
      for (int i = 0; i < FileMetadata.Length; i++) {
        if (FileMetadata[i].Name == name) {
          return i;
        }
      }
      return -1;
    }

    public bool DeleteFile(FixedString32Bytes name) {
      int index = IndexOfFile(name);
      if (index < 0) {
        return false;
      }

      var file = FileMetadata[index];
      if (file.IsArchived) {
        if (ArchiveFiles.TryGetValue(name, out var data)) {
          data.Dispose();
          ArchiveFiles.Remove(name);
        } else {
          throw new InvalidOperationException($"Tried to get data for archived file {name} but it was missing!");
        }
      }

      FileMetadata.RemoveAt(index);
      return true;
    }
  }
}

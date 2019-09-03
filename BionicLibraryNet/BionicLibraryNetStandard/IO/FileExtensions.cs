using System;

namespace BionicLibrary.NetStandard.IO
{
  [Flags]
  public enum FileExtensions
  {
    NotDefined = 0,
    Any = 1,
    Log = 2,
    Txt = 4,
    Ini = 8,
    Csv = 16,
    Bat = 32,
    Bak = 64,
    Config = 128,
    Sys = 256,
    Reg = 512,
    Info = 1024,
    Inf = 2048,
    Help = 4096,
    Hlp = 8192,
    Zip = 16384,
    Bin = 32768,
    Old = 65536,
    Iii = 131072,
    Xml = 262144,
    Jpg = 524288,
    Jpeg = 1048576,
    Bmp = 2097152,
    Exe = 4194304,
    Com = 8388608,
    Cgc = 16777216,
    Cgt = 33554432,
    Cfg = 67108864,
    Png = 134217728,
    Dll = 268435456
  }
}

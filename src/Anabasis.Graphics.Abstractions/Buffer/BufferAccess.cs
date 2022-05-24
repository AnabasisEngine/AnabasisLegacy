namespace Anabasis.Graphics.Abstractions.Buffer;

[Flags]
public enum BufferAccess
{
    None       = 0,
    Read       = 1,
    Write      = 2,
    ReadWrite  = Read | Write,
    Persistent = 4,
    Coherent   = 8,
    Dynamic    = 16,
    DefaultMap = Coherent | Persistent | Write,
}
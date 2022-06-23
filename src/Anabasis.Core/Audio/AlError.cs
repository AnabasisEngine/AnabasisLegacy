namespace Anabasis.Core.Audio;

public enum AlError
{
    /// <summary>There is no current error.</summary>
    NoError = 0,
    /// <summary>
    /// No Device. The device handle or specifier names an inaccessible driver/server.
    /// </summary>
    AudioInvalidDevice = 40961, // 0x0000A001
    /// <summary>
    /// Invalid context ID. The Context argument does not name a valid context.
    /// </summary>
    AudioInvalidContext = 40962, // 0x0000A002
    /// <summary>
    /// Bad enum. A token used is not valid, or not applicable.
    /// </summary>
    AudioInvalidEnum = 40963, // 0x0000A003
    /// <summary>
    /// Bad value. A value (e.g. Attribute) is not valid, or not applicable.
    /// </summary>
    AudioInvalidValue = 40964, // 0x0000A004
    /// <summary>Out of memory. Unable to allocate memory.</summary>
    AudioOutOfMemory = 40965, // 0x0000A005
    /// <summary>Invalid Name paramater passed to OpenAL call.</summary>
    ContextInvalidName = 140961, // 0x0000A001
    /// <summary>Invalid parameter passed to OpenAL call.</summary>
    ContextInvalidValue = 140962, // 0x0000A002
    /// <summary>Invalid OpenAL enum parameter value.</summary>
    ContextInvalidEnum = 140963, // 0x0000A003
    /// <summary>Illegal OpenAL call.</summary>
    ContextIllegalCommand = 140964, // 0x0000A004
    /// <summary>Illegal OpenAL call.</summary>
    ContextInvalidOperation = 140964, // 0x0000A004
    /// <summary>No OpenAL memory left.</summary>
    ContextOutOfMemory = 140965, // 0x0000A005
}
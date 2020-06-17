# Halforbit BitBuffers

Turn your data objects into bits, then turn your bits into data objects, with precision, ease, speed, and efficiency. 

BitBuffers allows you to make custom serializers and deserializers with sub-byte efficiency (think eight boolean flags in a single byte, or perhaps two integers at four bits each in a single byte), as well as branching, where a single bit can determine how the rest of the payload should be interpreted. 

BitBuffers is a forked subset of the excellent [Lidgren Network](https://github.com/lidgren/lidgren-network-gen3) library, which was designed for realtime games and simulations, so it is fast and efficient.

## Serialize an object into bytes

Create a new `BitWriter` and use the fluent `Write_` interface to transform your object into byte array:

```cs
protected override byte[] Serialize(Record value)
{
    return new BitWriter()
        .Write(value.Volume.Name)
        .Write(value.Location.Path)
        .Write(value.Location.Filename)
        .Write(value.Location.Extension)
        .WriteEpochSeconds(value.Time.CreatedTime)
        .WriteEpochSeconds(value.Time.UpdatedTime)
        .WriteEpochSeconds(value.Time.ObservedTime)
        .WriteEnum(value.Flags, numberOfBits: 7)
        .Write(value.Content.ContentHash.ToArray())
        .Write(value.Content.Length);
}
```

Here we write dates efficiently as epoch-second integers. We also store an `enum` value using seven bits of precision.

Notice that a `BitWriter` will implicitly convert to a `byte[]`. Or you can call `.ToArray()`.

## Deserialize bytes to an object

Create a `BitReader` with your byte array, and use the `Read_` methods:

```cs
protected override Record Deserialize(byte[] data)
{
    var b = new BitReader(data);

    return new Record(
        volume: new VolumeKey(
            name: b.ReadString()),
        location: new LocationKey(
            path: b.ReadString(),
            filename: b.ReadString(),
            extension: b.ReadString()),
        time: new TimeInfo(
            createdTime: b.ReadEpochSeconds(),
            updatedTime: b.ReadEpochSeconds(),
            observedTime: b.ReadEpochSeconds()),
        flags: b.ReadEnum<RecordFlag>(7),
        content: new ContentKey(
            contentHash: ContentHash.FromArray(b.ReadBytes(32)),
            length: b.ReadInt64()));
}
```

You can also use a `BitReader` and its `out` variations to deserialize values to local variables easily:

```cs
new BitReader(data)
    .ReadString(out var volumeKey)
    .ReadString(out var path)
    .ReadString(out var filename)
    .ReadString(out var extension)
    .ReadEpochSeconds(out var createdTime)
    .ReadEpochSeconds(out var updatedTime)
    .ReadEpochSeconds(out var observedTime)
    .ReadEnum<RecordFlag>(7, out var flags)
    .ReadBytes(32, out var contentHash)
    .ReadInt64(out var length);
```

## Sub-Byte Efficiency

Many overloads let you specify how many _bits_ you want to write and read for your data. You could, for example, have eight single-bit boolean values stored in a single byte:

```cs
return new BitWriter()
    .Write(true)
    .Write(false)
    .Write(true)
    .Write(false)
    .Write(true)
    .Write(false)
    .Write(true)
    .Write(false);
```

and retrieve them:

```cs
var r = new BitReader(bytes);

return new MyFlags(
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean(),
    r.ReadBoolean());
```

Or you could store two four-bit integers (with a range of 0-15), in a single byte:

```cs
return new BitWriter()
    .Write(14, numberOfBits: 4)
    .Write(9, numberOfBits: 4);
```

and retrieve them:

```cs
var r = new BitReader(bytes);

return MyIntegers(
    r.ReadInt32(4),
    r.ReadInt32(4));
```

## Branching

You can write one-bit boolean values to indicate branching:

```cs
var writer = new BitWriter();

writer.Write(IsLargePayload);

if (IsLargePayload)
{
    writer
        // Write a large payload here.
}
else
{
    writer
        // Write a small payload here.
}
```

and then interpret the bit:

```cs
var reader = new BitReader(bytes);

if (reader.ReadBoolean())
{
    reader
        // Read a large payload here.
}
else
{
    reader
        // Read a small payload here.
}
```

## Get It

BitBuffers is released as a nuget package, [Halforbit.BitBuffers](https://www.nuget.org/packages/Halforbit.BitBuffers).
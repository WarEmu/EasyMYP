using System;
using System.Security.Cryptography;

public class Crc_simple_32
{
    uint[] table;

    public uint ComputeChecksum(byte[] bytes)
    {
        uint crc = 0xffffffff;
        for (int i = 0; i < bytes.Length; i++)
        {
            byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
            crc = (uint)((crc >> 8) ^ table[index]);
        }
        return ~crc;
    }

    public Crc_simple_32()
    {
        uint poly = 0xedb88320;
        table = new uint[256];
        uint temp = 0;
        for (int i = 0; i < table.Length; i++)
        {
            temp = (uint)i;
            for (int j = 8; j > 0; j--)
            {
                if ((temp & 1) == 1)
                {
                    temp = (uint)((temp >> 1) ^ poly);
                }
                else
                {
                    temp >>= 1;
                }
            }
            table[i] = temp;
        }
    }
}


public class Crc32 : HashAlgorithm
{
    public const UInt32 DefaultPolynomial = 0xedb88320;
    public const UInt32 DefaultSeed = 0xffffffff;

    private UInt32 hash;
    private UInt32 seed;
    private UInt32[] table;

    public Crc32()
    {
        table = InitializeTable(DefaultPolynomial);
        seed = DefaultSeed;
        Initialize();
    }

    public Crc32(UInt32 polynomial, UInt32 seed)
    {
        table = InitializeTable(polynomial);
        this.seed = seed;
        Initialize();
    }

    public override void Initialize()
    {
        hash = seed;
    }

    protected override void HashCore(byte[] buffer, int start, int length)
    {
        hash = CalculateHash(table, hash, buffer, start, length);
    }

    protected override byte[] HashFinal()
    {
        byte[] hashBuffer = UInt32ToBigEndianBytes(hash);
        this.HashValue = hashBuffer;
        return hashBuffer;
    }

    public override int HashSize
    {
        get { return 32; }
    }

    public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
    {
        return CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
    }

    private static UInt32[] InitializeTable(UInt32 polynomial)
    {
        UInt32[] createTable = new UInt32[256];
        for (int i = 0; i < 256; i++)
        {
            UInt32 entry = (UInt32)i;
            for (int j = 0; j < 8; j++)
                if ((entry & 1) == 1)
                    entry = (entry >> 1) ^ polynomial;
                else
                    entry = entry >> 1;
            createTable[i] = entry;
        }
        return createTable;
    }

    private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
    {
        UInt32 crc = seed;
        for (int i = start; i < size; i++)
            unchecked
            {
                crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
            }
        return ~crc;
    }

    private byte[] UInt32ToBigEndianBytes(UInt32 x)
    {
        return new byte[] {
			(byte)((x >> 24) & 0xff),
			(byte)((x >> 16) & 0xff),
			(byte)((x >> 8) & 0xff),
			(byte)(x & 0xff)
		};
    }
}
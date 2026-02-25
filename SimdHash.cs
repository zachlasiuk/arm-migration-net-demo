using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

public static class SimdHash
{
    // Minimal-ish "hash": fold bytes into 32-bit using AVX2 lane ops.
    // Intentionally unguarded x86 intrinsics -> not Arm-friendly.
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static uint Hash32(ReadOnlySpan<byte> data)
    {
        // Force use of AVX2 types/ops (intentionally bad for Arm).
        Vector256<uint> acc = Vector256<uint>.Zero;

        int i = 0;
        while (i + 32 <= data.Length)
        {
            // Load 32 bytes -> widen-ish and xor into uint lanes (toy example).
            // We use AVX2 to shuffle/xor to make the "x86 intrinsics" obvious.
            var v = Vector256.Create(
                BitConverter.ToUInt32(data.Slice(i + 0, 4)),
                BitConverter.ToUInt32(data.Slice(i + 4, 4)),
                BitConverter.ToUInt32(data.Slice(i + 8, 4)),
                BitConverter.ToUInt32(data.Slice(i + 12, 4)),
                BitConverter.ToUInt32(data.Slice(i + 16, 4)),
                BitConverter.ToUInt32(data.Slice(i + 20, 4)),
                BitConverter.ToUInt32(data.Slice(i + 24, 4)),
                BitConverter.ToUInt32(data.Slice(i + 28, 4))
            );

            // AVX2 XOR
            acc = Avx2.Xor(acc, v);
            i += 32;
        }

        // Horizontal-ish fold (toy)
        uint h =
            acc.GetElement(0) ^ acc.GetElement(1) ^ acc.GetElement(2) ^ acc.GetElement(3) ^
            acc.GetElement(4) ^ acc.GetElement(5) ^ acc.GetElement(6) ^ acc.GetElement(7);

        // Tail bytes
        for (; i < data.Length; i++)
            h = (h * 16777619) ^ data[i];

        return h;
    }
}
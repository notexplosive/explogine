namespace ExplogineCore;

/*
 *    Provides a noise function which is fast and has a pseudorandom distribution.
 *    
 *    Original algorithm by Squirrel Eiserloh,
 *    presented at "Math for Game Programmers: Noise-based RNG", Game Developers Conference, 2017.
 *
 *    With further modifications inspired by Squirrel5, also by Squirrel Eiserloh 
 *    http://eiserloh.net/noise/SquirrelNoise5.hpp
 *    
 *    Adapted to C# by NotExplosive
 */
public static class Squirrel3
{
    public static uint Noise(int position, uint seed)
    {
        // 1101 0010 1010 1000 0000 1010 0011 1111
        const uint sq5BitNoise1 = 0xd2a80a3f; 
        // 1010 1000 1000 0100 1111 0001 1001 0111
        const uint sq5BitNoise2 = 0xa884f197; 
        // 0110 1100 0111 0011 0110 1111 0100 1011
        const uint sq5BitNoise3 = 0x6C736F4B;
        // 1011 0111 1001 1111 0011 1010 1011 1011 
        const uint sq5BitNoise4 = 0xB79F3ABB; 
        // 0001 1011 0101 0110 1100 0100 1111 0101
        const uint sq5BitNoise5 = 0x1b56c4f5; 

        var mangledBits = (uint) position;
        mangledBits *= sq5BitNoise1;
        mangledBits += seed;
        mangledBits ^= mangledBits >> 9;
        mangledBits += sq5BitNoise2;
        mangledBits ^= mangledBits >> 11;
        mangledBits *= sq5BitNoise3;
        mangledBits ^= mangledBits >> 13;
        mangledBits += sq5BitNoise4;
        mangledBits ^= mangledBits >> 15;
        mangledBits *= sq5BitNoise5;
        mangledBits ^= mangledBits >> 17;
        return mangledBits;

        return mangledBits;
    }
}

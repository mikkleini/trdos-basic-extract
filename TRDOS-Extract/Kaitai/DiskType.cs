/// <summary>
/// Origin: http://formats.kaitai.io/tr_dos_image/  
/// Done some aesthetic updates.
/// </summary>

namespace Kaitai
{
    public enum DiskType : byte
    {
        Type80TracksDoubleSide = 22,
        Type40TracksDoubleSide = 23,
        Type80TracksSingleSide = 24,
        Type40TracksSingleSide = 25,
    }
}

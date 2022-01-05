/// <summary>
/// Origin: http://formats.kaitai.io/tr_dos_image/  
/// Done some aesthetic updates.
/// </summary>

using Kaitai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaitai
{
    public partial class VolumeInfo : KaitaiStruct
    {
        public static VolumeInfo FromFile(string fileName)
        {
            return new VolumeInfo(new KaitaiStream(fileName));
        }

        public VolumeInfo(KaitaiStream p__io, TrDosImage p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            f_numTracks = false;
            f_numSides = false;
            Read();
        }

        private void Read()
        {
            CatalogEnd = m_io.ReadBytes(1);
            if (!((KaitaiStream.ByteArrayCompare(CatalogEnd, new byte[] { 0 }) == 0)))
            {
                throw new ValidationNotEqualError(new byte[] { 0 }, CatalogEnd, M_Io, "/types/volume_info/seq/0");
            }
            Unused = m_io.ReadBytes(224);
            FirstFreeSectorSector = m_io.ReadU1();
            FirstFreeSectorTrack = m_io.ReadU1();
            DiskType = ((DiskType)m_io.ReadU1());
            NumFiles = m_io.ReadU1();
            NumFreeSectors = m_io.ReadU2le();
            TrDosId = m_io.ReadBytes(1);
            if (!((KaitaiStream.ByteArrayCompare(TrDosId, new byte[] { 16 }) == 0)))
            {
                throw new ValidationNotEqualError(new byte[] { 16 }, TrDosId, M_Io, "/types/volume_info/seq/7");
            }
            Unused2 = m_io.ReadBytes(2);
            Password = m_io.ReadBytes(9);
            Unused3 = m_io.ReadBytes(1);
            NumDeletedFiles = m_io.ReadU1();
            Label = m_io.ReadBytes(8);
            _unused4 = m_io.ReadBytes(3);
        }

        private bool f_numTracks;
        private sbyte _numTracks;
        public sbyte NumTracks
        {
            get
            {
                if (f_numTracks)
                    return _numTracks;
                _numTracks = (sbyte)((((byte)DiskType & 1) != 0 ? 40 : 80));
                f_numTracks = true;
                return _numTracks;
            }
        }

        private bool f_numSides;
        private sbyte _numSides;
        public sbyte NumSides
        {
            get
            {
                if (f_numSides)
                    return _numSides;
                _numSides = (sbyte)((((byte)DiskType & 8) != 0 ? 1 : 2));
                f_numSides = true;
                return _numSides;
            }
        }

        private byte[] _unused4;
        public byte[] CatalogEnd { get; private set; }
        public byte[] Unused { get; private set; }
        public byte FirstFreeSectorSector { get; private set; }

        /// <summary>
        /// track number is logical, for double-sided disks it's
        /// (physical_track &lt;&lt; 1) | side, the same way that tracks are stored
        /// sequentially in .trd file
        /// </summary>
        public byte FirstFreeSectorTrack { get; private set; }
        public DiskType DiskType { get; private set; }

        /// <summary>
        /// Number of non-deleted files. Directory can have more than
        /// number_of_files entries due to deleted files
        /// </summary>
        public byte NumFiles { get; private set; }
        public ushort NumFreeSectors { get; private set; }
        public byte[] TrDosId { get; private set; }
        public byte[] Unused2 { get; private set; }
        public byte[] Password { get; private set; }
        public byte[] Unused3 { get; private set; }
        public byte NumDeletedFiles { get; private set; }
        public byte[] Label { get; private set; }
        public string LabelASCII => ASCIIEncoding.ASCII.GetString(Label);
        public byte[] Unused4 { get { return _unused4; } }
        public TrDosImage Root { get; }
        public TrDosImage Parent { get; }
    }
}

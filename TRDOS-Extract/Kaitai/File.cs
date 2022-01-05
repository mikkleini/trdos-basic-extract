/// <summary>
/// Origin: http://formats.kaitai.io/tr_dos_image/  
/// Done some aesthetic updates.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaitai
{
    public partial class File : KaitaiStruct
    {
        public static File FromFile(string fileName)
        {
            return new File(new KaitaiStream(fileName));
        }

        public File(KaitaiStream p__io, TrDosImage p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            f_isDeleted = false;
            f_isTerminator = false;
            f_contents = false;
            Read();
        }

        private void Read()
        {
            RawName = m_io.ReadBytes(8);
            var io___raw_name = new KaitaiStream(RawName);
            Name = new Filename(io___raw_name, this, Root);
            Extension = m_io.ReadU1();
            switch (Extension)
            {
                case 66:
                    {
                        PositionAndLength = new PositionAndLengthBasic(m_io, this, Root);
                        break;
                    }
                case 67:
                    {
                        PositionAndLength = new PositionAndLengthCode(m_io, this, Root);
                        break;
                    }
                case 35:
                    {
                        PositionAndLength = new PositionAndLengthPrint(m_io, this, Root);
                        break;
                    }
                default:
                    {
                        PositionAndLength = new PositionAndLengthGeneric(m_io, this, Root);
                        break;
                    }
            }
            LengthSectors = m_io.ReadU1();
            StartingSector = m_io.ReadU1();
            StartingTrack = m_io.ReadU1();
        }
        private bool f_isDeleted;
        private bool _isDeleted;
        public bool IsDeleted
        {
            get
            {
                if (f_isDeleted)
                    return _isDeleted;
                _isDeleted = (bool)(Name.FirstByte == 1);
                f_isDeleted = true;
                return _isDeleted;
            }
        }
        private bool f_isTerminator;
        private bool _isTerminator;
        public bool IsTerminator
        {
            get
            {
                if (f_isTerminator)
                    return _isTerminator;
                _isTerminator = (bool)(Name.FirstByte == 0);
                f_isTerminator = true;
                return _isTerminator;
            }
        }
        private bool f_contents;
        private byte[] _contents;
        public byte[] Contents
        {
            get
            {
                if (f_contents)
                    return _contents;
                long _pos = m_io.Pos;
                m_io.Seek((((StartingTrack * 256) * 16) + (StartingSector * 256)));
                _contents = m_io.ReadBytes((LengthSectors * 256));
                m_io.Seek(_pos);
                f_contents = true;
                return _contents;
            }
        }

        public Filename Name { get; private set; }
        public byte Extension { get; private set; }
        public KaitaiStruct PositionAndLength { get; private set; }
        public byte LengthSectors { get; private set; }
        public byte StartingSector { get; private set; }
        public byte StartingTrack { get; private set; }
        public TrDosImage Root { get; }
        public TrDosImage Parent { get; }
        public byte[] RawName { get; private set; }

        /// <summary>
        /// Get name which is made of visible ASCII characters
        /// </summary>
        public string RawNameASCII => ASCIIEncoding.ASCII.GetString(
            RawName.Select(b => (b < 0x20 ? (byte)'_' : (b >= 0x80 ? (byte)'~': b)))
            .ToArray()).TrimEnd();
    }
}

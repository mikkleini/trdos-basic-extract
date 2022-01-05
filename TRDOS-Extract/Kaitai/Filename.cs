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
    public partial class Filename : KaitaiStruct
    {
        public static Filename FromFile(string fileName)
        {
            return new Filename(new KaitaiStream(fileName));
        }

        public Filename(KaitaiStream p__io, File p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            f_firstByte = false;
            Read();
        }
        
        private void Read()
        {
            Name = m_io.ReadBytes(8);
        }

        private bool f_firstByte;
        private byte _firstByte;

        public byte FirstByte
        {
            get
            {
                if (f_firstByte)
                    return _firstByte;
                long _pos = m_io.Pos;
                m_io.Seek(0);
                _firstByte = m_io.ReadU1();
                m_io.Seek(_pos);
                f_firstByte = true;
                return _firstByte;
            }
        }

        public byte[] Name { get; private set; }
        public string NameASCII => ASCIIEncoding.ASCII.GetString(Name);
        public TrDosImage Root { get; }
        public File Parent { get; }
    }
}

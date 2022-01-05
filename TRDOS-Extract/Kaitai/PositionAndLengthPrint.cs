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
    public partial class PositionAndLengthPrint : KaitaiStruct
    {
        public static PositionAndLengthPrint FromFile(string fileName)
        {
            return new PositionAndLengthPrint(new KaitaiStream(fileName));
        }

        public PositionAndLengthPrint(KaitaiStream p__io, File p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            Read();
        }

        private void Read()
        {
            ExtentNo = m_io.ReadU1();
            Reserved = m_io.ReadU1();
            Length = m_io.ReadU2le();
        }

        public byte ExtentNo { get; private set; }
        public byte Reserved { get; private set; }
        public ushort Length { get; private set; }
        public TrDosImage Root { get; }
        public File Parent { get; }
    }

}

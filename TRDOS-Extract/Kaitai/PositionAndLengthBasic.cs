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
    public partial class PositionAndLengthBasic : KaitaiStruct
    {
        public static PositionAndLengthBasic FromFile(string fileName)
        {
            return new PositionAndLengthBasic(new KaitaiStream(fileName));
        }

        public PositionAndLengthBasic(KaitaiStream p__io, File p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            Read();
        }

        private void Read()
        {
            ProgramAndDataLength = m_io.ReadU2le();
            ProgramLength = m_io.ReadU2le();
        }

        public ushort ProgramAndDataLength { get; private set; }
        public ushort ProgramLength { get; private set; }
        public TrDosImage Root { get; }
        public File Parent { get; }
    }
}

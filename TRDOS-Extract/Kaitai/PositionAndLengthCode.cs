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
    public partial class PositionAndLengthCode : KaitaiStruct
    {
        public static PositionAndLengthCode FromFile(string fileName)
        {
            return new PositionAndLengthCode(new KaitaiStream(fileName));
        }

        public PositionAndLengthCode(KaitaiStream p__io, File p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root;
            Read();
        }

        private void Read()
        {
            StartAddress = m_io.ReadU2le();
            Length = m_io.ReadU2le();
        }

        /// <summary>
        /// Default memory address to load this byte array into
        /// </summary>
        public ushort StartAddress { get; private set; }
        public ushort Length { get; private set; }
        public TrDosImage Root { get; }
        public File Parent { get; }
    }
}

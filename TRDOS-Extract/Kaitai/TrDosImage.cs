/// <summary>
/// Origin: http://formats.kaitai.io/tr_dos_image/  
/// Done some aesthetic updates.
/// </summary>

using System.Collections.Generic;

namespace Kaitai
{
    /// <summary>
    /// .trd file is a raw dump of TR-DOS (ZX-Spectrum) floppy. .trd files are
    /// headerless and contain consequent &quot;logical tracks&quot;, each logical track
    /// consists of 16 256-byte sectors.
    /// 
    /// Logical tracks are defined the same way as used by TR-DOS: for single-side
    /// floppies it's just a physical track number, for two-side floppies sides are
    /// interleaved, i.e. logical_track_num = (physical_track_num &lt;&lt; 1) | side
    /// 
    /// So, this format definition is more for TR-DOS filesystem than for .trd files,
    /// which are formatless.
    /// 
    /// Strings (file names, disk label, disk password) are padded with spaces and use
    /// ZX Spectrum character set, including UDGs, block drawing chars and Basic
    /// tokens. ASCII range is mostly standard ASCII, with few characters (^, `, DEL)
    /// replaced with (up arrow, pound, copyright symbol).
    /// 
    /// .trd file can be smaller than actual floppy disk, if last logical tracks are
    /// empty (contain no file data) they can be omitted.
    /// </summary>
    public partial class TrDosImage : KaitaiStruct
    {
        public static TrDosImage FromFile(string fileName)
        {
            return new TrDosImage(new KaitaiStream(fileName));
        }

        public TrDosImage(KaitaiStream p__io, KaitaiStruct p__parent = null, TrDosImage p__root = null) : base(p__io)
        {
            Parent = p__parent;
            Root = p__root ?? this;
            f_volumeInfo = false;
            Read();
        }

        private void Read()
        {
            Files = new List<File>();
            {
                File file;
                do {
                    file = new File(m_io, this, Root);

                    // Not a file record anymore ?
                    if (file.RawName.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }))
                    {
                        break;
                    }

                    Files.Add(file);
                //} while (!(M_.IsTerminator));
                } while (true);
            }
        }

        private bool f_volumeInfo;
        private VolumeInfo _volumeInfo;
        public VolumeInfo VolumeInfo
        {
            get
            {
                if (f_volumeInfo)
                    return _volumeInfo;
                long _pos = m_io.Pos;
                m_io.Seek(2048);
                _volumeInfo = new VolumeInfo(m_io, this, Root);
                m_io.Seek(_pos);
                f_volumeInfo = true;
                return _volumeInfo;
            }
        }

        public List<File> Files { get; private set; }
        public TrDosImage Root { get; }
        public KaitaiStruct Parent { get; }
    }
}
